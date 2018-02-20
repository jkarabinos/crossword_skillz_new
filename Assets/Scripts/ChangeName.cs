using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeName : MonoBehaviour {

    public static string USERNAME = "username";

    [SerializeField]
    InputField inputField;

    [SerializeField]
    int maxNameLength = 10;

    [SerializeField]
    Text inputFieldText;

	// Use this for initialization
	void Start () {
        if (!PlayerPrefs.HasKey(USERNAME))
        {
            PlayerPrefs.SetString(USERNAME, "GUEST");
        }

        string playerName = PlayerPrefs.GetString(USERNAME);

        inputField.characterLimit = maxNameLength;


        inputField.text = playerName;

	}

    public void OnTextValueChanged()
    {
        inputField.text = inputField.text.ToUpper();
    }
	
    public void ChangeUsername()
    {
        string newName = inputField.text;
        if (NameIsLegal(newName))
        {
            PlayerPrefs.SetString(USERNAME, newName);
        }
        else
        {
            inputField.text = PlayerPrefs.GetString(USERNAME);
        }
    }


	bool NameIsLegal(string newName)
    {
        if(newName.Length > 2)
            return true;

        return false;
    }
}
