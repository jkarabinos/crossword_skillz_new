using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AnimateBackground))]
public class PlayerRank : MonoBehaviour {

    [SerializeField]
    Text rankText;

    [SerializeField]
    GameAudio gameAudio;

    [SerializeField]
    Image borderImage;

    [SerializeField]
    GameObject zImage;

    [SerializeField]
    Sprite fullStar;

    [SerializeField]
    Sprite emptyStar;

    [SerializeField]
    Image[] stars;

    [SerializeField]
    Image[] backgroundStars;

    [SerializeField]
    float fadeDuration = .25f;

    [SerializeField]
    float starAnimationTime = .2f;

    [SerializeField]
    float maxStarScale = 1.1f;

    [SerializeField]
    float maxLetterScale = 1.3f;

    [SerializeField]
    LoadRank loadRank;

    public static string RANK_BORDER = "rank_border";
    public static string BORDER_PATH = "Borders/Border_";

    List<string> alphabet = new List<string>() {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    int originalFontSize;
    Vector2 originalSizeDelta;
    Vector2 originalZSizeDelta;

    AnimateBackground animateBackground;

    private void Awake()
    {
        //PlayerPrefs.SetInt(GameManager.RANK, 0);

        animateBackground = GetComponent<AnimateBackground>();

        originalFontSize = rankText.fontSize;
        originalSizeDelta = this.GetComponent<RectTransform>().sizeDelta;
        originalZSizeDelta = zImage.GetComponent<RectTransform>().sizeDelta;

        if (loadRank != null)
        {
            loadRank.changeBorderDelegate += ReloadBackgroundImage;
        }
    }

    // Use this for initialization
    void Start () {
        //ScaleRank(.75f);
        
        
    }

    float fadeStartTime;
    bool isFadingIn = false;

    float rankStartTime;
    bool isAnimatingRank;

    bool isAnimatingLetterChange = false;

    int oldRank;
    int newRank;

	// Update is called once per frame
	void Update () {
        if (isFadingIn)
        {
            float t = (Time.time - fadeStartTime) / fadeDuration;
            GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(0, 1, t);
            if(t > 1)
            {
                GetComponent<CanvasGroup>().alpha = 1;
                isFadingIn = false;
            }
        }

        if (isAnimatingRank && rankUp)
        {
            float t = (Time.time - rankStartTime) / starAnimationTime;
            int index = (int)t;
            t = t - index;

            if (index == stars.Length &&  index < numStars)
            {
                StartLetterRankChange();
                return;
            }
            if (index >= numStars )
            {
                isAnimatingRank = false;
                return;
            }

            if (index == numStars - 3 && gotBonusStar && newLetterRank)
                FadeStar(t, index);

            if ((index == numStars - 2) && gotBonusStar)
                FadeStar(t, index);

            if (index == numStars - 1)
                FadeStar(t, index);

            ScaleStar(t, index);
            
        }
        else if (isAnimatingRank && !rankUp)
        {
            if(fadeStarIndex < 0) // if we had zero stars going into the game
            {
                StartLetterRankChange();
                return;
            }
                

            float t = (Time.time - rankStartTime) / (starAnimationTime * 2);
            FadeStar(t, fadeStarIndex);
        }


        if (isAnimatingLetterChange)
        {
            float t1 = (Time.time - rankStartTime) / (starAnimationTime * 3);
            float t2 = (Time.time - rankStartTime) / (starAnimationTime * 2);

            for (int i =0; i < stars.Length; i++)
            {
                FadeStar(t1, i);
            }

            FadeLetter(t2);
            if(t2 > 1 && rankUp)
            {
                float t3 = (Time.time - rankStartTime - starAnimationTime * 2) / starAnimationTime;
                ScaleLetter(t3);

                if(t3 > 1 && (numStars == (GameManager.numStars + 2) || numStars == (GameManager.numStars + 3)))
                {
                    AnimateLastStar();
                }
            }

            if(t2 > 1 && !rankUp)
            {
                float t3 = (Time.time - rankStartTime - starAnimationTime * 2) / starAnimationTime;

                if(t3 > 1 && fadeStarIndex < 0)
                {
                    AnimateLastStarDown();
                }
            }
        }
	}

    void AnimateLastStarDown()
    {
        isAnimatingLetterChange = false;
        rankStartTime = Time.time;

        foreach (Image s in stars)
        {
            s.sprite = fullStar;
            s.color = new Color(1, 1, 1, 1);
        }

        foreach (Image s in backgroundStars)
        {
            s.sprite = emptyStar;
            s.color = new Color(1, 1, 1, 0);
        }

        fadeStarIndex = 2;
        isAnimatingRank = true;
    }
    void AnimateLastStar()
    {
        isAnimatingLetterChange = false;
        rankStartTime = Time.time;

        foreach (Image s in stars)
        {
            s.sprite = emptyStar;
            s.color = new Color(1, 1, 1, 1);
        }

        foreach (Image s in backgroundStars)
        {
            s.sprite = fullStar;
            s.color = new Color(1, 1, 1, 0);
        }


        numStars = numStars - stars.Length - 1;

        Debug.Log("new num stars is " + numStars);
        isAnimatingRank = true;
    }

    public void SetLetterHidden()
    {
        rankText.gameObject.SetActive(false);
    }

    //start animations that will change the letter and fade the stars
    void StartLetterRankChange()
    {
        isAnimatingRank = false;

        //set the background stars to all full for a rank down

        if((numStars == 4 && !rankUp)||(numStars == 5 && rankUp) )//|| (numStars ==  )
        {
            stars[2].sprite = fullStar;
            stars[2].color = new Color(1, 1, 1, 1);
            backgroundStars[2].color = new Color(1, 1, 1, 0);
        }
        
        
        foreach(Image s in backgroundStars)
        {
            if (!rankUp)
                s.sprite = fullStar;
            else
                s.sprite = emptyStar;
        }

        rankStartTime = Time.time;
        isAnimatingLetterChange = true;

    }

    bool hasFaded = false;

    void FadeLetter(float t)
    {

        if (!hasFaded)
        {
            hasFaded = true;
            StartCoroutine(PlayRankSound());
        }

        if (t > .5f)
        {

            /*if(newRank == 25 * (GameManager.numStars + 1))
            {
                rankText.gameObject.SetActive(false);
                zImage.SetActive(true);
                zImage.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.SmoothStep(0, 1, (t - .5f) * 2));
            }
            else
            {*/
                rankText.text = LetterForRank(newRank);
                rankText.color = new Color(1, 1, 1, Mathf.SmoothStep(0, 1, (t - .5f) * 2));
            //}
        }
        else
        {
            rankText.color = new Color(1, 1, 1, Mathf.SmoothStep(1, 0, t * 2));
        }
    }

    IEnumerator PlayRankSound()
    {
        yield return new WaitForSeconds(.35f);

        if (rankUp)
            gameAudio.PlayOneShot(gameAudio.rankUpSound, 1.0f);
        else
            gameAudio.PlayOneShot(gameAudio.rankDownSound, .8f);
    }

    void ScaleLetter(float t)
    {
        float s;
        if (t < .5)
            s = Mathf.SmoothStep(1, maxLetterScale, t * 2);
        else
            s = Mathf.SmoothStep(maxLetterScale, 1, (t - .5f) * 2);

        rankText.GetComponent<RectTransform>().localScale = new Vector2(s, s);
        zImage.GetComponent<RectTransform>().localScale = new Vector2(s, s);
    }


    void FadeStar(float t, int index)
    {
        
        stars[index].color = new Color(1, 1, 1, Mathf.SmoothStep(1, 0, t));

        

    }
    int lastIndex = -1;
    int starUpCount = 0;

    void ScaleStar(float t, int index)
    {

        if (index != lastIndex && gameAudio != null)
        {
            lastIndex = index;
            gameAudio.PlayOneShot(gameAudio.starUpSound, 3.5f, .8f + .05f * starUpCount);
            starUpCount++;
        }

        float s;
        if (t < .5)
            s = Mathf.SmoothStep(1, maxStarScale, t * 2);
        else
            s = Mathf.SmoothStep(maxStarScale, 1, (t - .5f) * 2);

        stars[index].GetComponent<RectTransform>().localScale = new Vector2(s, s);
        backgroundStars[index].GetComponent<RectTransform>().localScale = new Vector2(s, s);
    }

    public void UpdateRank(int rank, bool showStars, int borderIndex)
    {
        rankText.text = LetterForRank(rank);

        int starIndex = rank % (GameManager.numStars + 1);
        for(int i = 0; i < starIndex; i++)
        {
            stars[i].sprite = fullStar;
            backgroundStars[i].sprite = emptyStar;
        }
        for(int j = starIndex; j < GameManager.numStars; j++)
        {
            stars[j].sprite = emptyStar;
            backgroundStars[j].sprite = fullStar;
        }

        float maxRank = 25 * (GameManager.numStars + 1);
        if (rank == maxRank)
        {
            //zImage.SetActive(true);
            //rankText.gameObject.SetActive(false);
            showStars = false;
        }

        if (!showStars)
        {
            foreach (Image star in stars)
                star.gameObject.SetActive(false);
        }


        SetBackground(borderIndex);

    }


    void ReloadBackgroundImage()
    {
        SetBackground(PlayerPrefs.GetInt(PlayerRank.RANK_BORDER));
    }

    void SetBackground(int borderIndex)
    {
        string folderPath = BORDER_PATH + borderIndex;
        //Debug.Log("folder path is " + folderPath);
        Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath);
        //Sprite s = Resources.Load<Sprite>(folderPath + "/rank_border");

        borderImage.sprite = sprites[0];
        if(sprites.Length > 2)
        {
            animateBackground.SetInitialBorderImages(sprites);
            animateBackground.SetAnimation(true);
        }
        else
        {
            animateBackground.SetAnimation(false);

        }

    }

