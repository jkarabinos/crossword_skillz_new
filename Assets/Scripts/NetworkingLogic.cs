using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CreateBoard))]
[RequireComponent(typeof(GameplayLogic))]

public class NetworkingLogic : MonoBehaviour {

    [SerializeField]
    int maxGridSize = 10;

    [SerializeField]
    GameOver gameOver;


    CreateBoard createBoard;
    GameplayLogic gameplayLogic;


	// Use this for initialization
	void Start () {
        
            string fileName = "CrosswordData/Size_8_" + Random.Range(0, 5) + "/crosswords_" + UnityEngine.Random.Range(0, 10);
        


            TextAsset ta = (TextAsset)Resources.Load(fileName);
            string[] crosswords = ta.text.Split('\n');
            int randomIndex = Random.Range(0, crosswords.Length);
            //int randomIndex = 0;


           

            int[] scoreMults = CreateScoreMults() ;
           StartGame( fileName, scoreMults, randomIndex);
      
        
	}



    void SetComponents()
    {
        createBoard = GetComponent<CreateBoard>();
        gameplayLogic = GetComponent<GameplayLogic>();
    }
	
	
    public void StartGame(string fileName, int[] scoreMults, int randomIndex)
    {
        SetComponents();
        createBoard.LoadCrossword(fileName, scoreMults, randomIndex);
    }


    public void UpdateBoard(string[] friendlyGrid, string[] enemyGrid, string [] neutralGrid)
    {
        gameplayLogic.SetGrids(friendlyGrid, enemyGrid, neutralGrid);
        
    }



    public void EndGame(int _friendlyScore, int _enemyScore)
    {
        gameOver.EndGame(_friendlyScore, _enemyScore, false);
    }


    int[] CreateScoreMults()
    {
        int[] scoreMults = new int[maxGridSize * maxGridSize];
        for (int i = 0; i < scoreMults.Length; i++)
        {
            int r = Random.Range(0, 100);
            int s = 1;
            if (r >= 99)
                s = 4;
            else if (r >= 96)
                s = 3;
            else if (r >= 91)
                s = 2;
            scoreMults[i] = s;
        }

        return scoreMults;
    }
}
