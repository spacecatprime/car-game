using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class DialogBox : MonoBehaviour, UIWindowBase
{
    public GameObject title;
    public GameObject content;
    public GameObject button1;
    public GameObject button2;

    public event Action<object> OnButton1;
    public event Action<object> OnButton2;
    public event Action<object> OnShow;
    public event Action<object> OnHide;

	// Use this for initialization
	void Start () 
    {	
	}
	
	// Update is called once per frame
	void Update () 
    {	
	}

    void SetupContent(string title, string content, string button1, string button2)
    {
        this.title.GetComponent<Text>().text = title;
        this.content.GetComponent<Text>().text = content;

        if (string.IsNullOrEmpty(button1))
        {
            GameObject.Destroy(this.button1);
        }
        else
        {
            this.button1.GetComponentInChildren<Text>().text = button1;
            this.button1.GetComponent<Button>().onClick.AddListener(OnClickButton1);
        }

        if (string.IsNullOrEmpty(button2))
        {
            GameObject.Destroy(this.button2);
        }
        else
        {
            this.button2.GetComponentInChildren<Text>().text = button2;
            this.button2.GetComponent<Button>().onClick.AddListener(OnClickButton2);
        }
    }

    void OnClickButton1()
    {
        if (OnButton1 != null)
        {
            OnButton1(this);
        }
    }

    void OnClickButton2()
    {
        if (OnButton2 != null)
        {
            OnButton2(this);
        }
    }

    #region UIWindowBase Members

    public void SetupData(object data)
    {
        List<string> input = data as List<string>;
        SetupContent(input[0], input[1], input[2], input[3]);
    }

    public void Conceal()
    {
        if (OnHide != null)
        {
            OnHide(this);
        }
        Renderer rend = this.gameObject.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.enabled = false;
        }
    }

    public void Reveal()
    {
        if (OnShow != null)
        {
            OnShow(this);
        }
        Renderer rend = this.gameObject.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.enabled = true;
        }
    }

    #endregion
}
