using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// http://answers.unity3d.com/questions/13267/how-can-i-mimic-the-frame-selected-f-camera-move-z.html

public class ZoomToExtents : MonoBehaviour 
{
	public GameObject target;
	public Vector3 originalPos;
	public Quaternion originalRot;
	public float distanceFactor = 1f;

	Bounds CalculateBounds(GameObject go) 
	{
		Bounds b = new Bounds(go.transform.position, Vector3.zero);
		Object[] rList = go.GetComponentsInChildren(typeof(Renderer));
		foreach (Renderer r in rList) 
		{
			b.Encapsulate(r.bounds);
		}
		return b;
	}
	
	public void FocusCameraOnGameObject(Camera c, GameObject go) 
	{
		Bounds b = CalculateBounds(go);
		Vector3 max = b.size;
		// Get the radius of a sphere circumscribing the bounds
		float radius = max.magnitude / 2f;
		// Get the horizontal FOV, since it may be the limiting of the two FOVs to properly encapsulate the objects
		float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(c.fieldOfView * Mathf.Deg2Rad / 2f) * c.aspect) * Mathf.Rad2Deg;
		// Use the smaller FOV as it limits what would get cut off by the frustum        
		float fov = Mathf.Min(c.fieldOfView, horizontalFOV);
		float dist = radius /  (Mathf.Sin(fov * Mathf.Deg2Rad / 2f));

		float distZ = distanceFactor * dist;
		c.transform.localPosition = new Vector3(b.center.x, b.center.y + distZ, b.center.z);
		if (c.orthographic)
		{
			c.orthographicSize = radius;
		}

		// Frame the object hierarchy
		c.transform.LookAt(b.center);
	}

	void Start()
	{
		originalPos = Camera.main.transform.localPosition;
		originalRot = Camera.main.transform.localRotation;
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.X)) 
		{
			Camera c = Camera.main;
			Vector3 mp = Input.mousePosition;
			Ray ray = c.ScreenPointToRay(mp);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("car")))
			{
				target = hit.transform.gameObject;
				FocusCameraOnGameObject(Camera.main, hit.transform.gameObject);
			}
		}
		else if(Input.GetKey(KeyCode.F1))
		{
			Camera.main.transform.localPosition = this.originalPos;
			Camera.main.transform.localRotation = this.originalRot;
		}
		else if(Input.GetKey(KeyCode.Escape))
		{
			FocusCameraOnGameObject(Camera.main, this.target);
		}
	}
}
