using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObject : MonoBehaviour {

    [SerializeField]
    float scaleDuration = .25f;

    [SerializeField]
    float maxScale = 1.3f;


    bool isScaling = false;

    float startTime;

    Vector2 largeScale;
    Vector2 smallScale;

	// Use this for initialization
	void Start () {
        smallScale = new Vector2(1, 1);
        largeScale = new Vector2(maxScale, maxScale);
	}
	
	// Update is called once per frame
	void Update () {
        if (isScaling)
        {

            float t = (Time.time - startTime) / scaleDuration;
            if (t < .5f)
                this.transform.localScale = Vector2.Lerp(smallScale, largeScale, t * 2);
            else
                this.transform.localScale = Vector2.Lerp(largeScale, smallScale, (t - .5f) * 2);

            if(t > 1)
            {
                this.transform.localScale = smallScale;
                isScaling = false;
            }

        }
	}

    public void ScaleObj()
    {
        startTime = Time.time;
        isScaling = true;
    }
}
