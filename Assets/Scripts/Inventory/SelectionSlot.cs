using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    private Image image;
    private int id;
    private ISlotSelectListener slotSelectListener;

    public void Initialize()
    {
        image = transform.Find("Image").GetComponent<Image>();
    }

    public void SetSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            image.sprite = null;
            image.color = new Color(255, 255, 255, 0);
        }
        else
        {
            image.sprite = sprite;
            image.color = Color.white;
        }
    }

    public void SetSelectionID(int id)
    {
        this.id = id;
    }

    public void SetListener(ISlotSelectListener slotSelectListener)
    {
        this.slotSelectListener = slotSelectListener;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (image.sprite == null || slotSelectListener == null)
            return;
        slotSelectListener.OnSlotSelected(id);
    }

}
