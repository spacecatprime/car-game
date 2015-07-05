using UnityEngine;
using System.Collections.Generic;

public class samples : MonoBehaviour 
{
    public GameObject dialogBox;
    public Canvas parent;
    public WindowManager windowManager;

    private GameObject m_lastUI;
    private Vector3 orgPos;
    private Vector3 offgridPos = new Vector3(-10000.0f, 0.0f);
    private RectTransform transform;

	// Use this for initialization
	void Start () 
    {
    	// Prefabs of UI elements are instantiated as normal using the Instantiate method. 
        // When setting the parent of the instantiated UI element, it’s recommended to do it 
        // using the Transform.SetParent method with the worldPositionStays parameter set to false.
        transform = this.GetComponent<RectTransform>();
        orgPos = this.GetComponent<RectTransform>().localPosition;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GameObject.Destroy(m_lastUI);
            m_lastUI = null;
            transform.localPosition = orgPos;
        }
	}

    public void LaunchDialogBox()
    {
        transform.localPosition = offgridPos;

        List<string> data = new List<string> { "my title", "my content", "lhs button", "rhs button" };

        m_lastUI = GameObject.Instantiate(dialogBox);
        m_lastUI.transform.SetParent(parent.transform, false);
        m_lastUI.SetActive(true);
        m_lastUI.SendMessage("SetupData", data, SendMessageOptions.RequireReceiver);
        m_lastUI.GetComponent<DialogBox>().OnButton1 += new System.Action<object>(samples_OnButton1);
    }

    public void LaunchDialogBoxOneButton()
    {
        transform.localPosition = offgridPos;

        List<string> data = new List<string> { "my title", "only one button", "lhs button", "" };

        m_lastUI = GameObject.Instantiate(dialogBox);
        m_lastUI.transform.SetParent(parent.transform, false);
        m_lastUI.SetActive(true);
        m_lastUI.SendMessage("SetupData", data, SendMessageOptions.RequireReceiver);
        m_lastUI.GetComponent<DialogBox>().OnButton1 += new System.Action<object>(samples_OnButton1);
    }

    public void LaunchDialogBoxPush()
    {
        string content = string.Format("menu content {0}", System.DateTime.Now);
        List<string> data = new List<string> { "my menu", content, "back", "next" };
        windowManager.PushWindow("DialogBox", data);
    }

    public void LaunchDialogBoxPop()
    {
        windowManager.PopWindow();
    }

    void ClearUI()
    {
        if (m_lastUI != null)
        {
            GameObject.Destroy(m_lastUI);
            m_lastUI = null;
        }
        transform.localPosition = orgPos;
    }

    void samples_OnButton1(object obj)
    {
        ClearUI();
    }
}
