using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    [SerializeField]
    AutomaticSubmission autoSubmit;

    [SerializeField]
    GameObject[] gameRecapLines;

    [SerializeField]
    GameAudio gameAudio;

    [SerializeField]
    Button nextButton;

    [SerializeField]
    GameplayLogic gameplayLogic;

    [SerializeField]
    GameObject[] scoreRows;

    [SerializeField]
    GameObject gameOverUI;


    [SerializeField]
    Text friendlyScoreText;

    [SerializeField]
    Text highScoreText;

    [SerializeField]
    Text timeBonusText;

    [SerializeField]
    Text bestWordScoreText;

    [SerializeField]
    Text numCluesText;

    


    [SerializeField]
    Transform startPos;

    [SerializeField]
    Transform endPos;


    [SerializeField]
    float waitTime = 1.0f;

    [SerializeField]
    float animationTime = .55f;


    [SerializeField]
    GameObject gameBoard;

    [SerializeField]
    GameObject keyboard;

    [SerializeField]
    GameObject clueHolder;

    [SerializeField]
    GameObject scoreHolder;

    [SerializeField]
    GameObject pauseButton;

    [SerializeField]
    Transform newGameBoardPos;

    [SerializeField]
    Transform newKeyboardPos;

    [SerializeField]
    Transform newClueHolderPos;

    [SerializeField]
    Transform newScoreHolderPos;

    [SerializeField]
    Transform newPauseButtonPos;

    [SerializeField]
    float timeMult = .5f; 


    bool gameIsOver = false;


    float startTime;

    bool isAnimating = false;
    int animationIndex = 0;

    float gameStartTime;

    Vector3 originalKeyboardPos;
    Vector3 originalBoardPos;
    Vector3 originalCluePanelPos;
    Vector3 originalScorePos;
    Vector3 originalPausePos;

    Vector2[] originalRecapRowPositions;

    private void Awake()
    {
        gameStartTime = Time.time;

        originalRecapRowPositions = new Vector2[gameRecapLines.Length];
        for(int i = 0; i < originalRecapRowPositions.Length; i++)
        {
            originalRecapRowPositions[i] = gameRecapLines[i].transform.position;
        }

        
    }

    private void OnEnable()
    {
        //EndGame(6, 2); //for testing purposes
    }

    void LerpObject(GameObject go, Vector3 originalPos, Vector3 newPos, bool changeIndex)
    {
        float t = (Time.time - startTime) / animationTime;
        go.transform.position = Vector3.Lerp(originalPos, newPos, t);

        if (t > 1)
        {
            go.transform.position = newPos;

            if(changeIndex)
            {
                startTime = Time.time;
                animationIndex++;

            }
        }          
    }

    int playedSoundForIndex = 0;

    void MoveRecapRow(GameObject go, Vector2 originalPos)
    {
        

        Vector2 centerPos = new Vector2(gameOverUI.transform.position.x, go.transform.position.y);
        float t = (Time.time - startTime) / animationTime;

        if(t > .3)
        {
            if(animationIndex != playedSoundForIndex)
            {
                gameAudio.PlayOneShot(gameAudio.whoosh1, 1.0f);
                playedSoundForIndex = animationIndex;
            }
        }


        go.transform.position = Vector2.Lerp(originalPos, centerPos, t);
        if(t > 1)
        {
            go.transform.position = centerPos;
            startTime = Time.time;
            animationIndex++;
        }
    }

    private void Update()
    {
        if (isAnimating)
        {

            switch (animationIndex) {

                case 0:
                    LerpObject(keyboard, originalKeyboardPos, newKeyboardPos.position, true);
                    break;
                case 1:
                    LerpObject(scoreHolder, originalScorePos, newScoreHolderPos.position, false);
                    LerpObject(pauseButton, originalPausePos, newPauseButtonPos.position, true);
                    break;
                case 2:
                    
                    LerpObject(gameBoard, originalBoardPos, newGameBoardPos.position, true);
                    break;
                case 3:
                    LerpObject(clueHolder, originalCluePanelPos, newClueHolderPos.position, true);
                    break;
                case 4:
                    gameOverUI.SetActive(true);
                    //autoSubmit.SetSubmissionTimer();
                    animationIndex++;
                    break;
                case 12:
                    autoSubmit.SetSubmissionTimer();
                    animationIndex++;
                    break;
                case 13:
                    break;
                default:
                    int index = animationIndex - 5;
                    MoveRecapRow(gameRecapLines[index], originalRecapRowPositions[index]);
                    break;
            }



        }
    }

    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(waitTime);

        //gameOverUI.SetActive(true);

        isAnimating = true;
        startTime = Time.time;
    }

    int GetTimeBonus()
    {
        int maxTime = gameplayLogic.GetTiles().Length * gameplayLogic.GetComponent<FillTiles>().GetWaitTime();
        int gameTime = (int)(Time.time - gameStartTime);
        int bonusPoints = (maxTime - gameTime);
        return (int)(bonusPoints * timeMult);
    }

    private int finalScore;

    public void EndGame(int _friendlyScore, int _enemyScore, bool _wasDisconnect)
    {
        if (gameIsOver)
            return;

        gameIsOver = true;

        //SetNextButtonActive(false);


        originalBoardPos = gameBoard.transform.position;
        originalKeyboardPos = keyboard.transform.position;
        originalCluePanelPos = clueHolder.transform.position;
        originalPausePos = pauseButton.transform.position;
        originalScorePos = scoreHolder.transform.position;


        int timeBonus = Mathf.Max(0,  GetTimeBonus());
        finalScore = _friendlyScore + timeBonus;

        if (HomeLogic.isUsingSkillz)
        {
            if (SkillzCrossPlatform.IsMatchInProgress())
                SkillzCrossPlatform.UpdatePlayersCurrentScore(finalScore);
        }

        int highscore = 0;
        if (PlayerPrefs.HasKey("highscore"))
            highscore = PlayerPrefs.GetInt("highscore");

        if(finalScore >= highscore) // for a new highscore
        {
            highscore = finalScore;
            PlayerPrefs.SetInt("highscore", highscore);
            highScoreText.color = friendlyScoreText.color;
        }

        timeBonusText.text = timeBonus.ToString();
        friendlyScoreText.text = finalScore.ToString();
        bestWordScoreText.text = gameplayLogic.bestWordScore.ToString();
        numCluesText.text = gameplayLogic.numClues.ToString();
        highScoreText.text = highscore.ToString();
       
        StartCoroutine(EndGameCoroutine());

        
    }

    /*
    IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(waitTime);

        gameOverUI.SetActive(true);

        isFading = true;
        startTime = Time.time;
    }

    bool isAnimatingRows = false;
    bool isAnimatingRowsOut = false;
    bool isAnimatingShoutoutText = false;

    int lastIndex = -1;

    private void Update()
    {
        if (isFading)
        {
            float p = (Time.time - startTime) / fadeDuration;
            gameOverUI.GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(0, 1, p);
            if (p > 1)
            {
                isFading = false;
                startTime = Time.time;
                isAnimatingRows = true;
            }
        }

        if (isAnimatingRows)
        {
            float p = ((Time.time - startTime) / rowAnimationTime);
            int index = (int)p;

            SetPreviousRows(index); //manually set the positions of the previous rows to make sure they reach the exact center

            if (index >= scoreRows.Length)
            {
                isAnimatingRows = false;

                
                return;
            }

            if(index != lastIndex)
            {
                lastIndex = index;
                gameAudio.PlayOneShot(gameAudio.whoosh1, 1.0f);
            }

            GameObject row = scoreRows[index];
            row.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.SmoothStep(maxRowSize, initialRowSize, p - index), row.GetComponent<RectTransform>().rect.height);
        }

        


      
        
    }

    Vector3 XPosForPos(Vector3 oldPos, Vector3 newPos)
    {
        Vector3 pos = new Vector3(newPos.x, oldPos.y, oldPos.z);
        return pos;
    }



    [SerializeField]
    Color notInteractableColor;

    void SetNextButtonActive(bool active)
    {
        nextButton.interactable = active;
        if (!active)
            nextButton.GetComponentInChildren<Text>().color = notInteractableColor;
        else
            nextButton.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
    }

   

    void AnimateRowExit()
    {
        //yield return new WaitForSeconds(0.0f);

        //disconnectedGO.GetComponent<FadeObject>().FadeOut();


        SetNextButtonActive(false);

        

        startTime = Time.time;

        isAnimatingRowsOut = true;
    }

    void SetPreviousRows(int index)
    {
        for(int i = index - 1; i >= 0; i--)
        {
            GameObject row = scoreRows[i];
            row.GetComponent<RectTransform>().sizeDelta = new Vector2(initialRowSize, row.GetComponent<RectTransform>().rect.height);
        }
    }

    public void HideGameOverUI()
    {
        gameOverUI.SetActive(false);
    }

    void SetDidWinText()
    {
        if (winner > 0)
        {
            didWinText.text = winString;
            didWinText.color = GameManager.FRIENDLY_COLOR;
        }
        else if (winner < 0)
        {
            didWinText.text = loseString;
            didWinText.color = GameManager.ENEMY_COLOR;
        }
        else
        {
            didWinText.text = drawString;
            didWinText.color = GameManager.DRAW_COLOR;
        }
    }

    */
   

    public void NextButton()
    {
        GameManager.instance.LeaveGame(finalScore);
    }
}
