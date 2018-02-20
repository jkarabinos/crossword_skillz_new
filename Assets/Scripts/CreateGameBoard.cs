using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreateGameBoard : MonoBehaviour {

    [SerializeField]
    GameObject tilePrefab;

    [SerializeField]
    GameObject gridParent; // contains the game grid

    private CW.Crossword crossword; // the crossword data


    //Called from NetworkingLogic to initialize the crossword puzzle
    public void LoadCrossword(string fileName, int[] scoreMults, int randomIndex)
    {
        if (SelectCrossword(fileName, randomIndex))
        {
            CreateGrid(scoreMults);
        }
    }

    //Select the appropriate crossword from the text file and convert it from a JSON string
    //Returns true if everything is OK
    bool SelectCrossword(string filePath, int randomIndex)
    {
        TextAsset asset = (TextAsset)Resources.Load(filePath);
        if (asset == null) // make sure a file with that name exists
            return false;

        string[] crosswords = asset.text.Split('\n');

        if (randomIndex < 0 || randomIndex >= crosswords.Length) //make sure the random index is within bounds of the array
            return false;

        string currentCrossword = crosswords[randomIndex];
        crossword = JsonUtility.FromJson<CW.Crossword>(currentCrossword);

        return true;
    }

    //Loop through the crossword data and create the necessary tiles
    void CreateGrid(int[] scoreMults)
    {
        float size = GetTileSize();
        float tileScale = size / tilePrefab.GetComponent<RectTransform>().rect.width;

        TileLogic[] tiles = new TileLogic[crossword.size.rows * crossword.size.cols];
        int index = 0;

        for (int row = 0; row < crossword.size.rows; row++)
        {
            for (int col = 0; col < crossword.size.cols; col++)
            {
                index = IndexForRowAndCol(row, col);

                bool isBlank = IsGridSpotBlank(row, col);
                if (isBlank)
                {
                    tiles[index] = null;
                    continue;
                }

                tiles[index] = CreateTile(tileScale, row, col, size, scoreMults);
            }
        }

        GameManager.instance.SetTileArray(tiles); // store the tile array in the game manager
    }

    //Create a tile from the prefab and set the size and properties
    TileLogic CreateTile(float tileScale, int row, int col, float tileSize, int[] scoreMults)
    {
        GameObject tile = Instantiate(tilePrefab, gridParent.transform, false);

        TileLogic tileLogic = tile.GetComponent<TileLogic>();
        if (tileLogic == null)
            return null;

        int index = IndexForRowAndCol(row, col);

        if (index < scoreMults.Length)
            tileLogic.SetScoreMult(scoreMults[index]); // set the randomly generated score multiplier for the tile

        tileLogic.ScaleTile(tileScale, tileSize, true);

        float a = row - (int)(crossword.size.rows / 2) + ((crossword.size.rows % 2 + 1) % 2) * .5f; //set the position of the tile in the grid based on the row and col
        float b = col - (int)(crossword.size.cols / 2) + ((crossword.size.cols % 2 + 1) % 2) * .5f;

        tile.transform.localPosition = new Vector2(b * tileSize, -(a * tileSize));


        string downAnswer = GetClueOrAnswer(false, false, index);
        string downClue = GetClueOrAnswer(true, false, index);
        string acrossAnswer = GetClueOrAnswer(false, true, index);
        string acrossClue = GetClueOrAnswer(true, true, index);

        tileLogic.SetupTile(crossword.grid[index], crossword.gridnums[index], acrossClue, acrossAnswer, downClue, downAnswer, index);

        return tileLogic;

    }

    //Return the clue or answer for the coresponding index and direction
    string GetClueOrAnswer(bool isClue, bool isAcross, int index)
    {
        CW.Indices indices = crossword.gridIndices[index];
        int a = indices.across;
        if (!isAcross)
            a = indices.down;

        string s = null;
        if (a < 0)
            return s;

        if (isClue && isAcross)
            s = crossword.clues.across[a];
        else if (isClue && !isAcross)
            s = crossword.clues.down[a];
        else if (!isClue && isAcross)
            s = crossword.answers.across[a];
        else
            s = crossword.answers.down[a];

        return s;
    }

    //Get the size of the tile
    float GetTileSize()
    {
        float size = gridParent.GetComponent<RectTransform>().rect.width / Mathf.Max(crossword.size.cols, crossword.size.rows);

        return size;
    }

    //If there is not a letter here
    bool IsGridSpotBlank(int row, int col)
    {
        int index = IndexForRowAndCol(row, col);
        return String.Compare(".", crossword.grid[index]) == 0;
    }

    //convert the 2D index to a 1D index
    int IndexForRowAndCol(int row, int col)
    {
        return row * crossword.size.cols + col;
    }
}
