using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class GemRow : MonoBehaviour {

    [SerializeField]
    Text opponentText;

    [SerializeField]
    Text friendlyText;

    [SerializeField]
    GameObject frienlyGemObj;

    [SerializeField]
    GameObject enemyGemObj;

    [SerializeField]
    float fadeDuration = .15f;

    bool isFading = false;
    float startTime;

	// Use this for initialization
	void Start () {
        GetComponent<CanvasGroup>().alpha = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (isFading)
        {
            GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(0, 1, (Time.time - startTime) / fadeDuration);
        }
	}

    public void Setup(int friendlyNumber, int enemyNumber, int rewardGems, int minGems)
    {
       

        if(friendlyNumber > enemyNumber)
        {
            friendlyText.text = "+" + rewardGems;
            opponentText.text = "+" + minGems;

            if(minGems == 0) {
                enemyGemObj.SetActive(false);
                opponentText.gameObject.SetActive(false);
            }
        }
        else if (enemyNumber > friendlyNumber)
        {
            opponentText.text = "+" + rewardGems;
            friendlyText.text = "+" + minGems;

            if(minGems == 0)
            {
                friendlyText.gameObject.SetActive(false);
                frienlyGemObj.SetActive(false);
            }
        }
        else
        {
            int n = (rewardGems + minGems) / 2;
            
            opponentText.text = "+" + n;
            friendlyText.text = "+" + n;
        }

       

        startTime = Time.time;

        isFading = true;
    }
}
