using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// describes a game level such as Camera and Green Car
/// </summary>
public class GameLevel : MonoBehaviour 
{
    public Camera levelCamera;
    public TrafficLogic trafficLogic;
    public GameLevel nextLevel;

    public string levelTitle;
    public string levelContent;

    private GameFlow m_gameFlow;

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () 
    {	
	}

    public void StepUp()
    {
        trafficLogic.SetOperation(TrafficLogic.Operation.StepUp);
    }

    public void Reset()
    {
        trafficLogic.SetOperation(TrafficLogic.Operation.Reset);
    }

    public void Playback(object actionList)
    {
        if (actionList is List<RectTransform>)
        {
            var list = actionList as List<RectTransform>;
            trafficLogic.SetOperation(TrafficLogic.Operation.Playback, list);
        }
    }

    public void StartLevel(object gameFlow)
    {
        if (Camera.main != null)
        {
            Camera.main.enabled = false;
        }
        levelCamera.enabled = true;
        m_gameFlow = gameFlow as GameFlow;
        m_gameFlow.SendMessage("SetTrafficLogic", this.trafficLogic);
        new WelcomeLevel(m_gameFlow.windowManager, levelTitle, levelContent);
    }

    public void FinishLineTriggered()
    {
        trafficLogic.SendMessage("FinishLineTriggered");
    }

    public class WelcomeLevel : GameFlow.DialogBoxHookup
    {
        static string s_title = "Welcome to the Level";
        static string s_content = "Use the arrows to direct the green car to the end.";
        static string s_okay = "okay";

        public WelcomeLevel(WindowManager winMgr, string title, string content)
            : base(winMgr)
        {
            base.Push(
                string.IsNullOrEmpty(title) ? WelcomeLevel.s_title : title, 
                string.IsNullOrEmpty(content) ? WelcomeLevel.s_content : content, 
                s_okay, 
                "");
        }
    }

}
