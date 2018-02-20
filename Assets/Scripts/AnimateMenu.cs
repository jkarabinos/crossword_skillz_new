using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateMenu : MonoBehaviour {

    [SerializeField]
    SearchingGraphics searchingGraphics;

    [SerializeField]
    GameObject plusIcon;

    [SerializeField]
    GameObject controlPanel;

    [SerializeField]
    GameObject storeZone;

    [SerializeField]
    Button storeButton;

    [SerializeField]
    MenuAudio menuAudio;

    [SerializeField]
    Transform soundButton;

    [SerializeField]
    Transform cancelButton;

    [SerializeField]
    Transform presentButton;

    [SerializeField]
    Transform playButton;

    [SerializeField]
    Transform playerRank;

    [SerializeField]
    Button profileButton;

    [SerializeField]
    Image presentImage;

    [SerializeField]
    Color disabledPresentColor;

    [SerializeField]
    OpenLootbox openLootBox;

    [SerializeField]
    float animationDuration = .75f;

    [SerializeField]
    float newRankScale = .75f;

    public bool isAnimating = false;

    float startTime;

    [SerializeField]
    ScaleButton scaleButton;

    [SerializeField]
    ProfileTutorial profileTut;

    [SerializeField]
    Transform onScreenRight;

    [SerializeField]
    Transform onScreenLeft;

    [SerializeField]
    Transform offScreenRight;

    [SerializeField]
    Transform offScreenLeft;

    [SerializeField]
    Transform offScreenBottom;

    [SerializeField]
    Transform offScreenTop;

    [SerializeField]
    Transform rankSearchingPosition;

    [SerializeField]
    Transform cancelPosition;

    [SerializeField]
    Text playText;

    [SerializeField]
    Text cancelText;

    [SerializeField]
    GameObject unlockZone;

    [SerializeField]
    GameObject profileZone;

    [SerializeField]
    TitleMotion titleMotion;

    Vector3 initialSoundPosition;
    Vector3 initialPresentPosition;
    Vector3 initialPlayPosition;
    Vector3 initialRankPosition;
    

    bool animatingOn = false;
    bool animatingOff = false;

    bool animatingUnlockOn = false;
    bool animatingUnlockOff = false;

    bool unlockZoneActive = false;
    bool goToStoreFromUnlock = false;

    private void Start()
    {

        initialSoundPosition = soundButton.position;
        initialPresentPosition = presentButton.position;
        initialPlayPosition = playButton.position;
        initialRankPosition = playerRank.position;

        plusIcon.GetComponent<FadeObject>().FadeIn();
        //plusIcon.GetComponent<CanvasGroup>().alpha = 1;
    }


    // Update is called once per frame
    void Update () {

        if (isAnimating)
        {
            if (animatingOff)
            {
                LerpTransform(soundButton, initialSoundPosition, onScreenRight.position);
                LerpTransform(presentButton, initialPresentPosition, onScreenLeft.position);
                LerpTransform(playButton, initialPlayPosition, cancelPosition.position);
                LerpTransform(playerRank, initialRankPosition, rankSearchingPosition.position);
                //LerpTransform(cancelButton, offScreenBottom.position, cancelPosition.position);

                FadeText(playText, 1, 0);
                FadeText(cancelText, 0, 1);

                ScaleRank(1.0f, newRankScale);

               

            }
            if(animatingOn)
            {
                LerpTransform(soundButton, onScreenRight.position, initialSoundPosition );
                LerpTransform(presentButton, onScreenLeft.position, initialPresentPosition);
                LerpTransform(playButton, cancelPosition.position, initialPlayPosition);
                LerpTransform(playerRank, rankSearchingPosition.position, initialRankPosition);
                //LerpTransform(cancelButton,  cancelPosition.position, offScreenBottom.position);

                FadeText(playText, 0, 1);
                FadeText(cancelText, 1, 0);

                ScaleRank(newRankScale, 1.0f);
            }

            if (animatingUnlockOn)
            {
                LerpTransform(soundButton, initialSoundPosition, offScreenRight.position);
                LerpTransform(presentButton, initialPresentPosition, offScreenLeft.position);
                LerpTransform(playButton, initialPlayPosition, offScreenBottom.position);
                LerpTransform(playerRank, initialRankPosition, offScreenTop.position);

                //unlockZone.SetActive(true);
                
            }

            if (animatingUnlockOff)
            {
                LerpTransform(soundButton, offScreenRight.position, initialSoundPosition);
                LerpTransform(presentButton, offScreenLeft.position, initialPresentPosition);
                LerpTransform(playButton,  offScreenBottom.position, initialPlayPosition);
                LerpTransform(playerRank, offScreenTop.position, initialRankPosition);

                //unlockZone.SetActive(false);
                
            }

            float p = (Time.time - startTime) / animationDuration;

            if (p > 1)
            {
                isAnimating = false;
                animatingOff = false;
                animatingOn = false;
                animatingUnlockOff = false;
                animatingUnlockOn = false;

                if (goToStoreFromUnlock)
                {
                    goToStoreFromUnlock = false;
                    AnimateStoreOnScreen();
                }
            }

        }

	}

    void FadeText(Text text, int a, int b)
    {
        float p = (Time.time - startTime) / animationDuration;
        text.color = new Color(1, 1, 1, Mathf.SmoothStep(a, b, p));
    }


    void LerpTransform(Transform trans, Vector3 initialPos, Vector3 finalPos)
    {
        float p = (Time.time - startTime) / animationDuration;
        trans.position = Vector3.Lerp(initialPos, finalPos, p);
    }

    void ScaleRank(float oldScale, float newScale)
    {
        float p = (Time.time - startTime) / animationDuration;
        float s = Mathf.SmoothStep(oldScale, newScale, p);
        playerRank.GetComponent<PlayerRank>().ScaleRank(s);

        
    }

    void SetButtonsInteractable(bool interactable, bool isFromPlayButton, bool isUnlockZone)
    {
        presentButton.GetComponent<Button>().interactable = interactable;
        profileButton.interactable = interactable;

        if (!isUnlockZone)
        {
            storeButton.interactable = interactable;

        }

        if(!isFromPlayButton)
            playButton.GetComponent<Button>().interactable = interactable;

        if (interactable)
        {
            presentImage.color = soundButton.GetComponent<Button>().colors.normalColor;
            if(!isUnlockZone)
                plusIcon.GetComponent<FadeObject>().FadeIn();
        }
        else
        {
            presentImage.color = disabledPresentColor;
            if(!isUnlockZone)
                plusIcon.GetComponent<FadeObject>().FadeOut();
        }
    }

    public void AnimateOnScreen()
    {
        menuAudio.PlayOneShot(menuAudio.menuSound2, .75f);
        
        searchingGraphics.FadeOut();

        scaleButton.ResetStartTime();

        SetButtonsInteractable(true, true, false);

        startTime = Time.time;

        animatingOn = true;

        isAnimating = true;
    }

    public void AnimateOffScreen(bool hideTitle)
    {
        menuAudio.PlayOneShot(menuAudio.menuSound1, 1.0f);

        searchingGraphics.FadeIn();

        profileTut.gameObject.SetActive(false);

        SetButtonsInteractable(false, true, false);

        startTime = Time.time;

        animatingOff = true;

        isAnimating = true;
    }


    public void AnimateUnlockOnScreen()
    {


        menuAudio.PlayOneShot(menuAudio.menuSound1, 1.0f);

        SetButtonsInteractable(false, false, true);

        profileTut.gameObject.SetActive(false);

        startTime = Time.time;

        unlockZone.GetComponent<FadeObject>().FadeIn();

        openLootBox.ReloadBuyButton();

        titleMotion.SendTitleOffScreen(animationDuration);

        animatingUnlockOn = true;

        isAnimating = true;

        unlockZoneActive = true;

    }

    public void AnimateUnlockOffScreen(bool playAudio)
    {
        if(playAudio)
            menuAudio.PlayOneShot(menuAudio.menuSound2, .75f);

        SetButtonsInteractable(true, false, true);

        scaleButton.ResetStartTime();

        startTime = Time.time;

        unlockZone.GetComponent<FadeObject>().FadeOut();

        titleMotion.SendTitleOnScreen(animationDuration);

        animatingUnlockOff = true;

        isAnimating = true;

        unlockZoneActive = false;
    }

    public void AnimateProfileOnScreen()
    {
        menuAudio.PlayOneShot(menuAudio.menuSound1, 1.0f);

        profileTut.DidPressProfileButton();

        profileTut.gameObject.SetActive(false);

        SetButtonsInteractable(false, false, false);

        startTime = Time.time;

        profileZone.GetComponent<FadeObject>().FadeIn();

        titleMotion.SendTitleOffScreen(animationDuration);

        animatingUnlockOn = true;

        isAnimating = true;
    }

    public void AnimateProfileOffScreen()
    {
        menuAudio.PlayOneShot(menuAudio.menuSound2, .75f);

        SetButtonsInteractable(true, false, false);

        scaleButton.ResetStartTime();

        startTime = Time.time;

        profileZone.GetComponent<FadeObject>().FadeOut();

        titleMotion.SendTitleOnScreen(animationDuration);

        animatingUnlockOff = true;

        isAnimating = true;
    }

    public void AnimateStoreOnScreen()
    {

        if (unlockZoneActive)
        {
            GoToStoreFromUnlock();
            return;
        }

        menuAudio.PlayOneShot(menuAudio.menuSound1, 1.0f);

        SetButtonsInteractable(false, false, false);

        profileTut.gameObject.SetActive(false);

        startTime = Time.time;

        storeZone.GetComponent<FadeObject>().FadeIn();

        titleMotion.SendTitleOffScreen(animationDuration);

        animatingUnlockOn = true;

        isAnimating = true;
    }


    public void AnimateStoreOffScreen()
    {
        controlPanel.SetActive(true);

        menuAudio.PlayOneShot(menuAudio.menuSound2, .75f);

        SetButtonsInteractable(true, false, false);

        scaleButton.ResetStartTime();

        startTime = Time.time;

        storeZone.GetComponent<FadeObject>().FadeOut();

        titleMotion.SendTitleOnScreen(animationDuration);

        animatingUnlockOff = true;

        isAnimating = true;
    }

    public void GoToStoreFromUnlock()
    {

        goToStoreFromUnlock = true;
        controlPanel.SetActive(false);
        AnimateUnlockOffScreen(false);

    }
    
}
