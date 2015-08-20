using UnityEngine;
using System.Collections;

public class ParticleFX : MonoBehaviour 
{
    public GameObject explosionPrefab;

    public enum Effects
    { 
        Explosion
    }

	void Start () 
    {	
	}

	void Update () 
    {	
	}

    public void Run(Effects eff, Vector3 position)
    {
        switch (eff)
        { 
            case Effects.Explosion:
                GameObject.Instantiate(explosionPrefab, position, Quaternion.identity);
                break;
        }
    }
}
