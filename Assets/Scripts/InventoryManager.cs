using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IInventorySlotUpdateListener
{

    public enum Section
    {
        TOOLBAR,
        MAIN,
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
    public GameObject toolbar;
    public GameObject main;

    public GameObject player;

    private CanvasGroup overlayGroup;
    private SpriteRenderer itemContainer;

    private int selectedToolbarSlot = 0;

    // Start is called before the first frame update
    void Start()
    {
        itemContainer = player.transform.Find("ItemContainer").GetComponent<SpriteRenderer>();
        SelectToolbarSlot(selectedToolbarSlot);
        overlayGroup = overlay.GetComponent<CanvasGroup>();
        for (int i = 0; i < 20; i++)
        {
            SetItem(Section.MAIN, i, Item.Create(i % 2 == 0 ? Item.Type.GRASS : Item.Type.STONE, i + 1));
        }
    }

    private void UpdatePlayerItemContainer()
    {
        InventorySlot selectedSlot = toolbar.transform.GetChild(selectedToolbarSlot).GetComponent<InventorySlot>();
        if (selectedSlot.GetItem().type == Item.Type.NONE)
        {
            itemContainer.color = new Color(1, 1, 1, 0);
            itemContainer.sprite = null;
        }
        else
        {
            itemContainer.sprite = SpriteManager.GetItemByID((int)selectedSlot.GetItem().type);
            itemContainer.color = Color.white;
        }
    }

    public void SelectToolbarSlot(int slot)
    {
        toolbar.transform.GetChild(selectedToolbarSlot).GetComponent<UnityEngine.UI.Image>().color = Color.white;
        toolbar.transform.GetChild(selectedToolbarSlot).GetComponent<InventorySlot>().RemoveInventorySlotUpdateListener(this);
        toolbar.transform.GetChild(slot % toolbar.transform.childCount).GetComponent<UnityEngine.UI.Image>().color = new Color(0.75f, 0.75f, 0.75f);
        toolbar.transform.GetChild(slot % toolbar.transform.childCount).GetComponent<InventorySlot>().AddInventorySlotUpdateListener(this);
        selectedToolbarSlot = slot % toolbar.transform.childCount;
        UpdatePlayerItemContainer();
    }

    public void OnInventorySlotUpdate(InventorySlot inventorySlot)
    {
        UpdatePlayerItemContainer();
    }

    public void SetItem(Section section, int idx, Item item)
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
