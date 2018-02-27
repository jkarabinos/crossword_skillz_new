using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticSubmission : MonoBehaviour {

    [SerializeField]
    GameObject coverPanel;

    [SerializeField]
    float submissionTime = 4.0f;

    [SerializeField]
    float delayTime = 2.0f;

    [SerializeField]
    GameOver gameOver;

    bool isSubmitting = false;
    float startTime;

    Vector2 originalSize;

	// Use this for initialization
	void Start () {
        originalSize = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        if (isSubmitting)
        {
            float t = (Time.time - startTime) / submissionTime;
            float s = Mathf.SmoothStep(originalSize.x, 0, t);
            coverPanel.transform.localScale = new Vector2(s, originalSize.y);

            if (t > 1)
            {
                gameOver.NextButton();
                isSubmitting = false;
            }
                //GameManager.instance.LeaveGame();
        }
	}

    public void SetSubmissionTimer()
    {
        Debug.Log("start the auto submission");
        isSubmitting = true;
        startTime = Time.time + delayTime;
    }

    public void ResetSubmissionTimer()
    {
        if (!isSubmitting)
            return;

        startTime = Time.time + delayTime;
    }
}
