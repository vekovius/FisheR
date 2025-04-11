using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{

    public SerializableEquipmentItem itemData;
    public SerializableFishItem itemFish;

    [Header("Item Info")]
    public Image image;
    public Transform parentAfterDrag;

    public void InitializeItem(SerializableEquipmentItem newItem)
    {
        itemData = newItem;
        image = GetComponent<Image>();
        image.sprite = itemData.icon;
    }
    public void InitializeItem(SerializableFishItem newItem)
    {
        Debug.Log($"Initializing item: {newItem}");
        Debug.Log($"{newItem.fishName} {newItem.icon}");
        itemFish = newItem;
        image = GetComponent<Image>();
        image.sprite = itemFish.icon;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager inventory = FindFirstObjectByType<InventoryManager>();
            if (inventory == null) return;

            bool isInEquipmentSlot = transform.parent.CompareTag("EquipmentSlot");

            if (isInEquipmentSlot)
            {
                bool success = inventory.TryAddItemToInventorySlot(gameObject);
                if (success)
                {
                    EquipmentManager.Instance.UnequipItem(itemData.equipmentType);
                    transform.SetParent(parentAfterDrag);
                }
            }
            else
            {
                EquipmentManager.Instance.EquipItem(itemData);

                EquipmentSlot targetSlot = EquipmentManager.Instance.GetSlotForType(itemData.equipmentType);
                if (targetSlot == null)
                {
                    Debug.Log("No slot found for this item type.");
                    return;
                }

                // If something is already in the slot, move it back to inventory
                if (targetSlot.transform.childCount > 0)
                {
                    Debug.Log("Slot already occupied, moving item back to inventory.");
                    InventoryItem previousItem = targetSlot.GetComponentInChildren<InventoryItem>();
                    if (previousItem != null)
                    {
                        inventory.TryAddItemToInventorySlot(previousItem.gameObject);
                    }
                }

                //Always set this item as child of target slot
                parentAfterDrag = targetSlot.transform;
                transform.SetParent(targetSlot.transform);
                transform.localPosition = Vector3.zero;
            }
        }
    }
}
