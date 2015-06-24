using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// http://docs.unity3d.com/Manual/HOWTO-UICreateFromScripting.html

public class ActionPanel : MonoBehaviour 
{
    public RectTransform playBoard;
    public Transform logic;

    private List<RectTransform> m_actionList = new List<RectTransform>();
    private TrafficLogic m_trafficLogic;

    private Button buttonStart;
    private Button buttonStop;
    private Button buttonUndo;
    private List<Button> buttonListOperations;
    
    // Active Playback or not
    private bool m_activePlayback = false;

	void Start () 
	{
        m_trafficLogic = logic.GetComponent<TrafficLogic>();

        buttonStart = GetButtonByTag("GameController");
        buttonStop = GetButtonByTag("Finish");
        buttonUndo = GetButtonByTag("undo");
        buttonListOperations = GetButtonListByTag("operation");
        UpdateVisible();
    }

    private void UpdateVisible()
    {
        if (m_activePlayback)
        {
            buttonStart.gameObject.SetActive(false);
            buttonStop.gameObject.SetActive(true);
            buttonUndo.gameObject.SetActive(false);
            buttonListOperations.ForEach(b => b.gameObject.SetActive(false));
        }
        else
        {
            buttonStart.gameObject.SetActive(true);
            buttonStop.gameObject.SetActive(false);
            buttonUndo.gameObject.SetActive(true);
            buttonListOperations.ForEach(b => b.gameObject.SetActive(true));
        }
    }
	
	void Update () 
	{
	}

    private void PushButton(string prefabName)
    {
        RectTransform prefab = Resources.Load<RectTransform>(prefabName);
        RectTransform button = Instantiate<RectTransform>(prefab);
        button.SetParent(playBoard);
        m_actionList.Add(button);
    }

    private void PopButton()
    {
        if (m_actionList.Count > 0)
        {
            var last = m_actionList[m_actionList.Count - 1];
            GameObject.DestroyImmediate(last.gameObject);
            m_actionList.RemoveAt(m_actionList.Count - 1);
        }
    }

	public void DoLeft()
	{
        PushButton("ui_t_left");
	}

	public void DoRight()
	{
        PushButton("ui_t_right");
    }

	public void DoForward()
	{
        PushButton("ui_go_forward");
    }

    private Button GetButtonByTag(string tag)
    {
        Button[] list = this.GetComponentsInChildren<Button>();
        foreach (Button b in list)
        {
            if (b.tag == tag)
            {
                return b;
            }
        }
        return null;
    }

    private List<Button> GetButtonListByTag(string tag)
    {
        var buttonList = new List<Button>();
        Button[] list = this.GetComponentsInChildren<Button>();
        foreach (Button b in list)
        {
            if (b.tag == tag)
            {
                buttonList.Add(b);
            }
        }
        return buttonList;
    }

    public void DoStep()
    {
        m_trafficLogic.SetOperation(TrafficLogic.Operation.StepUp);
    }

	public void DoExecute()
	{
        if (!m_activePlayback)
        {
            m_activePlayback = true;
            UpdateVisible();
            m_trafficLogic.SetOperation(TrafficLogic.Operation.Playback, m_actionList);
        }
    }

    public void DoUndo()
    {
        if (!m_activePlayback)
        {
            PopButton();
        }
    }

	public void DoCancel()
	{
        if (m_activePlayback)
        {
            m_actionList.ForEach(a => GameObject.DestroyImmediate(a.gameObject));
            m_actionList.Clear();

            m_activePlayback = false;
            UpdateVisible();

            m_trafficLogic.SetOperation(TrafficLogic.Operation.Reset);
        }
    }
}
