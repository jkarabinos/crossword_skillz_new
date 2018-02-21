using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameplayLogic))]
public class FillTiles : MonoBehaviour {

    [SerializeField]
    private float initialWaitTime = 2.0f;

    [SerializeField]
    private float waitTime = 5.0f;

    TileLogic[] tileFillOrder;

    float currentWaitTime;
    float lastLetterInputTime;

    GameplayLogic gameLogic;

	// Use this for initialization
	void Start () {
        gameLogic = GetComponent<GameplayLogic>();
        currentWaitTime = initialWaitTime;
        lastLetterInputTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - lastLetterInputTime > currentWaitTime)
        {
            //FillRandomTile();
            FillRandomTileNew();
        }
	}

    public void SetTileOrder(TileLogic[] tiles)
    {
        tileFillOrder = new TileLogic[tiles.Length];

        // copy the tile array to the new tile order
        for(int i = 0; i < tiles.Length; i++)
        {
            tileFillOrder[i] = tiles[i];
        }

        //shuffle the order to create a random fill order
        for(int i = 0; i < tiles.Length; i++)
        {
            int randomSwapIndex = Random.Range(0, tiles.Length);
            TileLogic tempTile = tileFillOrder[i];

            tileFillOrder[i] = tileFillOrder[randomSwapIndex];
            tileFillOrder[randomSwapIndex] = tempTile;
        }
    }

    public void ResetWaitTime()
    {
        lastLetterInputTime = Time.time;
        currentWaitTime = waitTime;
    }

    void FillRandomTile()
    {
        Debug.Log("fill random tile");
        ResetWaitTime();

        //fill a random tile with its correct letter
        TileLogic[] tiles = gameLogic.GetTiles();
        string[] userGrid = gameLogic.GetUserGrid();

        Dictionary<int, string> emptyTiles = new Dictionary<int, string>();

        int startIndex = Random.Range(0, tiles.Length - 1);
        for(int i = 0; i < tiles.Length; i++)
        {
            int index = (startIndex + i) % tiles.Length;
            TileLogic t = tiles[index];
            if (t == null)
                continue;

            if (t.tileLetter != userGrid[index])
            {
                emptyTiles.Add(index, t.tileLetter);
                //gameLogic.SetRandomTile(t.tileLetter, index);
                //break;
            }
        }

        List<int> keys = new List<int>(emptyTiles.Keys);

        if(keys.Count > 0)
        {

            int r = Random.Range(0, keys.Count);
            int ind = keys[r];
            gameLogic.SetRandomTile(emptyTiles[ind], ind);
        }

    }

    void FillRandomTileNew()
    {
        ResetWaitTime();
        string[] userGrid = gameLogic.GetUserGrid();

        for(int i = 0; i < tileFillOrder.Length; i++)
        {
            TileLogic tl = tileFillOrder[i];
            if (tl == null)
                continue;

            if(tl.tileLetter != userGrid[tl.index])
            {
                gameLogic.SetRandomTile(tl.tileLetter, tl.index);
                break;
            }
        }
    }
}
