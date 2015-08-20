using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Used to manage active dialog stacks
/// </summary>
public class WindowManager : MonoBehaviour 
{
    public Canvas parentCanvas;

    private Stack<GameObject> m_menuStack = new Stack<GameObject>();
    private Dictionary<string, GameObject> m_windowTypeCache = new Dictionary<string, GameObject>();

	void Start () 
    {
	}

	void Update () 
    {
	}

    public GameObject PushWindow(string path, object data)
    {
        // load prefab and/or cache
        GameObject gameObj = null;
        if (m_windowTypeCache.ContainsKey(path))
        {
            gameObj = m_windowTypeCache[path];
        }
        else
        {
            gameObj = Resources.Load(path) as GameObject;
            if (gameObj == null)
            {
                return null;
            }
            m_windowTypeCache.Add(path, gameObj);
        }

        GameObject newWindow = GameObject.Instantiate(gameObj);
        newWindow.transform.SetParent(parentCanvas.transform, false);

        if (m_menuStack.Count > 0)
        {
            GameObject currentWindow = m_menuStack.Peek();
            currentWindow.SendMessage("Conceal", this, SendMessageOptions.RequireReceiver);
            currentWindow.SetActive(false);
        }

        m_menuStack.Push(newWindow);
        newWindow.SetActive(true);
        newWindow.SendMessage("SetupData", data, SendMessageOptions.RequireReceiver);
        newWindow.SendMessage("Reveal", this, SendMessageOptions.RequireReceiver);
        return newWindow;
    }

    public void PopWindow()
    {
        // nix current queued window
        if (m_menuStack.Count > 0)
        {
            GameObject currentWindow = m_menuStack.Pop();
            currentWindow.SetActive(true);
            currentWindow.SendMessage("Conceal", this, SendMessageOptions.RequireReceiver);
            GameObject.Destroy(currentWindow);
        }

        // enable then next in the queue, if any
        if (m_menuStack.Count > 0)
        {
            GameObject nextWindow = m_menuStack.Peek();
            nextWindow.SetActive(true);
            nextWindow.SendMessage("Reveal", this, SendMessageOptions.RequireReceiver);
        }
    }
}
