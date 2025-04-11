using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public EquipmentType slotType;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        InventoryItem inventoryItem = droppedObject.GetComponent<InventoryItem>();
        if (inventoryItem == null) return;

        if (inventoryItem.itemData.equipmentType == slotType)
        {
            EquipmentManager.Instance.EquipItem(inventoryItem.itemData);

            inventoryItem.parentAfterDrag = transform;

        }
        else
        {
            Debug.Log("Item ty pe does not match slot type.");
        }
    }
}
