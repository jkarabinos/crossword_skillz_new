using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLogic : MonoBehaviour {

    [SerializeField]
    GameObject newPointsPrefab;

    [SerializeField]
    Transform gameBoard;

    [SerializeField]
    Transform myScore;

    [SerializeField]
    Transform enemyScore;

    [SerializeField]
    Color friendlyColor;

    [SerializeField]
    Color enemyColor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddScore(int _points, bool _isFriendly, Text _scoreText, int _totalScore)
    {
        if (_points <= 0)
            return;

        Vector3 pos = myScore.position;
        Color col = friendlyColor;
        string playerName = "ME";
        
        if (!_isFriendly)
        {
            pos = enemyScore.position;
            col = enemyColor;
            playerName = "YOU";
        }

        GameObject points = Instantiate(newPointsPrefab, gameBoard, false);
        points.GetComponent<Points>().Setup(_points, pos, col, _scoreText, _totalScore, playerName);
        Destroy(points, 1.25f);
    }

    
}
