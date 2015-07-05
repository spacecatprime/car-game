using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

/// <summary>
/// handles the game UI widget game flow from Splash to First Screen to Win Level
/// since this is such a simple game, maybe contain all the content in this module?
/// </summary>
public class GameFlow : MonoBehaviour 
{
    public WindowManager windowManager;
    public TrafficLogic trafficLogic;

    private class DialogBoxHookup
    {
        protected GameObject m_diaglogBox;
        protected WindowManager m_windowManager;

        public DialogBoxHookup(WindowManager winMgr, List<string> data)
        {
            m_windowManager = winMgr;
            m_diaglogBox = winMgr.PushWindow("DialogBox", data);
            m_diaglogBox.GetComponent<DialogBox>().OnButton1 += ButtonOne;
            m_diaglogBox.GetComponent<DialogBox>().OnButton2 += ButtonTwo;
        }

        public virtual void ButtonOne(object data)
        {
            m_windowManager.PopWindow();
        }

        public virtual void ButtonTwo(object data)
        {
            m_windowManager.PopWindow();
        }
    }

    private class Welcome : DialogBoxHookup
    {
        static string title = "Welcome to the Game";
        static string content = "Welcome to the Car Game! We are <material=2>texturally</material> amused";
        static string okay = "okay";
        static List<string> data = new List<string> { title, content, okay, "" };

        public Welcome(WindowManager winMgr)
            : base(winMgr, data)
        {
        }
    }

    private class YouWin : DialogBoxHookup
    {
        static string title = "Level Complete";
        static string content = "Well done.";
        static string okay = "okay";
        static List<string> data = new List<string> { title, content, okay, "" };

        public YouWin(WindowManager winMgr)
            : base(winMgr, data)
        {
        }
    }

    private class YouLost : DialogBoxHookup
    {
        static string title = "Level Failed";
        static string content = "Sorry. Try again.";
        static string okay = "okay";
        static List<string> data = new List<string> { title, content, okay, "" };

        public YouLost(WindowManager winMgr)
            : base(winMgr, data)
        {
        }
    }

	// Use this for initialization
	void Start () 
    {
        new Welcome(this.windowManager);
        trafficLogic.OnStateChange += new System.Action<TrafficLogic.LogicState>(trafficLogic_OnStateChange);
	}

    void trafficLogic_OnStateChange(TrafficLogic.LogicState obj)
    {
        if (obj == TrafficLogic.LogicState.Succeeded)
        {
            StartCoroutine(WaitAndShow(2, () => new YouWin(windowManager)));
        }
        else if (obj == TrafficLogic.LogicState.Failed)
        {
            StartCoroutine(WaitAndShow(2, () => new YouLost(windowManager)));
        }
    }

    IEnumerator WaitAndShow(int seconds, Func<DialogBoxHookup> create)
    {
        yield return new WaitForSeconds(seconds);
        create.Invoke();
    }
	
	// Update is called once per frame
	void Update () 
    {
	}
}
