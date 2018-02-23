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

        string fileName = "";

        if (HomeLogic.isUsingSkillz)
        {
            int r1 = SkillzCrossPlatform.Random.Range(0, 5);
            int r2 = SkillzCrossPlatform.Random.Range(0, 10);

            fileName = "CrosswordData/Size_8_" + r1 + "/crosswords_" + r2;
        }
        else
        {
            fileName = "CrosswordData/Size_8_" + Random.Range(0, 5) + "/crosswords_" + UnityEngine.Random.Range(0, 10);
        }

        


        TextAsset ta = (TextAsset)Resources.Load(fileName);
        string[] crosswords = ta.text.Split('\n');
        int randomIndex = 0;

        if (HomeLogic.isUsingSkillz)
            randomIndex = SkillzCrossPlatform.Random.Range(0, crosswords.Length);
        else
            randomIndex = Random.Range(0, crosswords.Length);
           
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
            int r = 0;

            if (HomeLogic.isUsingSkillz)
                r = SkillzCrossPlatform.Random.Range(0, 100);
            else
                r = Random.Range(0, 100);

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
