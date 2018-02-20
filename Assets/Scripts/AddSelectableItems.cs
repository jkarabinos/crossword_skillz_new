using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSelectableItems : MonoBehaviour {


    [SerializeField]
    MenuAudio menuAudio;

    [SerializeField]
    Transform content;

    [SerializeField]
    GameObject rankParentPrefab;

    [SerializeField]
    GameObject playerRankPrefab;

    [SerializeField]
    GameObject shoutOutPrefab;

    [SerializeField]
    LoadRank loadRank;

    [SerializeField]
    OpenLootbox openLootBox;

    [SerializeField]
    Sprite lockedItemSprite;

    [SerializeField]
    bool isBorderSelector;

    List<ItemSelector> items = new List<ItemSelector>();

    public static string SHOUT_OUT_PATH = "ShoutOuts/shoutouts";

    public static string SHOUT_OUTS = "shout_out"; // the string for storing the shoutouts in player prefs


	// Use this for initialization
	void Start () {
        if (isBorderSelector)
        {
            AddRankBorders(UnlockManager.numRareItems + UnlockManager.numEpicItems + UnlockManager.numLegItems);
        }
        else
        {
            AddShoutOuts();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void AddRankBorders(int maxRank)
    {
        //PlayerPrefs.SetInt(PlayerRank.RANK_BORDER + "_" + 3, 1);

        //PlayerPrefs.SetInt(PlayerRank.RANK_BORDER + "_" + 4, 0);

        for (int i = 1; i <= maxRank; i++)
        {

            GameObject parent = Instantiate(rankParentPrefab, content);
            ItemSelector itemSelector = parent.GetComponent<ItemSelector>();

            int rarity = 0;
            if (i <= UnlockManager.numRareItems)
                rarity = 1;
            else if (i <= UnlockManager.numEpicItems + UnlockManager.numRareItems)
                rarity = 2;
            else if (i <= UnlockManager.numRareItems + UnlockManager.numEpicItems + UnlockManager.numLegItems)
                rarity = 3;

            itemSelector.Setup(PlayerRank.RANK_BORDER, i, this, rarity, openLootBox, menuAudio);
            items.Add(itemSelector);

            GameObject playerRank = Instantiate(playerRankPrefab, itemSelector.selectableObjectParent, false);
            playerRank.transform.localPosition = new Vector2(0, -itemSelector.selectableObjectParent.GetComponent<RectTransform>().sizeDelta.y / 2);

            PlayerRank pr = playerRank.GetComponent<PlayerRank>();
            //pr.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            //pr.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);


            /*if (!itemSelector.IsUnlocked()) pr.UpdateRank(0, false, 0);
            else pr.UpdateRank(0, false, i);*/
            SetThemeLocked(itemSelector, pr, i);

            pr.GetComponent<RectTransform>().pivot = new Vector2(.5f, 0);
            itemSelector.SetItem(playerRank);

            pr.SetLetterHidden();

            pr.ScaleRank(parent.GetComponent<RectTransform>().sizeDelta.x / playerRank.GetComponent<RectTransform>().sizeDelta.x);

        }
    }



    public void AddShoutOuts()
    {
        
        /*
        for(int i = 2; i < UnlockManager.numCommonItems; i++)
        {
            PlayerPrefs.SetInt(SHOUT_OUTS + "_" + i, 0);
        }*/


        TextAsset ta = (TextAsset) Resources.Load(SHOUT_OUT_PATH);
        string[] shoutOuts = ta.text.Split('\n');

        
        for(int i = 0; i < shoutOuts.Length; i++)
        {
            GameObject parent = Instantiate(rankParentPrefab, content);
            ItemSelector itemSelector = parent.GetComponent<ItemSelector>();

            itemSelector.Setup(SHOUT_OUTS, i + 1, this, 0, openLootBox, menuAudio);
            items.Add(itemSelector);

            GameObject shoutOut = Instantiate(shoutOutPrefab, itemSelector.selectableObjectParent, false);
            shoutOut.transform.localPosition = new Vector2(0, -itemSelector.selectableObjectParent.GetComponent<RectTransform>().sizeDelta.y / 2);

            ShoutOutItem shoutOutItem = shoutOut.GetComponent<ShoutOutItem>();

            /*
            if (!itemSelector.IsUnlocked()) shoutOutItem.SetShoutOut("");
            else shoutOutItem.SetShoutOut(shoutOuts[i]);*/

            SetShoutOutLocked(itemSelector, shoutOuts[i], shoutOutItem);

            shoutOut.GetComponent<RectTransform>().pivot = new Vector2(.5f, 0);
            itemSelector.SetItem(shoutOut);

        }
    }

    public void ReloadLockImages()
    {
        if (isBorderSelector)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemSelector itemSelector = items[i];

                PlayerRank pr = itemSelector.GetItem().GetComponent<PlayerRank>();
                if (pr != null)
                    SetThemeLocked(itemSelector, pr, i+1);

            }
        }
        else
        {

            TextAsset ta = (TextAsset)Resources.Load(SHOUT_OUT_PATH);
            string[] shoutOuts = ta.text.Split('\n');
            for (int i = 0; i < items.Count; i++)
            {
                ItemSelector itemSelector = items[i];

                ShoutOutItem shoutOutItem = itemSelector.GetItem().GetComponent<ShoutOutItem>();
                if (shoutOutItem != null)
                    SetShoutOutLocked(itemSelector, shoutOuts[i], shoutOutItem);

            }
        }
    }

    void SetShoutOutLocked(ItemSelector itemSelector, string shoutOutText, ShoutOutItem shoutOutItem)
    {
       
        if (!itemSelector.IsUnlocked())
            shoutOutItem.SetShoutOut("");
        else
            shoutOutItem.SetShoutOut(shoutOutText);
        
    }

    void SetThemeLocked(ItemSelector itemSelector, PlayerRank pr, int i)
    {
        if (!itemSelector.IsUnlocked())
            pr.UpdateRank(0, false, 0);
        else
            pr.UpdateRank(0, false, i);
    }

    public void ReloadAllSelectionImages()
    {
        foreach(ItemSelector item in items)
        {
            item.ReloadSelectedImage();
        }

        if(isBorderSelector)
            loadRank.changeBorderDelegate.Invoke();
    }
}
