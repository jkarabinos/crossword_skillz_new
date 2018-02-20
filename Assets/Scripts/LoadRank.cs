using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadRank : MonoBehaviour {


    [SerializeField]
    Text rankText; //temp for displaying the player's current rank

    [SerializeField]
    PlayerRank playerRank;


    public delegate void ChangeBorderDelegate();
    public ChangeBorderDelegate changeBorderDelegate;

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start () {

        if (!PlayerPrefs.HasKey(GameManager.RANK))
        {
            PlayerPrefs.SetInt(GameManager.RANK, 0);
        }

        if (!PlayerPrefs.HasKey(PlayerRank.RANK_BORDER))
        {
            PlayerPrefs.SetInt(PlayerRank.RANK_BORDER, 1);
        }

        if (!PlayerPrefs.HasKey(AddSelectableItems.SHOUT_OUTS))
        {
            PlayerPrefs.SetInt(AddSelectableItems.SHOUT_OUTS, 1);
        }

        //PlayerPrefs.SetInt(PlayerRank.RANK_BORDER, 4);
        changeBorderDelegate += ChangeBorder;

        changeBorderDelegate.Invoke();
        //StartCoroutine(InvokeDelegateWithDelay(1));
        DisplayRank(PlayerPrefs.GetInt(GameManager.RANK));

    }
	
    IEnumerator InvokeDelegateWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        changeBorderDelegate.Invoke();
    }

	// Update is called once per frame
	void Update () {
		
	}

    void ChangeBorder()
    {
        Debug.Log("change the border");
    }

    void DisplayRank(int rank)
    {
        rankText.text = "RANK: " + rank.ToString();
        playerRank.UpdateRank(rank, true, PlayerPrefs.GetInt(PlayerRank.RANK_BORDER));
        //playerRank.ScaleRank(.8f);
    }
}
