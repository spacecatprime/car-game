using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// http://docs.unity3d.com/Manual/HOWTO-UICreateFromScripting.html

public class ActionPanel : MonoBehaviour {

    public RectTransform playBoard;

    private List<RectTransform> actionList = new List<RectTransform>();

	void Start () 
	{
	}
	
	void Update () 
	{
	}

    private void PushButton(string prefabName)
    {
        RectTransform prefab = Resources.Load<RectTransform>(prefabName);
        RectTransform button = Instantiate<RectTransform>(prefab);
        button.SetParent(playBoard);
        actionList.Add(button);
    }

    private void PopButton()
    {
        if (actionList.Count > 0)
        {
            var last = actionList[actionList.Count - 1];
            GameObject.DestroyImmediate(last.gameObject);
            actionList.RemoveAt(actionList.Count - 1);
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
        PushButton("ui_go_execute");
    }

	public void DoCancel()
	{
        //AddButton("ui_stop");
        PopButton();
    }
}
