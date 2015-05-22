using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MoveType : MonoBehaviour
{
    public enum MoveKind
    { 
        None,
        Forward,
        TurnLeft,
        TurnRight,
        UTurn
    }

    public MoveKind kind;
    public TrafficLogic.Operation operation;

	void Start () 
	{
	}

    void Update()
    {
    }
}
