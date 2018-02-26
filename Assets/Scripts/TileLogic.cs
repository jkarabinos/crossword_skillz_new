using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ShakeLetter))]
public class TileLogic : MonoBehaviour {

    public Button tileButton;
    [SerializeField]
    GameObject scoreMultText;

    [SerializeField]
    GameObject borderImage;

    [SerializeField]
    Color highlightedColor;

    [SerializeField]
    Color normalLetterColor;

    [SerializeField]
    Color doubleLetterColor;

    [SerializeField]
    Color tripleLetterColor;

    [SerializeField]
    Color quadLetterColor;

    [SerializeField]
    Text tileLetterText;

    [SerializeField]
    Text clueNumberText;

    [SerializeField]
    Button interactionButton;

    [SerializeField]
    float scaleTime = .25f;

    [SerializeField]
    float maxScaleSize = 1.3f;

    [HideInInspector]
    public string tileLetter;
    [HideInInspector]
    public int clueNumber;

    public string acrossClue;
    public string downClue;
    public string acrossAnswer;
    public string downAnswer;
    public int index;

    public bool didSolveAcross = false;
    public bool didSolveDown = false;

    public int parentAcrossIndex  = -1;
    public int parentDownIndex = -1;



    Color[] colors = new Color[4];

    public int scoreMult = 1;
    Vector2 originalLetterPosition;


    private void Start()
    {
        originalLetterPosition = tileLetterText.transform.localPosition;
    }


    public void SetTileColor(ColorBlock cb, bool isHighlighted, string currentLetter)
    {
        /*if (currentLetter == tileLetter && !isHighlighted)
        {
            ColorBlock newCb = new ColorBlock();
            newCb.normalColor = new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 1);
            newCb.pressedColor = new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 1);
            newCb.disabledColor = new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 1);
            newCb.highlightedColor = new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, 1);
            newCb.colorMultiplier = cb.colorMultiplier;
            newCb.fadeDuration = cb.fadeDuration;
            tileButton.colors = newCb;

        }
        else*/
            tileButton.colors = cb;   

        if (isHighlighted)
            //SetColor(-1);
            SetColor(0);// SetColor(-1); for other highlight method

        else
            SetColor(scoreMult - 1);

        borderImage.SetActive(isHighlighted);
       
    }

    public void SetScoreMult(int _scoreMult){
        colors[0] = normalLetterColor;
        colors[1] = doubleLetterColor;
        colors[2] = tripleLetterColor;
        colors[3] = quadLetterColor;

        scoreMult = _scoreMult;
        SetColor(scoreMult -1);

        if (scoreMult == 1)
            scoreMultText.GetComponent<Text>().text = "";
        else
            scoreMultText.GetComponent<Text>().text = scoreMult + "x";
    }

    public void DidSelectTile()
    {
        GameManager.instance.DidSelectTile(this);
    }

    public string GetCurrentAnswer(bool isAcross)
    {
        if (isAcross)
            return acrossAnswer;
        else
            return downAnswer;
    }

    public bool DidSolveCurrentAnswer(bool isAcross)
    {
        if (isAcross)
            return didSolveAcross;
        else
            return didSolveDown;
    }

    public void SetupTile(string _tileLetter, int _clueNumber, string _acrossClue, string _acrossAnswer, string _downClue, string _downAnswer, int _index)
    {
        index = _index;
        tileLetter = _tileLetter;
        clueNumber = _clueNumber;
        acrossClue = _acrossClue;
        downClue = _downClue;
        acrossAnswer = _acrossAnswer;
        downAnswer = _downAnswer;

        if (downAnswer == null)
            didSolveDown = true;
        if (acrossAnswer == null)
            didSolveAcross = true;

        //if (_clueNumber <= 0)
            //interactionButton.interactable = false;
        UpdateTileUI();
    }

    void UpdateTileUI()
    {
        //tileLetterText.text = tileLetter;
        //tileLetterText.text = "";
        if (clueNumber > 0)
            clueNumberText.text = clueNumber.ToString();
        else
            clueNumberText.text = "";
    }


    bool hasSetPermanent = false;

    public void SetTileText(string letter, Color color, bool isPermanent)
    {
        if (isPermanent && !hasSetPermanent) //if this is the first time setting a permanent letter, scale the darn thing!
            ScaleLetter();

        tileLetterText.text = letter;
        tileLetterText.color = color;

        tileLetterText.transform.localPosition = originalLetterPosition; // in case a shake has set the off center

        if (letter == "")
            scoreMultText.SetActive(true);
        else
            scoreMultText.SetActive(false);
            
    }

    void ScaleLetter()
    {
        hasSetPermanent = true;

        startTime = Time.time;
        isScaling = true;
    }

    public string GetTileText()
    {
        return tileLetterText.text;
    }


    public void ScaleTile(float _newScale, float _newSize, bool _isFirstScale) // resize the tile and font independently
    {
      
        
        GetComponent<RectTransform>().sizeDelta = new Vector2(_newSize, _newSize);
        borderImage.GetComponent<RectTransform>().sizeDelta = new Vector2(_newSize, _newSize);

        tileLetterText.fontSize = (int)(tileLetterText.fontSize * _newScale);
        clueNumberText.fontSize = (int)(clueNumberText.fontSize * _newScale);
        scoreMultText.GetComponent<Text>().fontSize = (int)(scoreMultText.GetComponent<Text>().fontSize * _newScale);
    }

   

    void SetColor(int index)
    {
        if (index < 0)
            interactionButton.GetComponent<Image>().color = highlightedColor;
        else
            interactionButton.GetComponent<Image>().color = colors[index];

    }

    public void ShakeLetter(float _totalTime, bool _isAcross)
    {
        GetComponent<ShakeLetter>().ShakeTheLetter(_totalTime, tileLetterText, _isAcross);
    }


    float startTime;
    bool isScaling = false;

    private void Update()
    {
        if (isScaling)
        {
            float t = (Time.time - startTime) / scaleTime;

            if(t < 1)
            {
                float s = Mathf.SmoothStep(1, maxScaleSize, t * 2);
                if (t > .5f) s = Mathf.SmoothStep(maxScaleSize, 1, (t - .5f) * 2);

                tileLetterText.transform.localScale = new Vector2(s, s);
            }
            else
            {
                tileLetterText.transform.localScale = new Vector2(1, 1);
                isScaling = false;
            }

        }
    }
}
