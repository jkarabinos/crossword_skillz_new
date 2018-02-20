using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuAudio : MonoBehaviour {

    public AudioClip menuSound1;
    public AudioClip menuSound2;

    public AudioClip itemSelectSound;

    public AudioClip boopSound;
    public AudioClip openBoxSound;

    public void PlayOneShot(AudioClip clip, float volume)
    {
        SoundManager.PlayOneShot(GetComponent<AudioSource>(), clip, volume);
    }

    public void PlayOneShot(AudioClip clip, float volume, float pitch)
    {
        GetComponent<AudioSource>().pitch = pitch;
        PlayOneShot(clip, volume);
    }
}
