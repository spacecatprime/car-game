using UnityEngine;
using System.Collections;

public class CarSetup : MonoBehaviour 
{
	public float rotateDegrees = 0;

	bool moved = false;

	// Use this for initialization
	void Start () 
	{
		//transform.Rotate(transform.up, rotateDegrees);
	}

	// Update is called once per frame
	void Update () 
	{	
		if(!moved)
		{
			float distance = 1f;
			float speed = 000.1f;
			//transform.position = origin + (transform.forward*(Mathf.PingPong(Time.time*distance*speed + (distance*speed/2), distance*2)-distance));
			//transform.position = transform.position + (transform.forward*(Mathf.PingPong(Time.time*distance*speed + (distance*speed/2), distance*2)-distance));
			transform.position = transform.position + (Vector3.forward * 10.0f);
			moved = true;
		}
	}
}
