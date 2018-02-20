using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveGame : MonoBehaviour {

    [SerializeField]
    GameObject leaveGameObj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenLeaveGameOptions()
    {
        //leaveGameObj.SetActive(true);
        FadeObject fade = leaveGameObj.GetComponent<FadeObject>();
        if (fade != null) fade.FadeIn();
    }


    public void CancelLeaveGame()
    {
        //leaveGameObj.SetActive(false);
        FadeObject fade = leaveGameObj.GetComponent<FadeObject>();
        if (fade != null) fade.FadeOut();
    }

   
}
