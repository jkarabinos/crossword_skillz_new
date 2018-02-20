using UnityEngine;
using System;


public class CreateBoard : MonoBehaviour {

    [SerializeField]
    GameObject tilePrefab;

    [SerializeField]
    GameObject gridParent; // contains the game grid

    private CW.Crossword crossword; // the crossword data


    //Called from NetworkingLogic to initialize the crossword puzzle
    public void LoadCrossword(string fileName, int[] scoreMults, int randomIndex)
    {
        if(SelectCrossword(fileName, randomIndex))
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

        int[] lastColIndices = new int[crossword.size.cols];
        for (int k = 0; k < lastColIndices.Length; k++)
            lastColIndices[k] = -1;

        for(int row = 0; row < crossword.size.rows; row++)
        {
            int lastNumberedAcrossClueIndex = -1;
            for(int col = 0; col < crossword.size.cols; col++)
            {
                
                index = IndexForRowAndCol(row, col);

                bool isBlank = IsGridSpotBlank(row, col);
                if (isBlank)
                {
                    lastNumberedAcrossClueIndex = -1;
                    lastColIndices[col] = -1;
                    tiles[index] = null;
                    continue;
                }
                
                tiles[index] = CreateTile(tileScale, row, col, size);

                TileLogic tileLogic = tiles[index];
                if(index < scoreMults.Length)
                    tileLogic.SetScoreMult(scoreMults[index]); // set the randomly generated score multiplier for the tile

                if(tileLogic.acrossAnswer != null) // if the clue has an across answer, set it as the most recent
                {
                    lastNumberedAcrossClueIndex = index;
                }
                else if(lastNumberedAcrossClueIndex > -1)// if the clue does not have an across answer, set it's parent across answer to be the most recent across
                {
                    tileLogic.parentAcrossIndex = lastNumberedAcrossClueIndex;
                }

                //now do the same for the down answers
                if(tileLogic.downAnswer != null)
                {
                    lastColIndices[col] = index;
                }else if(lastColIndices[col] > -1)
                {
                    tileLogic.parentDownIndex = lastColIndices[col];
                }
            }
        }

        GameManager.instance.SetTileArray(tiles); // store the tile array in the game manager
    }

    //Create a tile from the prefab and set the size and properties
    TileLogic CreateTile(float tileScale, int row, int col, float tileSize)
    {
        GameObject tile = Instantiate(tilePrefab, gridParent.transform, false);
        
        TileLogic tileLogic = tile.GetComponent<TileLogic>();
        if (tileLogic == null)
            return null;

        tileLogic.ScaleTile(tileScale, tileSize, true);

        float a = row - (int)(crossword.size.rows / 2) + ((crossword.size.rows % 2 + 1) % 2) * .5f;
        float b = col - (int)(crossword.size.cols / 2) + ((crossword.size.cols % 2 + 1) % 2) * .5f;

        tile.transform.localPosition = new Vector2(b * tileSize, -(a * tileSize));

        int index = IndexForRowAndCol(row, col);

        string _downAnswer = GetClueOrAnswer(false, false, index);
        string _downClue = GetClueOrAnswer(true, false, index);
        string _acrossAnswer = GetClueOrAnswer(false, true, index);
        string _acrossClue = GetClueOrAnswer(true, true, index);

        tileLogic.SetupTile(crossword.grid[index], crossword.gridnums[index], _acrossClue, _acrossAnswer, _downClue, _downAnswer, IndexForRowAndCol(row, col));

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
