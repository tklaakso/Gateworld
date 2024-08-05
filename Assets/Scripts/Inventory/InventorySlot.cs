using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    private GameObject inventoryItem;
    private GameObject itemCount;
    private Image itemImage;
    private TextMeshProUGUI itemText;

    private Item item;

    public GameObject draggableItem;

    private List<IInventorySlotUpdateListener> inventorySlotUpdateListeners;

    public void Initialize()
    {
        inventorySlotUpdateListeners = new List<IInventorySlotUpdateListener>();
        item = Item.Create(Item.Type.NONE);
        inventoryItem = transform.Find("InventoryItem").gameObject;
        itemCount = transform.Find("ItemCount").gameObject;
        itemImage = inventoryItem.GetComponent<Image>();
        itemText = itemCount.GetComponent<TextMeshProUGUI>();
    }

    public void AddInventorySlotUpdateListener(IInventorySlotUpdateListener listener)
    {
        inventorySlotUpdateListeners.Add(listener);
    }

    public void RemoveInventorySlotUpdateListener(IInventorySlotUpdateListener listener)
    {
        inventorySlotUpdateListeners.Remove(listener);
    }

    public Item GetItem()
    {
        return item;
    }

    public void SetItem(Item item)
    {
        Item oldItem = this.item;
        if (item == null)
        {
            this.item = Item.Create(Item.Type.NONE);
        }
        else
        {
            this.item = item;
        }
        UpdateItemImage();
        foreach (var listener in inventorySlotUpdateListeners)
        {
            listener.OnInventorySlotUpdate(this, oldItem, this.item);
        }
    }

    private void UpdateItemImage()
    {
        if (item.type == Item.Type.NONE)
        {
            itemImage.sprite = null;
            itemImage.color = new Color(255, 255, 255, 0);
            itemText.text = string.Empty;
        }
        else
        {
            itemImage.sprite = item.GetSprite();
            itemImage.color = Color.white;
            if (item.quantity > 1)
            {
                itemText.text = item.quantity.ToString();
            }
            else
            {
                itemText.text = string.Empty;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item.type == Item.Type.NONE)
            return;
        itemImage.transform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item.type == Item.Type.NONE)
            return;
        inventoryItem.transform.SetParent(draggableItem.transform);
        itemCount.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (item.type == Item.Type.NONE)
            return;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        bool intersectsSlot = false;
        foreach (RaycastResult result in results)
        {
            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
            if (slot)
                intersectsSlot = true;
            if (slot && slot != this)
            {
                Item slotItem = slot.GetItem();
                Item.Type type = slotItem.type;
                if (type == Item.Type.NONE)
                {
                    slot.SetItem(item);
                    SetItem(null);
                    break;
                }
                else if (slotItem.Matches(item))
                {
                    int total = slot.GetItem().quantity + item.quantity;
                    slot.SetItem(Item.Create(slotItem, Mathf.Min(total, slotItem.MaxStackSize)));
                    if (total > slotItem.MaxStackSize)
                    {
                        SetItem(Item.Create(slotItem, total - slotItem.MaxStackSize));
                    }
                    else
                    {
                        SetItem(null);
                    }
                    break;
                }
                else
                {
                    Item temp = slot.GetItem();
                    slot.SetItem(item);
                    SetItem(temp);
                }
            }
        }
        if (!intersectsSlot)
        {
            Vector2 playerPos = Game.Player.transform.position;
            GameObject itemEntity = Game.World.CreateEntity(playerPos.x, playerPos.y, Entity.Type.ITEM);
            ItemEntity itemEntityScript = itemEntity.GetComponent<ItemEntity>();
            itemEntityScript.ActivateCooldown();
            itemEntityScript.item = item;
            Util.ApplyItemEntityDispersalForce(itemEntity);
            SetItem(null);
        }
        inventoryItem.transform.SetParent(transform);
        inventoryItem.transform.SetAsFirstSibling();
        itemImage.transform.localPosition = Vector3.zero;
        itemCount.SetActive(true);
    }

    public void Consume(int n)
    {
        if (n >= item.quantity)
        {
            SetItem(null);
        }
        else
        {
            SetItem(Item.Create(item, item.quantity - n));
        }
    }

}
