using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FillTiles))]
public class GameplayLogic : MonoBehaviour {

    [SerializeField]
    GameAudio gameAudio;

    [SerializeField]
    Text clueText;

    [SerializeField]
    ScoreLogic scoreLogic;
    
    [SerializeField]
    Color localInputTextColor;

    [SerializeField]
    Text scoreText;

    [SerializeField]
    Text enemyScoreText;

    [Header("Highlighted Colors")]
    [SerializeField]
    ColorBlock highlightedColorBlock;

    [Header("Normal Colors")]
    [SerializeField]
    ColorBlock normalColorBlock;

    private TileLogic[] tiles;
    TileLogic currentTile;
    bool currentDirectionIsAcross;

    string[] userGrid;
    //public string currentUserText;

    string[] currentUserInput;
    //int currentInputIndex = 0;

    string[] localPlayerGrid; // tiles filled in by the local player
    string[] enemyPlayerGrid; // tiles filled in by the opponent
    string[] neutralGrid; // tiles filled in by the random neutral fill

    [SerializeField]
    Color localColor;
    [SerializeField]
    Color enemyColor;
    [SerializeField]
    Color neutralColor;

    [SerializeField]
    private float waitToInteractTime = .25f;

    [SerializeField]
    GameOver gameOver;


    FillTiles fillTiles;
    NetworkingLogic networkingLogic;

    public bool canInteract = true;



    private void Start()
    {
        networkingLogic = GetComponent<NetworkingLogic>();
        fillTiles = GetComponent<FillTiles>();
    }

  


    public void SetGrids(string [] _friendlyGrid, string[] _enemyGrid, string[] _neutralGrid)
    {
        fillTiles.ResetWaitTime();

        //localPlayerGrid = _friendlyGrid;
        //enemyPlayerGrid = _enemyGrid;
        //neutralGrid = _neutralGrid;
        SyncGrids(_friendlyGrid, localPlayerGrid, enemyPlayerGrid, neutralGrid);
        SyncGrids(_enemyGrid, enemyPlayerGrid, localPlayerGrid, neutralGrid);
        SyncGrids(_neutralGrid, neutralGrid, enemyPlayerGrid, localPlayerGrid);

        //if(canInteract)
        //InputLetter("", false); // in case another user or a random tile completes a word the user is in the middle of finishing

        UpdateSolvedClues();
        UpdateScore();
        UpdateBoardTilesText();
        CheckGameOver();
    }

    void SyncGrids(string[] newGrid, string[] gridToCopyTo, string[] otherGrid1, string[] otherGrid2)
    {
        for(int i = 0; i < newGrid.Length; i++)
        {
            if(newGrid[i] != "" && otherGrid1[i] == "" && otherGrid2[i] == "")
            {
                gridToCopyTo[i] = newGrid[i];
            }
        }
    }

    public void InputLetter(string letter, bool fromLocalUser)
    {
        // currentUserText += letter;
        //string answer = currentTile.GetCurrentAnswer(currentDirectionIsAcross);

        //if (currentUserText.Length == answer.Length)
        if (fromLocalUser)
        {
            int inputIndex = GetInputIndex();
            if (inputIndex >= currentUserInput.Length)
                return;
            currentUserInput[inputIndex] = letter;
        }

        int nextInputIndex = GetInputIndex();
        if(nextInputIndex >= currentUserInput.Length)
            CheckAnswer(currentUserInput, fromLocalUser);

        UpdateBoardTilesText();
    }

    int GetInputIndex()
    {
        UpdateUserGrid();
        int offset = 1;
        if(!currentDirectionIsAcross)
            offset = (int)Mathf.Sqrt(tiles.Length);

        for(int i = 0; i < currentUserInput.Length; i++)
        {
            int index = currentTile.index + i * offset;
            if (userGrid[index] == "" && currentUserInput[i] == "")
                return i;
        }

        return currentUserInput.Length;
    }

    
    string GetUserAnswer()
    {
        UpdateUserGrid();
        int offset = 1;
        if (!currentDirectionIsAcross)
            offset = (int)Mathf.Sqrt(tiles.Length);
        string answer = "";
        for (int i = 0; i < currentUserInput.Length; i++)
        {
            int index = currentTile.index + i * offset;
            if (userGrid[index] != "")
                answer += userGrid[index];
            else if (currentUserInput[i] != "")
                answer += currentUserInput[i];
                
        }

        return answer;
    }

    public void ClearAllInput()
    {
        //currentUserText = "";
        currentUserInput = BlankGrid(currentTile.GetCurrentAnswer(currentDirectionIsAcross).Length);
        UpdateBoardTilesText();
    }

