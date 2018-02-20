using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatManager : MonoBehaviour {

    public static string WINS = "wins";
    public static string LOSSES = "losses";
    public static string BEST_WORD = "best_word";
    public static string CLUES_SOLVED = "clues_solved";

    [SerializeField]
    Text winText;

    [SerializeField]
    Text lossText;

    [SerializeField]
    Text winPercentageText;

    [SerializeField]
    Text bestWordText;

    [SerializeField]
    Text cluesSolvedText;


	// Use this for initialization
	void Start () {

        //Debug.Log("set the player rank to the max rank");
        //PlayerPrefs.SetInt(GameManager.RANK, 25 * (GameManager.numStars + 1)-1);
        //PlayerPrefs.SetInt(GameManager.RANK, 0);

        if (!PlayerPrefs.HasKey(WINS)) PlayerPrefs.SetInt(WINS, 0);
        if (!PlayerPrefs.HasKey(LOSSES)) PlayerPrefs.SetInt(LOSSES, 0);
        if (!PlayerPrefs.HasKey(BEST_WORD)) PlayerPrefs.SetInt(BEST_WORD, 0);
        if (!PlayerPrefs.HasKey(CLUES_SOLVED)) PlayerPrefs.SetInt(CLUES_SOLVED, 0);


        SetStatValues();
    }



    void SetStatValues()
    {
        winText.text = PlayerPrefs.GetInt(WINS).ToString();
        lossText.text = PlayerPrefs.GetInt(LOSSES).ToString();
        bestWordText.text = PlayerPrefs.GetInt(BEST_WORD).ToString();
        cluesSolvedText.text = PlayerPrefs.GetInt(CLUES_SOLVED).ToString();

        int wins = PlayerPrefs.GetInt(WINS);
        int losses = PlayerPrefs.GetInt(LOSSES);

        if(losses == 0)
        {
            if (wins == 0)
                winPercentageText.text = "0.00";
            else
                winPercentageText.text = "100.00";
        }
        else
        {
            float winP = ((wins * 1.0f) / (losses + wins)) * 100;
            winPercentageText.text = winP.ToString("F2");
        }
        
    }


}
