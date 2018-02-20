using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelector : MonoBehaviour {

    [SerializeField]
    public Transform selectableObjectParent;



    [SerializeField]
    Image selectionImage;

    [SerializeField]
    Color selectedColor;

    [SerializeField]
    Color notSelectedColor;

    [SerializeField]
    float minAlpha = .4f;

    [SerializeField]
    float selectedScale = 1.2f;

    [SerializeField]
    float lineSelectedScale = 1.6f;

    string itemName;
    int itemID;
    AddSelectableItems addSelectableItems;
    MenuAudio menuAudio;

    bool isSelected;
    bool isLocked;


    GameObject selectableItem;
   

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        

        if (isSelected)
        {
            float s = Mathf.MoveTowards(selectableItem.transform.localScale.x, selectedScale, Time.deltaTime * 2);
            float s2 = Mathf.MoveTowards(selectionImage.transform.localScale.x, lineSelectedScale, Time.deltaTime * 2f);
            selectableItem.transform.localScale = new Vector2(s, s);
            selectionImage.transform.localScale = new Vector2(s2, 1);

            SetAlpha(selectionImage, Mathf.MoveTowards(selectionImage.color.a, 1, Time.deltaTime * 2));
        }
        else
        {
            float s = Mathf.MoveTowards(selectableItem.transform.localScale.x, 1, Time.deltaTime * 2);
            float s2 = Mathf.MoveTowards(selectionImage.transform.localScale.x, 1, Time.deltaTime * 2f);
            selectableItem.transform.localScale = new Vector2(s, s);
            selectionImage.transform.localScale = new Vector2(s2, 1);

            SetAlpha(selectionImage, Mathf.MoveTowards(selectionImage.color.a, minAlpha, Time.deltaTime * 2));
        }
	}


    public void Setup(string _itemName, int _itemID, AddSelectableItems _addSelectableItems, int rarity, OpenLootbox openLootBox, MenuAudio  _menuAudio)
    {
        itemID = _itemID;
        itemName = _itemName;
        addSelectableItems = _addSelectableItems;
        menuAudio = _menuAudio;

        SetColorForRarity(rarity, openLootBox);

        ReloadSelectedImage();

        //selectionImage.GetComponent<CanvasRenderer>(). = ImageProcess.SetGrayscale(selectionImage.sprite.texture) as Texture;
    }


    void SetColorForRarity(int rarity, OpenLootbox openLootBox)
    {
        selectionImage.color = openLootBox.ColorForRarity(rarity);
        SetAlpha(selectionImage, minAlpha);
    }

    void SetAlpha(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    public bool IsUnlocked()
    {
        if (!PlayerPrefs.HasKey(UnlockStringForID()))
        {
            if(itemID != 1)
                PlayerPrefs.SetInt(UnlockStringForID(), 0); // set the unlock to be locked initially
            else
                PlayerPrefs.SetInt(UnlockStringForID(), 1);
        }

       
        if(PlayerPrefs.GetInt(UnlockStringForID()) != 0)
        {
            isLocked = false;
            return true;
        }

        isLocked = true;
        return false;

    }

    public GameObject GetItem()
    {
        return selectableItem;
    }

    public void SetItem(GameObject item)
    {
        selectableItem = item;
    }

    public string UnlockStringForID()
    {
        return itemName + "_" + itemID;
    }

    public void SelectThis()
    {
        if (isLocked || isSelected)
            return;

        PlayerPrefs.SetInt(itemName, itemID);
        addSelectableItems.ReloadAllSelectionImages();

        menuAudio.PlayOneShot(menuAudio.itemSelectSound, 1.0f);

    }

    public void ReloadSelectedImage()
    {
        if (PlayerPrefs.GetInt(itemName) == itemID)
        {
            //selectionImage.color = selectedColor;
            isSelected = true;
        }
        else
        {
            //selectionImage.color = notSelectedColor;
            isSelected = false;
        }
    }
}
