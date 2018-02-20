using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPlayers : MonoBehaviour {

    [SerializeField]
    GameObject playerRankPrefab;

    [SerializeField]
    GameObject playerRankParent1;

    [SerializeField]
    GameObject playerRankParent2;

	// Use this for initialization
	void Start () {

        //AddPlayerRank(playerRankParent1, 6);
        //AddPlayerRank(playerRankParent2, 13);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowLocalPlayer(int rank, int rankBorder)
    {
        AddPlayerRank(playerRankParent1, rank, rankBorder);
    }

    public void ShowEnemyPlayer(int rank, int rankBorder)
    {
        AddPlayerRank(playerRankParent2, rank, rankBorder);
    }

    void AddPlayerRank(GameObject parent, int rank, int rankBorder)
    {
        GameObject playerRank = Instantiate(playerRankPrefab, parent.transform, false);
        playerRank.transform.localPosition = new Vector2(0, 0);
        PlayerRank pr = playerRank.GetComponent<PlayerRank>();
        pr.UpdateRank(rank, false, rankBorder);

        pr.ScaleRank(parent.GetComponent<RectTransform>().sizeDelta.x / playerRank.GetComponent<RectTransform>().sizeDelta.x);

    }
}
