using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(GameplayLogic))]
public class GameManager : MonoBehaviour {

    [SerializeField]
    Color friendlyColor;

    [SerializeField]
    Color enemyColor;

    [SerializeField]
    Color drawColor;

    [SerializeField]
    GameOver gameOver;

    [SerializeField]
    int numRanksForBonusStar = 5;

    [SerializeField]
    string difficultOppenentString = "DIFFICULT OPPONENT - BONUS STAR";

    [SerializeField]
    string winStreakString = "WIN STREAK - BONUS STAR";

    public static Color FRIENDLY_COLOR;
    public static Color ENEMY_COLOR;
    public static Color DRAW_COLOR;

    public static string RANK = "current_rank";
    public static string WIN_STREAK = "win_streak";
    public static int numStars = 3;

    public string myShoutOut;
    public string enemyShoutOut;

    public int enemyRank; // the rank of the enemy at the start of the game

    public string myUsername;
    public string enemyUsername;

    public static GameManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene");
        }
        else
        {
            instance = this;
            FRIENDLY_COLOR = friendlyColor;
            ENEMY_COLOR = enemyColor;
            DRAW_COLOR = drawColor;

            //make sure other players cannot join the room
           
        }
    }

    public void DidSelectTile(TileLogic tile)
    {
        GetComponent<GameplayLogic>().DidSelectTile(tile);
    }

    public void SetTileArray(TileLogic[] tiles)
    {
        GetComponent<GameplayLogic>().SetTileArray(tiles);
    }

    public void LeaveGame()
    {
        //end the game and submit the score to skillz
        SceneManager.LoadScene("HomeScene");
    }


   
}
