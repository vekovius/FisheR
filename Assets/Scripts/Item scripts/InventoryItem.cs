using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SerializableEquipmentItem item;
    
    [Header("Item Info")]
    public Image image;

    public Transform parentAfterDrag;

    public void InitialiseItem(SerializableEquipmentItem newItem)
    {
        item = newItem;
        image = GetComponent<Image>();
        image.sprite = item.icon;
    }
    
    //Drag and drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}
