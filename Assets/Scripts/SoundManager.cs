using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {


    public static string SOUND_ON = "sound_on";


    // Use this for initialization
    void Start () {
        

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void PlayOneShot(AudioSource audioSource, AudioClip clip, float volume)
    {

        if(PlayerPrefs.GetInt(SOUND_ON) != 0)
            audioSource.PlayOneShot(clip, volume);
    }
}
