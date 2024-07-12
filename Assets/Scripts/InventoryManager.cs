using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public enum Section
    {
        TOOLBAR,
        MAIN
    }

    public GameObject overlay;
    public GameObject toolbar;
    public GameObject main;

    private CanvasGroup overlayGroup;

    // Start is called before the first frame update
    void Start()
    {
        overlayGroup = overlay.GetComponent<CanvasGroup>();
        for (int i = 0; i < 20; i++)
        {
            SetItem(Section.MAIN, i, new Item(i % 2 == 0 ? Item.Type.GRASS : Item.Type.STONE, i + 1));
        }
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventoryOpen();
        }
    }
}
