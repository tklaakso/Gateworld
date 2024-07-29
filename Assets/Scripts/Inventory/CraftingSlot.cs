using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    private GameObject inventoryItem;
    private Image itemImage;

    private Item item;

    public void Initialize()
    {
        item = Item.Create(Item.Type.NONE);
        inventoryItem = transform.Find("InventoryItem").gameObject;
        itemImage = inventoryItem.GetComponent<Image>();
    }

    public Item GetItem()
    {
        return item;
    }

    public void SetItem(Item item)
    {
        if (item == null)
        {
            this.item = Item.Create(Item.Type.NONE);
            gameObject.SetActive(false);
        }
        else
        {
            this.item = item;
            gameObject.SetActive(item.type != Item.Type.NONE);
        }
        UpdateItemImage();
    }

    private void UpdateItemImage()
    {
        if (item.type == Item.Type.NONE)
        {
            itemImage.sprite = null;
            itemImage.color = new Color(255, 255, 255, 0);
        }
        else
        {
            itemImage.sprite = item.GetSprite();
            itemImage.color = Color.white;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (item.type == Item.Type.NONE)
            return;
        Game.CraftingManager.CraftSingleItem(item.Identifier);
    }

}
