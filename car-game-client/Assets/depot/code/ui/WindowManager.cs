using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Used to manage active dialog stacks
/// </summary>
public class WindowManager : MonoBehaviour 
{
    public Canvas parentCanvas;

    private Queue<GameObject> m_menuQueue = new Queue<GameObject>();
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
        newWindow.SetActive(true);
        newWindow.SendMessage("SetupData", data, SendMessageOptions.RequireReceiver);

        if (m_menuQueue.Count > 0)
        {
            m_menuQueue.Peek().SetActive(false);
        }
        m_menuQueue.Enqueue(newWindow);
        return newWindow;
    }

    public void PopWindow()
    {
        // nix current queued window
        if (m_menuQueue.Count > 0)
        {
            GameObject currentWindow = m_menuQueue.Dequeue();
            currentWindow.SendMessage("Hide", this, SendMessageOptions.RequireReceiver);            
            GameObject.Destroy(currentWindow);
        }

        // enable then next in the queue, if any
        if (m_menuQueue.Count > 0)
        {
            GameObject nextWindow = m_menuQueue.Peek();
            nextWindow.SetActive(true);
            nextWindow.SendMessage("Show", this, SendMessageOptions.RequireReceiver);
        }
    }
}
