using UnityEngine;
using System.Collections;

public class AudioFX : MonoBehaviour 
{
    public AudioClip sfxForward;
    public AudioClip sfxBonk;
    public AudioClip sfxTurn;
    public AudioSource sfxSource;

    public enum Sounds
    { 
        Forward,
        Bonk,
        Turn
    };

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void PlaySound(Sounds snd)
    {
        switch (snd)
        { 
            case Sounds.Forward:
                sfxSource.PlayOneShot(this.sfxForward);
                break;
            case Sounds.Bonk:
                sfxSource.PlayOneShot(this.sfxBonk);
                break;
            case Sounds.Turn:
                sfxSource.PlayOneShot(this.sfxTurn);
                break;
        }
    }
}
