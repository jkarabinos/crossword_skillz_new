using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeLogic : MonoBehaviour {

    public static bool isUsingSkillz = true;

    public void PlayGame()
    {

        if (isUsingSkillz)
        {
            SkillzCrossPlatform.LaunchSkillz();
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }


    }

}
