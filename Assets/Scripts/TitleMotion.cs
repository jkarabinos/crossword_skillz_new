using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMotion : MonoBehaviour {


    [SerializeField]
    GameObject title1;

    [SerializeField]
    GameObject title2;

    [SerializeField]
    Transform offScreenRight;

    [SerializeField]
    Transform offScreenLeft;

    Vector2 initPos1;
    Vector2 initPos2;

    Vector2 endPos1;
    Vector2 endPos2;

    Vector2 startMovPos1;
    Vector2 startMovPos2;

    float startTime;

    bool isAnimating = false;
    bool isGoingOff = false;

    float duration;

	// Use this for initialization
	void Start () {
        startTime = Time.time;

        initPos1 = title1.transform.position;
        initPos2 = title2.transform.position;

        endPos1 = new Vector2(0, title1.transform.localPosition.y);
        endPos2 = new Vector2(0, title2.transform.localPosition.y);
	}
	
	// Update is called once per frame
	void Update () {

        if (isAnimating)
        {
            Vector2 finalPos1 = offScreenLeft.position;
            Vector2 finalPos2 = offScreenRight.position;

            if (!isGoingOff)
            {
                finalPos1 = initPos1;
                finalPos2 = initPos2;
            }

            float t = (Time.time - startTime) / duration;

            if(t < 1)
            {
                title1.transform.position = Vector2.Lerp(startMovPos1, finalPos1, t);
                title2.transform.position = Vector2.Lerp(startMovPos2, finalPos2, t);
            }
            else
            {
                if (!isGoingOff)
                {
                    title1.transform.position = finalPos1;
                    title2.transform.position = finalPos2;
                    startTime = Time.time;
                    isAnimating = false;

                }
            }
        }
        else
        {
            float s = SpeedForTime(Time.time - startTime);

            title1.transform.localPosition = Vector2.MoveTowards(title1.transform.localPosition, endPos1, Time.deltaTime * s);
            title2.transform.localPosition = Vector2.MoveTowards(title2.transform.localPosition, endPos2, Time.deltaTime * s);
        }


       
        

    }

    float SpeedForTime(float t)
    {
        return Mathf.Max(Mathf.Pow(.75f, t) * 10, 1);
    }


    public void SendTitleOffScreen(float _duration)
    {
        duration = _duration;

        startMovPos1 = title1.transform.position;
        startMovPos2 = title2.transform.position;

        startTime = Time.time;

        isGoingOff = true;
        isAnimating = true;
    }

    public void SendTitleOnScreen(float _duration)
    {
        duration = _duration;

        startMovPos1 = title1.transform.position;
        startMovPos2 = title2.transform.position;

        startTime = Time.time;

        isGoingOff = false;
        isAnimating = true;
    }
}
