using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour 
{
    void OnTriggerEnter(Collider other)
    {
        gameObject.SendMessageUpwards("FinishLineTriggered");
    }
}
