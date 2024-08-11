using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IInventorySlotUpdateListener
{

    public enum Section
    {
        MAIN,
        TOOLBAR,
    }

    private KeyCode[] numCodes =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };

    public GameObject overlay;
    public GameObject hammerOverlay;
    public GameObject toolbar;
    public GameObject main;
    public GameObject craftingWindow;

    private CanvasGroup overlayGroup;
    private CanvasGroup hammerOverlayGroup;
    private SpriteRenderer itemContainer;

    private int selectedToolbarSlot = 0;

    public void Initialize()
    {
        InitializeSlots();
        SelectToolbarSlot(selectedToolbarSlot);
        overlayGroup = overlay.GetComponent<CanvasGroup>();
        hammerOverlayGroup = hammerOverlay.GetComponent<CanvasGroup>();
        for (int i = 0; i < 20; i++)
        {
            SetItem(Section.MAIN, i, Item.Create(i % 2 == 0 ? Tile.Type.GRASS : Tile.Type.STONE, i + 1));
        }
        AddItem(Item.Create(Item.Type.PICKAXE));
        AddItem(Item.Create(Item.Type.AXE));
    }

    private void InitializeSlots()
    {
        Transform hammerBuildTypeSelector = hammerOverlay.transform.Find("BuildTypeSelector");
        for (int i = 0; i < hammerBuildTypeSelector.childCount; i++)
        {
            SelectionSlot slot = hammerBuildTypeSelector.GetChild(i).GetComponent<SelectionSlot>();
            slot.Initialize();
        }
        for (int i = 0; i < craftingWindow.transform.childCount; i++)
        {
            CraftingSlot slot = craftingWindow.transform.GetChild(i).GetComponent<CraftingSlot>();
            slot.Initialize();
            slot.SetItem(null);
        }
        foreach (Section section in Enum.GetValues(typeof(Section)))
        {
            GameObject sectionObject = GetSectionGameObject(section);
            for (int i = 0; i < sectionObject.transform.childCount; i++)
            {
                InventorySlot slot = sectionObject.transform.GetChild(i).GetComponent<InventorySlot>();
                slot.Initialize();
            }
        }
    }

    private void UpdateCraftingWindow()
    {
        List<ItemIdentifier> craftable = Game.CraftingManager.GetCraftableItems();
        for (int i = 0; i < craftingWindow.transform.childCount; i++)
        {
            if (i < craftable.Count)
            {
                craftingWindow.transform.GetChild(i).GetComponent<CraftingSlot>().SetItem(Item.Create(craftable[i]));
            }
            else
            {
                craftingWindow.transform.GetChild(i).GetComponent<CraftingSlot>().SetItem(null);
            }
        }
    }

    public void SetItemContainer(SpriteRenderer container)
    {
        itemContainer = container;
    }

    public Item AddItem(Item item)
    {
        foreach (Section section in Enum.GetValues(typeof(Section)))
        {
            if (item.type == Item.Type.NONE)
                break;
            item = AddItem(item, section);
        }
        return item;
    }

    public Item AddItem(Item item, Section section)
    {
        GameObject sectionObject = GetSectionGameObject(section);
        for (int i = 0; i < sectionObject.transform.childCount; i++)
        {
            InventorySlot slot = sectionObject.transform.GetChild(i).GetComponent<InventorySlot>();
            if (slot.GetItem().Matches(item))
            {
                Item slotItem = slot.GetItem();
                int amount = Mathf.Min(item.quantity, item.MaxStackSize - slotItem.quantity);
                item.quantity -= amount;
                slot.SetItem(Item.Create(slotItem, slotItem.quantity + amount));
                UpdateCraftingWindow();
                if (item.quantity == 0)
                    return Item.Create(Item.Type.NONE);
            }
            else if (slot.GetItem().type == Item.Type.NONE)
            {
                slot.SetItem(item);
                UpdateCraftingWindow();
                return Item.Create(Item.Type.NONE);
            }
        }
        UpdateCraftingWindow();
        return item;
    }

    public Dictionary<ItemIdentifier, int> GetAvailableItemAmounts()
    {
        Dictionary<ItemIdentifier, int> itemAmounts = new Dictionary<ItemIdentifier, int>();
        foreach (Section section in Enum.GetValues(typeof(Section)))
        {
            GameObject sectionObject = GetSectionGameObject(section);
            for (int i = 0; i < sectionObject.transform.childCount; i++)
            {
                InventorySlot slot = sectionObject.transform.GetChild(i).GetComponent<InventorySlot>();
                Item item = slot.GetItem();
                if (item.type == Item.Type.NONE)
                    continue;
                if (!itemAmounts.ContainsKey(item.Identifier))
                    itemAmounts[item.Identifier] = 0;
                itemAmounts[item.Identifier] += item.quantity;
            }
        }
        return itemAmounts;
    }

    private void Consume(ItemIdentifier identifier, int amount)
    {
        foreach (Section section in Enum.GetValues(typeof(Section)))
        {
            GameObject sectionObject = GetSectionGameObject(section);
            for (int i = 0; i < sectionObject.transform.childCount; i++)
            {
                InventorySlot slot = sectionObject.transform.GetChild(i).GetComponent<InventorySlot>();
                Item item = slot.GetItem();
                if (!item.Identifier.Equals(identifier))
                    continue;
                if (amount >= item.quantity)
                {
                    amount -= item.quantity;
                    slot.SetItem(null);
                }
                else
                {
                    slot.SetItem(Item.Create(item, item.quantity - amount));
                    amount = 0;
                }
                if (amount == 0)
                    break;
            }
            if (amount == 0)
                break;
        }
    }

    public bool ConsumeItems(List<Item> items)
    {
        Dictionary<ItemIdentifier, int> itemAmounts = new Dictionary<ItemIdentifier, int>();
        Dictionary<ItemIdentifier, int> availableItemAmounts = GetAvailableItemAmounts();
        foreach (Item item in items)
        {
            ItemIdentifier identifier = item.Identifier;
            if (!itemAmounts.ContainsKey(identifier))
                itemAmounts[identifier] = 0;
            itemAmounts[identifier] += item.quantity;
        }
        foreach (KeyValuePair<ItemIdentifier, int> pair in itemAmounts)
        {
            if (!availableItemAmounts.ContainsKey(pair.Key) || availableItemAmounts[pair.Key] < pair.Value)
                return false;
        }
        foreach (KeyValuePair<ItemIdentifier, int> pair in itemAmounts)
        {
            Consume(pair.Key, pair.Value);
        }
        UpdateCraftingWindow();
        return true;
    }

    public bool ConsumeItem(Item item)
    {
        return ConsumeItems(new List<Item>() { item });
    }

    private void UpdatePlayerItemContainer()
    {
        if (itemContainer == null)
            return;
        InventorySlot selectedSlot = toolbar.transform.GetChild(selectedToolbarSlot).GetComponent<InventorySlot>();
        if (selectedSlot.GetItem().type == Item.Type.NONE)
        {
            itemContainer.color = new Color(1, 1, 1, 0);
            itemContainer.sprite = null;
        }
        else
        {
            itemContainer.sprite = selectedSlot.GetItem().GetSprite();
            itemContainer.color = Color.white;
        }
    }

    public void SelectToolbarSlot(int slot)
    {
        Transform prevSlot = toolbar.transform.GetChild(selectedToolbarSlot);
        InventorySlot prevInventorySlot = prevSlot.GetComponent<InventorySlot>();
        Transform nextSlot = toolbar.transform.GetChild(slot % toolbar.transform.childCount);
        InventorySlot nextInventorySlot = nextSlot.GetComponent<InventorySlot>();
        prevSlot.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        prevInventorySlot.RemoveInventorySlotUpdateListener(this);
        if (prevInventorySlot.GetItem().type != Item.Type.NONE)
        {
            prevInventorySlot.GetItem().OnDeselected(Input.mousePosition);
        }
        nextSlot.GetComponent<UnityEngine.UI.Image>().color = new Color(0.75f, 0.75f, 0.75f);
        nextInventorySlot.AddInventorySlotUpdateListener(this);
        if (nextInventorySlot.GetItem().type != Item.Type.NONE)
        {
            nextInventorySlot.GetItem().OnSelected(Input.mousePosition);
        }
        selectedToolbarSlot = slot % toolbar.transform.childCount;
        UpdatePlayerItemContainer();
    }

    public void OnInventorySlotUpdate(InventorySlot inventorySlot, Item oldItem, Item newItem)
    {
        if (oldItem.type != Item.Type.NONE)
        {
            oldItem.OnDeselected(Input.mousePosition);
        }
        if (newItem.type != Item.Type.NONE)
        {
            newItem.OnSelected(Input.mousePosition);
        }
        UpdatePlayerItemContainer();
    }

    private void SetItem(Section section, int idx, Item item)
    {
        GameObject sectionObject = GetSectionGameObject(section);
        sectionObject.transform.GetChild(idx).GetComponent<InventorySlot>().SetItem(item);
    }

    private GameObject GetSectionGameObject(Section section)
    {
        if (section == Section.MAIN)
        {
            return main;
        }
        else if (section == Section.TOOLBAR)
        {
            return toolbar;
        }
        return null;
    }

    public void SetInventoryOpen(bool open)
    {
        overlayGroup.interactable = open;
        overlayGroup.blocksRaycasts = open;
        overlayGroup.alpha = open ? 1 : 0;
    }

    public void SetHammerOverlayOpen(bool open)
    {
        hammerOverlayGroup.interactable = open;
        hammerOverlayGroup.blocksRaycasts = open;
        hammerOverlayGroup.alpha = open ? 1 : 0;
    }

    public void SetHammerOverlayListener(ISlotSelectListener listener)
    {
        Transform buildTypeSelector = hammerOverlay.transform.Find("BuildTypeSelector");
        for (int i = 0; i < buildTypeSelector.childCount; i++)
        {
            buildTypeSelector.GetChild(i).gameObject.GetComponent<SelectionSlot>().SetListener(listener);
        }
    }

    public void ToggleInventoryOpen()
    {
        SetInventoryOpen(!IsOpen());
    }

    public bool IsOpen()
    {
        return overlayGroup.interactable;
    }

    public InventorySlot GetSelectedToolbarInventorySlot()
    {
        return toolbar.transform.GetChild(selectedToolbarSlot).GetComponent<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventoryOpen();
        }
        Transform activeSlot = toolbar.transform.GetChild(selectedToolbarSlot);
        InventorySlot activeInventorySlot = activeSlot.GetComponent<InventorySlot>();
        if (activeInventorySlot.GetItem().type != Item.Type.NONE)
        {
            activeInventorySlot.GetItem().SelectedUpdate(Input.mousePosition);
        }
        for (int i = 0; i < numCodes.Length; i++)
        {
            if (i >= toolbar.transform.childCount)
                break;
            if (Input.GetKeyDown(numCodes[i]))
            {
                SelectToolbarSlot(i);
            }
        }
    }
}