    string LetterForRank(int rank)
    {
        int letterIndex = rank / (GameManager.numStars + 1);
        if (letterIndex < alphabet.Count)
             return alphabet[letterIndex];

        return "";
    }


    public void ScaleRank(float newScale)
    {
        this.GetComponent<RectTransform>().sizeDelta = newScale * originalSizeDelta;
        zImage.GetComponent<RectTransform>().sizeDelta = newScale * originalZSizeDelta;
        /*
        foreach(Image starImage in stars)
        {
            starImage.GetComponent<RectTransform>().sizeDelta = newScale * starImage.GetComponent<RectTransform>().sizeDelta;
        }*/

        rankText.fontSize = (int) (originalFontSize * newScale);
    }

    public void AnimateRankChange(int _oldRank, int _newRank)
    {
        oldRank = _oldRank;
        newRank = _newRank;

        FadeIn();
        StartCoroutine(WaitForFade());
    }

    bool rankUp = false;
    bool newLetterRank = false;
    bool gotBonusStar = false;

    int numStars;
    int fadeStarIndex;

    IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(fadeDuration * 3);

        bool isDraw = false;
        //Debug.Log("new rank is " + newRank);
       // Debug.Log("old rank is " + oldRank);

        if (newRank > oldRank)
            rankUp = true;
        else if (oldRank > newRank)
            fadeStarIndex = (oldRank % (GameManager.numStars + 1)) -1;
        else
        {
            //Debug.Log("it is a draw in terms of displaying rank");
            isDraw = true;
        }


        if (!isDraw)
        {

            //if (((newRank % (GameManager.numStars + 1) == 0) && rankUp) || (!rankUp && (newRank % (GameManager.numStars + 1) == GameManager.numStars)) )
            //newLetterRank = true;

            if ((newRank / (GameManager.numStars + 1)) != (oldRank / (GameManager.numStars + 1)))
                newLetterRank = true;

            if ((newRank - oldRank == 2 && !newLetterRank) || (newRank - oldRank == 3 && newLetterRank))
                gotBonusStar = true;

            numStars = (oldRank % (GameManager.numStars + 1)) + (newRank - oldRank);

            Debug.Log("initial num stars is " + numStars);

            rankStartTime = Time.time;

            isAnimatingRank = true;
        }
    }

    void FadeIn()
    {
        fadeStartTime = Time.time;
        isFadingIn = true;
    }


    
}