    public void ClearLastLetter()
    {

        UpdateUserGrid();
        int offset = 1;
        if (!currentDirectionIsAcross)
            offset = (int)Mathf.Sqrt(tiles.Length);

        for (int i = currentUserInput.Length - 1; i >= 0; i--)
        {
            int index = currentTile.index + i * offset;
            if (userGrid[index] == "" && currentUserInput[i] != "")
            {
                currentUserInput[i] = "";
                break;
            }
        }
        UpdateBoardTilesText();
        /*
        if (currentUserText.Length > 0)
        {
            currentUserText = currentUserText.Substring(0, currentUserText.Length - 1);
            UpdateBoardTilesText();
        }*/
    }

    public TileLogic[] GetTiles()
    {
        return tiles;
    }

    public string[] GetUserGrid()
    {
        UpdateUserGrid();
        return userGrid;
    }

    void UpdateUserGrid() // set the string array user grid to contain all the text from the local player, enemy player, and neutral grids
    {
        AddGridToUserGrid(enemyPlayerGrid);
        AddGridToUserGrid(neutralGrid);
        AddGridToUserGrid(localPlayerGrid);
    }

    void AddGridToUserGrid(string[] grid)
    {
        for(int i = 0; i < grid.Length; i++)
        {
            if (grid[i] != "")
                userGrid[i] = grid[i];
        }
    }

    public void SetRandomTile(string letter, int index)
    {
        //userGrid[index] = letter;
        //neutralGrid[index] = letter;

        string[] tempNeutralGrid = CopyGrid(neutralGrid);
        tempNeutralGrid[index] = letter;
        networkingLogic.UpdateBoard(localPlayerGrid, enemyPlayerGrid, tempNeutralGrid);



        InputLetter("", false);
    }

    bool CheckGameOver()
    {
        

        bool isOver = true;
        foreach(TileLogic tl in tiles)
        {
            if(tl != null)
            {
                if (!tl.didSolveAcross || !tl.didSolveDown)
                    isOver = false;
            }
        }
        if (isOver)
        {
            Debug.Log("GAME OVER");
            networkingLogic.EndGame(oldScore, oldEnemyScore);
            //gameOver.EndGame(oldScore, oldEnemyScore);
        }

        return isOver;
    }

    void UpdateSolvedClues()
    {
        UpdateUserGrid();
        foreach(TileLogic tl in tiles)
        {
            if (tl == null)
                continue;

            if (!tl.didSolveDown)
                tl.didSolveDown = DidSolveClue(tl, false, userGrid);
            if (!tl.didSolveAcross)
                tl.didSolveAcross = DidSolveClue(tl, true, userGrid);
        }
    }

    bool DidSolveClue(TileLogic tile, bool isAcross, string[] grid)
    {
        int offset = 1;
        string answer = tile.GetCurrentAnswer(isAcross);
        if (!isAcross)
            offset = (int)Mathf.Sqrt(tiles.Length);

       
        for(int i = 0; i < answer.Length; i++)
        {
            string letter = grid[tile.index + i * offset];
            if (answer.Substring(i, 1) != letter)
            {
                return false;
            }
        }
        return true;
    }

    void CheckAnswer(string[] userInput, bool fromLocalUser)
    {
        string userAnswer = GetUserAnswer();

        if (userAnswer == currentTile.GetCurrentAnswer(currentDirectionIsAcross) && !currentTile.DidSolveCurrentAnswer(currentDirectionIsAcross))
        {


            Debug.Log("send the correct answer to the other players");
            //Debug.Log("the user has input the correct answer");
            //put the correct answer in the user grid and select the next tile
            UpdateUserGrid();
            int offset = 1;
            if(!currentDirectionIsAcross)
                offset = (int)Mathf.Sqrt(tiles.Length);

            string[] tempFriendlyGrid = CopyGrid(localPlayerGrid);

            for (int j = 0; j < currentUserInput.Length; j++)
            {
                int index = currentTile.index + j * offset;
                if (userGrid[index] == "")
                    tempFriendlyGrid[index] = currentUserInput[j];
            }

            networkingLogic.UpdateBoard(tempFriendlyGrid, enemyPlayerGrid, neutralGrid);
                    

            TileLogic nextTile = currentTile;
            for(int i = 0; i < tiles.Length; i++)
            {
                TileLogic tl = tiles[(currentTile.index + i + 1) % tiles.Length];
                if(tl != null)
                {
                    //if the tile has as grid number and can be selected
                    if(tl.clueNumber > 0 && (!tl.didSolveAcross || !tl.didSolveDown))
                    {
                        nextTile = tl;
                        break;
                    }
                }
            }
            
            DidSelectTile(nextTile);
        }
        else
        {
            if (!canInteract)
                Debug.Log("dont do the animation or set the delay");
            else
                StartCoroutine(ReselectTileWithDelay(currentUserInput, fromLocalUser));
            //ShowWordSelected(currentTile, currentDirectionIsAcross); // reselect the current tile to show the user that the answer is wrong
        }
    }

