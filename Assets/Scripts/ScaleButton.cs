using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScaleButton : MonoBehaviour {

    [SerializeField]
    Text buttonText;

    Vector2 originalSizeDelta;
    int originalTextSize;

    [SerializeField]
    float scaleDuration = .25f;

    [SerializeField]
    float waitTime = 2.0f;

    [SerializeField]
    float maxScale = 1.3f;


    float startTime;
   

    private void Start()
    {
        
        startTime = Time.time;
    }

    private void Awake()
    {
        originalSizeDelta = GetComponent<RectTransform>().sizeDelta;
        originalTextSize = buttonText.fontSize;
    }

    // Update is called once per frame
    void Update () {

        if (true)
        {
            int i = (int)((Time.time - startTime) / (waitTime + scaleDuration));

            float p = (Time.time - startTime) - (waitTime + scaleDuration) * i;

            float t = (p - waitTime) / scaleDuration;

           
            float s = Mathf.SmoothStep(1, maxScale, t * 2);
            if (t > .5f)
                s = Mathf.SmoothStep(maxScale, 1, (t - .5f) * 2);
            Scale(s);
        }
        else
        {
            Scale(1);
        }
	}

    public void ResetStartTime()
    {
        startTime = Time.time;
    }

    void Scale(float scale)
    {
        buttonText.fontSize = (int) (scale * originalTextSize);
        GetComponent<RectTransform>().sizeDelta = scale * originalSizeDelta;
    }
}
