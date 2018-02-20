using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileTutorial : MonoBehaviour {


    public static string HAS_FINISHED_TUT = "has_finished_tut";
    public static string IS_FIRST_TIME_LOADING = "is_first_time";

	// Use this for initialization
	void Start () {
        //fade in the profile tutorial if it not the user's first time here and they have not clicked the profile button ever

        //PlayerPrefs.SetInt(HAS_FINISHED_TUT, 0);

        
        if (!PlayerPrefs.HasKey(IS_FIRST_TIME_LOADING))
        {
            PlayerPrefs.SetInt(IS_FIRST_TIME_LOADING, 1);
            PlayerPrefs.SetInt(HAS_FINISHED_TUT, 0);
            return;
        }

        if(PlayerPrefs.GetInt(HAS_FINISHED_TUT) == 0)
        {
            //if the player has not clicked the profile button
            GetComponent<FadeObject>().FadeIn();

        }
        

        
	}

   
	
	public void DidPressProfileButton() 
    {
        PlayerPrefs.SetInt(HAS_FINISHED_TUT, 1);
        
    }
}
