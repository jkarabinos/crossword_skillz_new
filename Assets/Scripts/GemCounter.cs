using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemCounter : MonoBehaviour {

    [SerializeField]
    Text gemCountText;

    [SerializeField]
    Text boxCostText;

	// Use this for initialization
	void Start () {
        RefreshGemCount();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RefreshGemCount()
    {
        gemCountText.text = PlayerPrefs.GetInt(GemManager.GEMS) + " / " + GemManager.CostOfLootBox();
        boxCostText.text = GemManager.CostOfLootBox().ToString();
    }
}
