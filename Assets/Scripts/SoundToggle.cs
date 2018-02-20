using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour {


    [SerializeField]
    Sprite soundOffImage;

    [SerializeField]
    Sprite soundOnImage;

    [SerializeField]
    Image soundImage;
    

	// Use this for initialization
	void Start () {

        if (!PlayerPrefs.HasKey(SoundManager.SOUND_ON))
            PlayerPrefs.SetInt(SoundManager.SOUND_ON, 1);

        SetSoundIcon();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetSoundIcon()
    {
        int soundIsOn = PlayerPrefs.GetInt(SoundManager.SOUND_ON);

        
        if (soundIsOn == 0)
            soundImage.sprite = soundOffImage;
        else
            soundImage.sprite = soundOnImage;
    }

    public void ToggleSound()
    {
        int soundIsOn = PlayerPrefs.GetInt(SoundManager.SOUND_ON);

        soundIsOn = (soundIsOn + 1) % 2;

        PlayerPrefs.SetInt(SoundManager.SOUND_ON, soundIsOn);

        SetSoundIcon();
    }
}
