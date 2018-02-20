using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonBorder : MonoBehaviour {

    [SerializeField]
    string initialPath = "Borders/Borders_Long/rank_border_long_";

    [SerializeField]
    string pathExtension = "";

    [SerializeField]
    LoadRank loadRank;

    private void Awake()
    {
        if (loadRank != null)
        {
            loadRank.changeBorderDelegate += UpdateImageBackground;
        }
    }

    // Use this for initialization
    void Start()
    {
        UpdateImageBackground();
        

    }
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateImageBackground()
    {
        string filePath = initialPath + PlayerPrefs.GetInt(PlayerRank.RANK_BORDER) + pathExtension;
        GetComponent<Image>().sprite = Resources.Load<Sprite>(filePath);
	
    }

    
}
