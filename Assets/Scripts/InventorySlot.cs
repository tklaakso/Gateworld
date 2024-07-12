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

    // Start is called before the first frame update
    void Awake()
    {
        item = new Item(Item.Type.NONE);
        inventoryItem = transform.Find("InventoryItem").gameObject;
        itemCount = transform.Find("ItemCount").gameObject;
        itemImage = inventoryItem.GetComponent<Image>();
        itemText = itemCount.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Item GetItem()
    {
        return item;
    }

    public void SetItem(Item item)
    {
        if (item == null)
        {
            this.item = new Item(Item.Type.NONE);
        }
        else
        {
            this.item = item;
        }
        UpdateItemImage();
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
            itemImage.sprite = SpriteManager.GetItemByID((int)item.type);
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
        foreach (RaycastResult result in results)
        {
            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
            if (slot && slot != this)
            {
                Item.Type type = slot.GetItem().type;
                if (type == Item.Type.NONE)
                {
                    slot.SetItem(item);
                    SetItem(null);
                    break;
                }
                else if (type == item.type)
                {
                    int total = slot.GetItem().quantity + item.quantity;
                    slot.SetItem(new Item(type, Mathf.Min(total, Item.MAX_STACK)));
                    if (total > Item.MAX_STACK)
                    {
                        SetItem(new Item(type, total - Item.MAX_STACK));
                    }
                    else
                    {
                        SetItem(null);
                    }
                    break;
                }
            }
        }
        inventoryItem.transform.SetParent(transform);
        inventoryItem.transform.SetAsFirstSibling();
        itemImage.transform.localPosition = Vector3.zero;
        itemCount.SetActive(true);
    }
}
