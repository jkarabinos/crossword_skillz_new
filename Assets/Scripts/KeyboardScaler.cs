using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardScaler : MonoBehaviour {

    [SerializeField]
    GameObject[] keyboardRows;

    float mult;

	// Use this for initialization
	void Start () {

        SetMult(keyboardRows[0]);

		foreach(GameObject keyboardRow in keyboardRows)
        {
            ScaleRow(keyboardRow);
        }
	}
	

    void SetMult(GameObject keyboardRow)
    {
        float totalWidthInButtons = 0;
        int numButtons = 0;

        float targetWidth = keyboardRow.GetComponent<RectTransform>().rect.width;

        foreach (Transform child in keyboardRow.transform)
        {

            numButtons++;

            totalWidthInButtons += child.GetComponent<LayoutElement>().preferredWidth;
        }


        float currentWidth = (numButtons - 1) * keyboardRow.GetComponent<HorizontalLayoutGroup>().spacing + totalWidthInButtons;

        mult = targetWidth / currentWidth;

        Debug.Log("the target width is " + targetWidth);
        Debug.Log("the current width is " + currentWidth);


        Debug.Log("the mult is " + mult);
    }
	

    void ScaleRow(GameObject keyboardRow)
    {

        //keyboardRow.GetComponent<HorizontalLayoutGroup>().spacing = keyboardRow.GetComponent<HorizontalLayoutGroup>().spacing * mult;


        foreach (Transform child in keyboardRow.transform)
        {
            child.GetComponent<LayoutElement>().preferredWidth = child.GetComponent<LayoutElement>().preferredWidth * mult;
            
        }

    }
}
