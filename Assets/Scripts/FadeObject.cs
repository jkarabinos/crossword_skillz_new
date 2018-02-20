using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeObject : MonoBehaviour {

    //A helpful little class to aid in smooth UI transitions. Add to a parent object to fade all children when called

    [SerializeField]
    private float fadeTime = .25f; //default fade time is .25f

    private float startTime;
    private bool isFading = false;

    private float alpha1;
    private float alpha2;

    public float GetFadeTime()
    {
        return fadeTime;
    }

    //Fade the object in
    public void FadeIn()
    {
        Fade(true);
    }

    //Fade the object out
    public void FadeOut()
    {
        Fade(false);
    }

    //Fade the object either in or out
    private void Fade(bool isFadeIn)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = isFadeIn;

        if (isFadeIn)
        {
            alpha1 = 0;
            alpha2 = 1;
        }
        else
        {
            alpha1 = 1;
            alpha2 = 0;
        }

        startTime = Time.time;
        isFading = true;
    }

	// Update is called once per frame
	void Update () {

        if (isFading)
        {
            float percentFade = (Time.time - startTime) / fadeTime; 

            if(percentFade > 1) // if our fade is complete
            {
                GetComponent<CanvasGroup>().alpha = alpha2;
                isFading = false;
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(alpha1, alpha2, percentFade); //Smoothly change the alpha from the first value to the second
            }
        }
	}
   
}
