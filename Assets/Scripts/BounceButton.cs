using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceButton : MonoBehaviour {

    [SerializeField]
    float maxRotation = 30;

    [SerializeField]
    float rotDuration =.15f;

    float startTime;

    bool isBouncing = false;

    public void SetBouncing(bool bouncing)
    {
        isBouncing = bouncing;
    }

	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (isBouncing)
        {

            float p = (Time.time - startTime) / rotDuration;
            float i = (int)p;
            float t = p - i;

            float z = Mathf.SmoothStep(maxRotation, -maxRotation, t);
            if(i % 2 == 0)
                z = Mathf.SmoothStep(-maxRotation, maxRotation, t);

            this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, z));
        }
        else
        {
            this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

	}
}
