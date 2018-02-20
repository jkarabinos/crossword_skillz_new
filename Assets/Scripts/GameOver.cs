using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {



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
    Text enemyScoreText;

    [SerializeField]
    Text bestWordScoreText;

    [SerializeField]
    Text bestWordScoreTextEnemy;

    [SerializeField]
    Text numCluesText;

    [SerializeField]
    Text numCluesTextEnemy;

    [SerializeField]
    Text didWinText;



    [SerializeField]
    Transform startPos;

    [SerializeField]
    Transform endPos;

    [SerializeField]
    float shoutOutUpTime = 2.0f;

    [SerializeField]
    string winString = "VICTORY!";

    [SerializeField]
    string loseString = "DEFEAT!";

    [SerializeField]
    string drawString = "DRAW!";

    [SerializeField]
    float fadeDuration = .25f;

    [SerializeField]
    float waitTime = 1.0f;

    [SerializeField]
    float rowAnimationTime = 1.0f;

    [SerializeField]
    float timeBetweenGems = .25f;

    [SerializeField]
    GemRow[] gemRows;

    [SerializeField]
    int smallGemReward = 20;

    [SerializeField]
    int gameCompleteReward = 50;

    [SerializeField]
    int winReward = 50;

    bool gameIsOver = false;


    int winner = 0; // -1 for enemy win, 1 for local win, 0 for draw

    float startTime;

    bool isFading = false;

    float initialRowSize;
    float maxRowSize;

    

    private void Awake()
    {
        

        if(scoreRows.Length > 0)
        {
            initialRowSize = scoreRows[0].GetComponent<RectTransform>().rect.width;
            maxRowSize =  gameOverUI.GetComponent<RectTransform>().rect.width * 2;
        }
    }

    private void OnEnable()
    {
        //EndGame(6, 2); //for testing purposes
    }

    public void EndGame(int _friendlyScore, int _enemyScore, bool _wasDisconnect)
    {
        if (gameIsOver)
            return;

        gameIsOver = true;

        //SetNextButtonActive(false);

        winner = 1;
        

         


        foreach (GameObject row in scoreRows)
        {
            row.GetComponent<RectTransform>().sizeDelta = new Vector2(maxRowSize, row.GetComponent<RectTransform>().rect.height);
        }

        friendlyScoreText.text = _friendlyScore.ToString();
        enemyScoreText.text = _enemyScore.ToString();

        bestWordScoreText.text = gameplayLogic.bestWordScore.ToString();
        bestWordScoreTextEnemy.text = gameplayLogic.bestWordScoreEnemy.ToString();

        numCluesText.text = gameplayLogic.numClues.ToString();
        numCluesTextEnemy.text = gameplayLogic.numCluesEnemy.ToString();

        SetDidWinText();

        gameOverUI.GetComponent<CanvasGroup>().alpha = 0;


        StartCoroutine(EndGameCoroutine());

        AddRewardGems();


        //add the clues solved to the total clues solved for the user and store the new best word if there is one
        PlayerPrefs.SetInt(StatManager.CLUES_SOLVED, PlayerPrefs.GetInt(StatManager.CLUES_SOLVED) + gameplayLogic.numClues);

        int oldBestWord = PlayerPrefs.GetInt(StatManager.BEST_WORD);
        if (gameplayLogic.bestWordScore > oldBestWord)
            PlayerPrefs.SetInt(StatManager.BEST_WORD, gameplayLogic.bestWordScore);
    }

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




    void AddRewardGems()
    {
        int numGems = gameCompleteReward;

        if (int.Parse(bestWordScoreText.text) > int.Parse(bestWordScoreTextEnemy.text))
            numGems += smallGemReward;
        else if (int.Parse(bestWordScoreText.text) == int.Parse(bestWordScoreTextEnemy.text))
            numGems += smallGemReward / 2;

        if (int.Parse(numCluesText.text) > int.Parse(numCluesTextEnemy.text))
            numGems += smallGemReward;
        else if (int.Parse(numCluesText.text) == int.Parse(numCluesTextEnemy.text))
            numGems += smallGemReward / 2;

        if (int.Parse(friendlyScoreText.text) > int.Parse(enemyScoreText.text))
            numGems += winReward;
        else if (int.Parse(friendlyScoreText.text) == int.Parse(enemyScoreText.text))
            numGems += winReward / 2;

        GemManager.AddGems(numGems);
        Debug.Log("adding gems: " + numGems);
        
    }

    IEnumerator AnimateGem(int index)
    {
        yield return new WaitForSeconds(timeBetweenGems);

        if(index < gemRows.Length)
        {

            gameAudio.PlayOneShot(gameAudio.gemSound, .8f);

            GemRow gemRow = gemRows[index];

            if(index == 0)
            {
                gemRow.Setup(int.Parse(bestWordScoreText.text), int.Parse(bestWordScoreTextEnemy.text), smallGemReward, 0);
            }else if(index == 1)
            {
                gemRow.Setup(int.Parse(numCluesText.text), int.Parse(numCluesTextEnemy.text), smallGemReward, 0);
            }else if(index == 2)
            {
                gemRow.Setup(int.Parse(friendlyScoreText.text), int.Parse(enemyScoreText.text), winReward * 2, gameCompleteReward);
            }
           
            StartCoroutine(AnimateGem(index + 1));
        }
        else
        {
            //StartCoroutine(AnimateRowExit());
            lastIndex = -1;
            SetNextButtonActive(true);
        }
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


   

    public void NextButton()
    {
        SceneManager.LoadScene("HomeScene");
    }
}