    IEnumerator ReselectTileWithDelay(string[] userInput, bool fromLocalUser)
    {


        bool toggle = true;
        if (!canInteract)
            toggle = false;

        canInteract = false;

        if(fromLocalUser)
            gameAudio.PlayOneShot(gameAudio.failedSolveClueAudio, 1.0f);

        UpdateUserGrid();
        int offset = 1;
        if(!currentDirectionIsAcross)
            offset = (int)Mathf.Sqrt(tiles.Length);
        for(int i =0; i < userInput.Length; i++)
        {
            int index = currentTile.index + i * offset;
            if(userGrid[index] == "")
            {
                TileLogic tl = tiles[index]; //one of the tiles that the user was attempting to fill in
                tl.ShakeLetter(waitToInteractTime - .2f, currentDirectionIsAcross);
            }
        }

        yield return new WaitForSeconds(waitToInteractTime);
        ShowWordSelected(currentTile, currentDirectionIsAcross);
        if(toggle)
            canInteract = true;
    }

    string[] CopyGrid(string[] _grid)
    {
        string[] grid = new string[_grid.Length];
        for(int i = 0; i < _grid.Length; i++)
        {
            grid[i] = _grid[i];
        }
        return grid;
    }

    public void DidSelectTile(TileLogic tile)
    {
        if(canInteract)
            DidSelectTileWithDirection(tile, true, false);
    }

    void DidSelectTileWithDirection(TileLogic tile, bool isAcross, bool selectNow)
    {
        //first, if the tile is already selected, switch from across to down or visa virsa
        bool tilesMatch = false;
        if (currentTile != null)
        {
            if (tile == currentTile)
            {
                if (tile.acrossAnswer != null && !currentDirectionIsAcross)
                {
                    ShowWordSelected(tile, true);
                    return;
                }
                else if (tile.downAnswer != null && currentDirectionIsAcross)
                {
                    ShowWordSelected(tile, false);
                    return;
                }
                tilesMatch = true;
            }
        }

        if (isAcross && !tilesMatch)
        {

            if ((tile.acrossClue != null && !tile.didSolveAcross) || selectNow)
            { // try to select the answers that haven't been solved first
                ShowWordSelected(tile, true);
                return;
            }
            else if (tile.downClue != null && !tile.didSolveDown)
            {
                ShowWordSelected(tile, false);
                return;
            }
            /*
            else if (tile.acrossAnswer != null )
            {
                ShowWordSelected(tile, true);
                return;
            }
            else if (tile.downClue != null )
            {
                ShowWordSelected(tile, false);
                return;
            }*/
        }
        else if(!tilesMatch)
        {
             if ((tile.downClue != null && !tile.didSolveDown) || selectNow)
            {
                ShowWordSelected(tile, false);
                return;
            }
            else if (tile.acrossClue != null && !tile.didSolveAcross)
            { // try to select the answers that haven't been solved first
                ShowWordSelected(tile, true);
                return;
            }
             /*
            else if (tile.downClue != null )
            {
                ShowWordSelected(tile, false);
                return;
            }
            else if (tile.acrossAnswer != null )
            {
                ShowWordSelected(tile, true);
                return;
            }*/
        }

        bool canSelect = true;

        bool shouldSelectAcross = false;
        bool shouldSelectDown = false;
        if (tile.parentAcrossIndex > -1)
        {
            if (currentTile != null)
            {
                if (currentTile == tiles[tile.parentAcrossIndex] && currentDirectionIsAcross)
                    canSelect = false;
            }
            if (canSelect)
            {
                //DidSelectTileWithDirection(tiles[tile.parentAcrossIndex], true);
                //return;
                shouldSelectAcross = true;
            }
        }
        if (tile.parentDownIndex > -1)
        {
            canSelect = true;
            if (currentTile != null)
            {
                if (currentTile == tiles[tile.parentDownIndex] && !currentDirectionIsAcross)
                    canSelect = false;
            }
            if (canSelect)
            {
                //DidSelectTileWithDirection(tiles[tile.parentDownIndex], false);
                //return;
                shouldSelectDown = true;
            }
        }

        bool hasSolvedParentAcross = true;
        bool hasSolvedParentDown = true;
        if (shouldSelectAcross)
            hasSolvedParentAcross = tiles[tile.parentAcrossIndex].didSolveAcross;
        if (shouldSelectDown)
            hasSolvedParentDown = tiles[tile.parentDownIndex].didSolveDown;

        if(isAcross && !tilesMatch)
        {

            if (tile.acrossAnswer != null && hasSolvedParentDown)
            {
                ShowWordSelected(tile, true);
                return;
            }
            else if (tile.downClue != null && hasSolvedParentAcross)
            {
                ShowWordSelected(tile, false);
                return;
            }
        }else if (!tilesMatch)
        {
            if (tile.downClue != null && hasSolvedParentAcross)
            {
                ShowWordSelected(tile, false);
                return;
            }
            else if (tile.acrossAnswer != null && hasSolvedParentDown)
            {
                ShowWordSelected(tile, true);
                return;
            }
        }

        if (shouldSelectAcross && !shouldSelectDown)
        {
            DidSelectTileWithDirection(tiles[tile.parentAcrossIndex], true, true);
            return;
        }
        if(shouldSelectDown && !shouldSelectAcross)
        {
            DidSelectTileWithDirection(tiles[tile.parentDownIndex], false, true);
            return;
        }
        if(shouldSelectAcross && shouldSelectDown)
        {
            if(tiles[tile.parentAcrossIndex].didSolveAcross && !tiles[tile.parentDownIndex].didSolveDown)
            {
                DidSelectTileWithDirection(tiles[tile.parentDownIndex], false, true);

                //Debug.Log("select the down parent tile with index " + tile.parentDownIndex);
            }
            else
                DidSelectTileWithDirection(tiles[tile.parentAcrossIndex], true, true);
        }
    }

