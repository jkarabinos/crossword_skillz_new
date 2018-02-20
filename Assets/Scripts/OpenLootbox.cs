using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenLootbox : MonoBehaviour {


    [SerializeField]
    GameObject[] panels; // the panels in order of rarity

    [SerializeField]
    GameObject fourColorParent;

    [SerializeField]
    AnimateMenu animateMenu;

    [SerializeField]
    GemCounter gemCounter;

    [SerializeField]
    Button lootBoxButton;

    [SerializeField]
    GameObject shoutOutPrefab;

    [SerializeField]
    GameObject rankBorderPrefab;

    [SerializeField]
    float fadeDelay = 2.0f;

    [SerializeField]
    float fadeDelayForItem = .5f;

    [SerializeField]
    float minAlpha = .2f;

    [SerializeField]
    float maxScale = 1.2f;

    [SerializeField]
    int numBaseColorFades = 100;

    [SerializeField]
    float fadeDuration = .1f;

    [SerializeField]
    float waitPercentage = .5f;

    [SerializeField]
    int[] rarityPercentages;

    [SerializeField]
    AddSelectableItems[] selectableItems;

    [SerializeField]
    Text rarityText;

    [SerializeField]
    Transform unlockedItemParent;

    [SerializeField]
    MenuAudio menuAudio;

    [SerializeField]
    string[] rarities;

    List<float> originalSaturations;

    float startTime;

    int numFades;

    float currentFadeDuration = 1;
    

	// Use this for initialization
	void Start () {
        originalSaturations = new List<float>();
        foreach(GameObject panel in panels)
        {
            float h;
            float s;
            float v;
            Color.RGBToHSV(panel.GetComponent<Image>().color, out h, out s, out v);

            originalSaturations.Add(s);
        }
	}

    int iterations = 0;
    bool isAnimating = false;

	// Update is called once per frame
	void Update () {
        if (!isAnimating)
            return;


        float p = (Time.time - startTime) / currentFadeDuration;
      

        if(p > 1)
        {

            iterations++;

            if(iterations <= numFades)
                menuAudio.PlayOneShot(menuAudio.boopSound, 1.0f, .80f + Mathf.Pow(iterations, 1.2f) * .01f);

            currentFadeDuration = TimeForIteration(iterations);
            startTime = Time.time;

            p = (Time.time - startTime) / currentFadeDuration;
        }

        if(iterations <= numFades)
        {
            float t = p;
            int index = iterations % panels.Length;
            for(int j = 0; j < panels.Length; j++)
            {
                if(j == index)
                {
                    float w2 = (1 - waitPercentage) / 2;
                    float a = Mathf.SmoothStep(minAlpha, 1, t * (1 / w2));
                    float s = Mathf.SmoothStep(1, maxScale, t * (1 / w2));
                    if (t > .5f && iterations != numFades)
                    {
                        a = Mathf.SmoothStep(1, minAlpha, (t - waitPercentage - w2) * (1 / w2));
                        s = Mathf.SmoothStep(maxScale, 1, (t - waitPercentage - w2) * (1 / w2));
                    }

                    SetScale(panels[j], s);
                    SetColor(panels[j], a);
                }
                else
                {
                    SetScale(panels[j], 1);
                    SetColor(panels[j], minAlpha);
                }
            }
        }
        else
        {
            UnlockItem(numFades);
            isAnimating = false;
        }
        
	}


    float TimeForIteration(int i)
    {
        
        return Mathf.Max(.1f, Mathf.Pow(2, i - numFades) * 1.6f);
    }

    void UnlockItem(int numFades)
    {
        Debug.Log("unlock item");

        int rarity = numFades - numBaseColorFades;

        int itemID = UnlockManager.UnlockRandomItem(rarity);

        ReloadBuyButton();

        //PlayerPrefs.SetInt(AddSelectableItems.SHOUT_OUTS + "_" + 14, 1);
        rarityText.text = rarities[rarity];
        Color c = panels[rarity].GetComponent<Image>().color;
        rarityText.color = new Color(c.r, c.g, c.b, 1);

        foreach(AddSelectableItems asi in selectableItems)
        {
            asi.ReloadLockImages(); // reload all the components to show the newly unlocked item
        }

        StartCoroutine(SetNewItemVisible(rarity, itemID));

        fourColorParent.GetComponent<FadeObject>().FadeOut();
        
        rarityText.GetComponent<FadeObject>().FadeIn();

        //StartCoroutine(ScaleTextWithDelay(rarityText.GetComponent<FadeObject>().GetFadeTime()));
        StartCoroutine(ScaleTextWithDelay(rarityText.GetComponent<FadeObject>().GetFadeTime() / 2));

        StartCoroutine(FadeAfterDelay(fadeDelay));
    }

    public Color ColorForRarity(int rarity)
    {
        Color c = panels[rarity].GetComponent<Image>().color;
        return new Color(c.r, c.g, c.b, 1);
    }

    IEnumerator ScaleTextWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        menuAudio.PlayOneShot(menuAudio.openBoxSound, 1.0f, 1.0f);

        rarityText.GetComponent<ScaleObject>().ScaleObj();
    }

    IEnumerator SetNewItemVisible(int rarity, int itemID)
    {
        //Debug.Log("the item id is " + itemID);
        yield return new WaitForSeconds(fadeDelayForItem);

        

        unlockedItemParent.GetComponent<CanvasGroup>().alpha = 0;

        if(rarity == 0)
        {
            
            TextAsset ta = (TextAsset)Resources.Load(AddSelectableItems.SHOUT_OUT_PATH);
            string[] shoutOuts = ta.text.Split('\n');

            GameObject shoutOut = Instantiate(shoutOutPrefab, unlockedItemParent, false);
            shoutOut.transform.localPosition = new Vector2(0, 0);

            float m = unlockedItemParent.GetComponent<RectTransform>().sizeDelta.x / shoutOut.GetComponent<RectTransform>().sizeDelta.x; 
            shoutOut.GetComponent<RectTransform>().sizeDelta = unlockedItemParent.GetComponent<RectTransform>().sizeDelta;

            ShoutOutItem soi = shoutOut.GetComponent<ShoutOutItem>();
            soi.SetShoutOut(shoutOuts[itemID - 1]);
            soi.SetTextBorders(m);
        }
        else
        {
            GameObject playerBorder = Instantiate(rankBorderPrefab, unlockedItemParent, false);
            playerBorder.transform.localPosition = new Vector2(0, 0);

            PlayerRank pr = playerBorder.GetComponent<PlayerRank>();

            pr.UpdateRank(0, false, itemID);
            pr.SetLetterHidden();
            pr.ScaleRank(unlockedItemParent.GetComponent<RectTransform>().sizeDelta.x / playerBorder.GetComponent<RectTransform>().sizeDelta.x);
        }

        //add the new item as a child here

        unlockedItemParent.GetComponent<FadeObject>().FadeIn();
    }

    IEnumerator FadeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        

        GetComponent<FadeObject>().FadeOut();
    }

    public void OpenABox()
    {
        if (shouldGoToStore)
        {
            animateMenu.GoToStoreFromUnlock();
            return;
        }

        GemManager.BuyLootBox(); // subtract the required gems from the user's gem count

        ReloadBuyButton();

        gemCounter.RefreshGemCount();

        foreach(GameObject panel in panels)
        {
            SetColor(panel, minAlpha);
        }

        foreach(Transform child in unlockedItemParent.transform)
        {
            Destroy(child.gameObject);
        }

        iterations = 0;

        GetComponent<FadeObject>().FadeIn();

        fourColorParent.GetComponent<CanvasGroup>().alpha = 1;

        rarityText.GetComponent<CanvasGroup>().alpha = 0;

        currentFadeDuration = fadeDuration;

        numFades = numBaseColorFades + GetRandomRarity();

        startTime = Time.time + GetComponent<FadeObject>().GetFadeTime();

        isAnimating = true;

        menuAudio.PlayOneShot(menuAudio.menuSound1, 1.0f, 1.0f);
    }


    int GetRandomRarity()
    {
        int r = Random.Range(1, 101);

        int a1 = UnlockManager.NumLockedItemsForRarity(0);
        int a2 = UnlockManager.NumLockedItemsForRarity(1);
        int a3 = UnlockManager.NumLockedItemsForRarity(2);
        int a4 = UnlockManager.NumLockedItemsForRarity(3);

        
        if (r <= rarityPercentages[0] && a1 > 0)
            return 0;
        if (r <= (rarityPercentages[1] + rarityPercentages[0]) && a2 > 0)
            return 1;
        if (r <= (rarityPercentages[2] + rarityPercentages[1] + rarityPercentages[0]) && a3 > 0)
            return 2;
        if (r <= (rarityPercentages[3] + rarityPercentages[2] + rarityPercentages[1] + rarityPercentages[0]) && a4 > 0)
            return 3;

        if (a1 > 0)
            return 0;
        if (a2 > 0)
            return 1;
        if (a3 > 0)
            return 2;

        return 0;
    }

    void SetScale(GameObject go, float scale)
    {
        go.transform.localScale = new Vector2(scale, scale);
    }

    void SetColor(GameObject go, float alpha)
    {
        //Color c = go.GetComponent<Image>().color;
        //go.GetComponent<Image>().color = new Color(c.r, c.g, c.b, alpha);
        go.GetComponent<CanvasGroup>().alpha = alpha;
        /*float h;
        float s;
        float v;
        Color.RGBToHSV(c, out h, out s, out v);
        go.GetComponent<Image>().color = Color.HSVToRGB(h, saturation, v);*/
    }

    bool shouldGoToStore = false;

    public void ReloadBuyButton()
    {
        //set the buy button to not be interactable if the user does not have enough gems to make a purchase

        int totalUnlocks =  UnlockManager.NumLockedItemsForRarity(0) + 
                            UnlockManager.NumLockedItemsForRarity(1) + 
                            UnlockManager.NumLockedItemsForRarity(2) + 
                            UnlockManager.NumLockedItemsForRarity(3);

        if (GemManager.CanAffordLootBox() && totalUnlocks > 0)
        {
            //lootBoxButton.interactable = true;
            shouldGoToStore = false;
            lootBoxButton.GetComponent<BounceButton>().SetBouncing(true);
        }
        else
        {
            //lootBoxButton.interactable = false;
            shouldGoToStore = true;
            lootBoxButton.GetComponent<BounceButton>().SetBouncing(false);
        }
    }
}
