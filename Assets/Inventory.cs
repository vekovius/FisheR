using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSize;
    public List<GameObject> inventory = new List<GameObject>();

    public void AddItem(GameObject item) 
    {
        if(inventory.Count + 1 <= maxSize && item.tag == "Item") 
        {
            Debug.Log("Item Added: " + item.name);
            inventory.Add(item);
        }
        else 
        {
            Debug.Log("Inventory Full");
        }

        item.GetComponent<ItemScript>().id = inventory.IndexOf(item);
    }

    public void RemoveItem(int id) 
    {
        inventory.RemoveAt(id); //only removes it from list the object still exists

        foreach (GameObject item in inventory) 
        {
            item.GetComponent<ItemScript>().id = inventory.IndexOf(item);
        }
    }
}