    public void SetTileArray(TileLogic[] _tiles)
    {
        tiles = _tiles;
        ResetTileColors();
        userGrid = BlankGrid(tiles.Length);

        localPlayerGrid = BlankGrid(tiles.Length);
        enemyPlayerGrid = BlankGrid(tiles.Length);
        neutralGrid = BlankGrid(tiles.Length);
        
        DidSelectTile(tiles[0]); // a bit risky but I trust it
        UpdateBoardTilesText();
    }

    public void UpdateBoardTilesText() //update the tiles with the locked in text or the text the user is attempting to input
    {
        //SetTileTextFromGridWithColor(userGrid, Color.black, true, true);
        SetTileTextFromGridWithColor(localPlayerGrid, localColor,  true, true);
        SetTileTextFromGridWithColor(enemyPlayerGrid, enemyColor,  false, true);
        SetTileTextFromGridWithColor(neutralGrid, neutralColor,  false, true);

        int offset = 1;
        if (!currentDirectionIsAcross)
            offset = (int) Mathf.Sqrt(tiles.Length);

        string[] grid = BlankGrid(userGrid.Length);
        //now add any text the user is trying to input to the grid

        /*
        for(int j = 0; j < currentUserText.Length; j++)
        {
            int index = currentTile.index + j * offset;
            grid[index] = currentUserText.Substring(j, 1);
        }*/
        UpdateUserGrid();

        for(int j = 0; j < currentUserInput.Length; j++)
        {
            int index = currentTile.index + j * offset;
            if (userGrid[index] == "")
                grid[index] = currentUserInput[j];
        }

        SetTileTextFromGridWithColor(grid, localInputTextColor,  false, false);
    }

    string[] BlankGrid(int size)
    {
        string[] grid = new string[size];
        for(int i = 0; i < size; i++)
        {
            grid[i] = "";
        }
        return grid;
    }

    void SetTileTextFromGridWithColor(string[] grid, Color color, bool fillBlanks, bool isPermanent)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            TileLogic tl = tiles[i];
            if (tl == null)
                continue;

