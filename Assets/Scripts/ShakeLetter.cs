using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShakeLetter : MonoBehaviour {

    [SerializeField]
    private int numShakes = 1;

    [SerializeField]
    private float percentMoveDist = .15f; // the percent of the tile that the letter will move

    [SerializeField]
    private float maxWaitTime = .05f;

    private float startTime;
    private float duration;
    private float moveDist;

    Text letterText;
    Vector2 initPos;

    bool isShaking = false;
    Vector2 finalPos1;
    Vector2 finalPos2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!isShaking)
            return;

        float p = (Time.time - startTime) / duration;

        if(p < 1)
        {
            MoveLetter(p);
        }else
        {
            isShaking = false;
            letterText.transform.localPosition = initPos;
        }
	}

    void MoveLetter(float p)
    {
        int i = (int)(p * numShakes);
        float t = (p - ((i * 1.0f) / numShakes))*numShakes;

        if(i == 0)
            letterText.transform.localPosition = Vector2.Lerp(initPos, finalPos1, t);

        else if(i % 2 == 0)
            letterText.transform.localPosition = Vector2.Lerp(finalPos2, finalPos1, t);
        else
            letterText.transform.localPosition = Vector2.Lerp(finalPos1, finalPos2, t);


    }


    public void ShakeTheLetter(float _totalTime, Text _letterText, bool _isAcross) // jiggle the letter from side to side to show that it is incorrect
    {
        duration = _totalTime;
        letterText = _letterText;
        initPos = letterText.transform.localPosition;
        moveDist = percentMoveDist * GetComponent<RectTransform>().rect.size.x;

        /*if (_isAcross)
        {
            finalPos1 = new Vector2(initPos.x, initPos.y + moveDist);
            finalPos2 = new Vector2(initPos.x, initPos.y - moveDist);
        }
        else
        {
        }*/
            finalPos1 = new Vector2(initPos.x + moveDist, initPos.y);
            finalPos2 = new Vector2(initPos.x - moveDist, initPos.y);

        StartCoroutine(StartShakeCoroutine());
        
    }


    IEnumerator StartShakeCoroutine()
    {
        float r = Random.Range(0, maxWaitTime);

        yield return new WaitForSeconds(r);

        startTime = Time.time;
        isShaking = true;
    }
}
