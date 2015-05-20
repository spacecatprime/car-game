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

    // TMP
    private bool m_activePlayback = false;

	void Start () 
	{
        m_trafficLogic = logic.GetComponent<TrafficLogic>();
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

	public void DoExecute()
	{
        //PushButton("ui_go_execute");
        // should be changed to TrafficLogic.Operation.Playback at some point

        if (m_activePlayback)
        {
            m_trafficLogic.SetOperation(TrafficLogic.Operation.StepUp);
        }
        else
        {
            m_activePlayback = true;
            m_trafficLogic.SetOperation(TrafficLogic.Operation.TurnByTurn, m_actionList);
        }
    }

	public void DoCancel()
	{
        if (m_activePlayback)
        {
            m_actionList.ForEach(a => GameObject.DestroyImmediate(a.gameObject));
            m_actionList.Clear();

            m_activePlayback = false;
            m_trafficLogic.SetOperation(TrafficLogic.Operation.Reset);
        }
        else
        {
            PopButton();
        }
    }
}
