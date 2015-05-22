using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour 
{
    public GameObject logic;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        logic.SendMessage("FinishLineTriggered", this);
    }
}
