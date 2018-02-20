using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardInput : MonoBehaviour {

    [SerializeField]
    GameplayLogic gameLogic;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DidPressKey(Text keyText)
    {
        //Debug.Log("the user pressed the key " + keyText.text);
        if(gameLogic.canInteract)
            gameLogic.InputLetter(keyText.text, true);
        
    }

    public void ClearLastInput()
    {
        if (gameLogic.canInteract)
            gameLogic.ClearLastLetter();
    }

    public void ClearAllInput()
    {
        if (gameLogic.canInteract)
            gameLogic.ClearAllInput();
    }
}