            if(grid[i] != "")
                tl.SetTileText(grid[i], color, isPermanent);
            else if (fillBlanks)
            {
                bool isInputHere = false;
                int index = i - currentTile.index;
                int s = (int)Mathf.Sqrt(tiles.Length);
                
                if(index >= 0)
                {
                    if(!currentDirectionIsAcross && (currentTile.index % s == i % s))
                    {
                        index = index / s;
                        if (index < currentUserInput.Length)
                            isInputHere = (currentUserInput[index] != "");
                    }
                    else if (currentDirectionIsAcross)
                    {
                        if (index < currentUserInput.Length)
                            isInputHere = (currentUserInput[index] != "");
                    }

                }
                if (!isInputHere)
                    tl.SetTileText(grid[i], color, false);
                //else
                    //Debug.Log("don't draw blank over user input at index " + i);
            }
        }
    }

    void ShowWordSelected(TileLogic tile, bool isAcross)
    {
        currentTile = tile;
        currentDirectionIsAcross = isAcross;
        //currentUserText = "";
        //currentInputIndex = 0;

        if (isAcross)
        {
            //clueText.text = tile.acrossClue;
            SetClueText(tile.acrossClue);
            //currentUserInput = new string[tile.acrossAnswer.Length];
            currentUserInput = BlankGrid(tile.acrossAnswer.Length);
        } else
        {
            SetClueText(tile.downClue);
            //clueText.text = tile.downClue;
            //currentUserInput = new string[tile.downAnswer.Length];
            currentUserInput = BlankGrid(tile.downAnswer.Length);
        }

        //SelectButton(downButton, !isAcross);
        //SelectButton(acrossButton, isAcross);

        HighlightTiles(tile, isAcross);

        UpdateBoardTilesText();
    }
   

    void SetClueText(string _text)
    {
        //Debug.Log("the last letter is " + _text.Substring(_text.Length - 1, 1));
        if(_text.Length <= 3)
        {
            clueText.text = _text;
            return;
        }


        if(_text.Substring(_text.Length -1, 1) == "\"")
        {
            _text = _text.Remove(_text.Length - 1, 1);
            _text = _text.Remove(0, 1);
        }

        List<int> indices = new List<int>();

        for(int i = 0; i < _text.Length - 1; i++)
        {
            if (_text.Substring(i, 2) == "\"\"")
                indices.Add(i);
        }

        if(indices.Count % 2 == 0)
        {
            for(int j = indices.Count - 1; j >=0; j--)
            {
                _text = _text.Remove(indices[j], 1);
            }
        }

        clueText.text = _text;
    }

    void HighlightTiles(TileLogic startingTile, bool isAcross)
    {
        //ResetTileColors();

        string answer = startingTile.acrossAnswer;
        int offset = 1;
        if (!isAcross)
        {
            offset = (int)Mathf.Sqrt(tiles.Length);
            answer = startingTile.downAnswer;
        }

        List<int> highlightIndices = new List<int>();
        
        for(int i = 0; i < answer.Length; i++)
        {
            int index = startingTile.index + i * offset;
            //TileLogic tl = tiles[index];
            //tl.SetTileColor(highlightedColorBlock, true);
            highlightIndices.Add(index);
            
        }

        for(int j = 0; j < tiles.Length; j++)
        {
            TileLogic tl = tiles[j];
            if (tl == null) continue;

            if (highlightIndices.Contains(j))
                tl.SetTileColor(highlightedColorBlock, true);
            else
                tl.SetTileColor(normalColorBlock, false);
        }
    }

    void ResetTileColors()
    {
        foreach(TileLogic tl in tiles)
        {
            if (tl != null)
                tl.SetTileColor(normalColorBlock, false);
        }
    }


    public int oldScore = 0;
    public int oldEnemyScore = 0;

    public int bestWordScore = 0;
    public int bestWordScoreEnemy= 0;

    public int numClues = 0;
    public int numCluesEnemy;

    void UpdateScore()
    {
        int s1 = 0;
        int s2 = 0;

        for(int i =0; i < tiles.Length; i++)
        {
            if (localPlayerGrid[i] != "")
                s1 += tiles[i].scoreMult;

            if (enemyPlayerGrid[i] != "")
                s2 += tiles[i].scoreMult;
        }

        int scoreDelta = s1 - oldScore;
        scoreLogic.AddScore(scoreDelta, true, scoreText, s1);

        if (scoreDelta > bestWordScore)
            bestWordScore = scoreDelta;

        if (scoreDelta > 0)
        {
            numClues++;
            gameAudio.PlayOneShot(gameAudio.solvedClueAudio, 1.0f);
        }

        scoreDelta = s2 - oldEnemyScore;
        scoreLogic.AddScore(scoreDelta, false, enemyScoreText, s2);

        if (scoreDelta > bestWordScoreEnemy)
            bestWordScoreEnemy = scoreDelta;

        if (scoreDelta > 0)
        {
            numCluesEnemy++;
            gameAudio.PlayOneShot(gameAudio.enemySolvedClueAudio, 1.0f);
        }

        //scoreText.text = "ME: " + s1;
        //enemyScoreText.text = "YOU: " + s2;
        oldScore = s1;
        oldEnemyScore = s2;
    }

    
}
