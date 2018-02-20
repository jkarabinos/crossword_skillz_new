using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DotDotDot : MonoBehaviour {

    [SerializeField]
    float waitTime = .75f;

    private string baseText = "SEARCHING";

    float lastTime;

    int numDots = 0;

	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {

        GetComponent<Text>().text = baseText;
        lastTime = Time.time;
    }

    // Update is called once per frame
    void Update () {
        if (Time.time - lastTime > waitTime)
        {
            lastTime = Time.time;
            numDots = (numDots + 1) % 4;

            GetComponent<Text>().text = baseText;
            for (int i = 0; i < numDots; i++)
            {
                GetComponent<Text>().text += ".";
            }
        }
	}
}
