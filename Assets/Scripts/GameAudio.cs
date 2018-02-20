using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameAudio : MonoBehaviour {


    
    public AudioClip solvedClueAudio;

    public AudioClip enemySolvedClueAudio;

    public AudioClip failedSolveClueAudio;


    public AudioClip whoosh1;
    public AudioClip whoosh2;

    public AudioClip gemSound;

    public AudioClip windSound;

    public AudioClip starUpSound;

    public AudioClip rankUpSound;
    public AudioClip rankDownSound;

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
