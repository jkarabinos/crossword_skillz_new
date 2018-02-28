using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeLogic : MonoBehaviour {
    [SerializeField]
    string tripeakAppLink;

    public static bool isUsingSkillz = true;

    public void PlayGame()
    {
        //isUsingSkillz = false;

        if (isUsingSkillz)
        {
            
            SkillzCrossPlatform.LaunchSkillz();
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }


    }

    public void LinkToTripeaks()
    {
        LinkToApp(tripeakAppLink);
    }

    void LinkToApp(string appURL)
    {
        Application.OpenURL(appURL);
    }

}
