using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour {

    public static int numCommonItems = 25;
    public static int numRareItems = 12; //12
    public static int numEpicItems = 6; //7
    public static int numLegItems = 5; //5

	// Use this for initialization
	void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static int NumLockedItemsForRarity(int rarity)
    {
        int startValue = 1;
        int endValue = 0;
        string startPath = PlayerRank.RANK_BORDER ;

        if (rarity == 0)
        {
            endValue = numCommonItems;
            startPath = AddSelectableItems.SHOUT_OUTS;
        }
        else if (rarity == 1)
            endValue = numRareItems;
        else if(rarity == 2)
        {
            startValue = numRareItems + 1;
            endValue = numRareItems + numEpicItems;
        }else if(rarity == 3)
        {
            startValue = numRareItems + numEpicItems + 1;
            endValue = numRareItems + numEpicItems + numLegItems;
        }

        int count = 0;

        for(int i = startValue; i <= endValue; i++)
        {
            if (PlayerPrefs.GetInt(startPath + "_" + i) == 0)
                count++;
        }

        return count;
    }

    public static int UnlockRandomItem(int rarity)
    {
        List<int> possibleUnlocks = new List<int>();

        int startValue = 1;
        int endValue = 0;
        string startPath = PlayerRank.RANK_BORDER;

        if (rarity == 0)
        {
            endValue = numCommonItems;
            startPath = AddSelectableItems.SHOUT_OUTS;
        }
        else if (rarity == 1)
            endValue = numRareItems;
        else if (rarity == 2)
        {
            startValue = numRareItems + 1;
            endValue = numRareItems + numEpicItems;
        }
        else if (rarity == 3)
        {
            startValue = numRareItems + numEpicItems + 1;
            endValue = numRareItems + numEpicItems + numLegItems;
        }


        for(int i = startValue; i <= endValue; i++)
        {
            if (PlayerPrefs.GetInt(startPath + "_" + i) == 0)
                possibleUnlocks.Add(i);
        }

        if(possibleUnlocks.Count > 0)
        {
            int n = possibleUnlocks[Random.Range(0, possibleUnlocks.Count)];
            Debug.Log("unlocking: " + startPath + "_" + n);
            PlayerPrefs.SetInt(startPath + "_" + n, 1);

            return n;
        }

        return 0;
    }
}
