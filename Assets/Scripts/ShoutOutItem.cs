using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoutOutItem : MonoBehaviour {

    [SerializeField]
    Text shoutOutText;

    [SerializeField]
    Sprite unlockedSprite;

    [SerializeField]
    Sprite lockedSprite;

    [SerializeField]
    Image backgroundImage;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetShoutOut(string text)
    {

        shoutOutText.text = text.ToUpper();

        if (text == "")
            backgroundImage.sprite = lockedSprite;
        else
            backgroundImage.sprite = unlockedSprite;

        
    }

    public void SetTextBorders(float mult)
    {
        float indent = mult * shoutOutText.GetComponent<RectTransform>().offsetMin.x;

        shoutOutText.GetComponent<RectTransform>().offsetMin = new Vector2(indent, indent);
        shoutOutText.GetComponent<RectTransform>().offsetMax = new Vector2(-indent, -indent);
    }
}
