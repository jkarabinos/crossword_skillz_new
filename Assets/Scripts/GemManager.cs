using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemManager : MonoBehaviour {

    public static string GEMS = "gems";
    public static string LOOTBOXES_PURCHASED = "lootboxes_purchased";

    public static int REWARD_GEMS = 200;
    //public static int GEMS_FOR_PURCHASE = 1;
    static int initialLootBoxCost = 300;
    static int extraCost = 50;



	public static void AddGems(int numGems)
    {

        int oldGems = PlayerPrefs.GetInt(GEMS);
        PlayerPrefs.SetInt(GEMS, oldGems + numGems);

    }


    public static bool CanAffordLootBox()
    {
        if (PlayerPrefs.GetInt(GEMS) >= CostOfLootBox())
            return true;

        return false;
    }

    public static void BuyLootBox()
    {
        if (CanAffordLootBox())
        {
            int oldGems = PlayerPrefs.GetInt(GEMS);
            PlayerPrefs.SetInt(GEMS, oldGems - CostOfLootBox());

            PlayerPrefs.SetInt(LOOTBOXES_PURCHASED, PlayerPrefs.GetInt(LOOTBOXES_PURCHASED) + 1); //store the number of boxes purchased
        }
    }

    public static int CostOfLootBox()
    {
        return initialLootBoxCost + extraCost * PlayerPrefs.GetInt(LOOTBOXES_PURCHASED);
    }
}
