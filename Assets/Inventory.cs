using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryPanel;
    public int maxSize;
    public List<GameObject> inventory = new List<GameObject>();

    public Transform[] sortPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel.activeSelf == true)
            {
                inventoryPanel.SetActive(false);
            }
            else 
            {
                inventoryPanel.SetActive(true);
            }
        }
    }

    public void AddItem(GameObject item) 
    {
        if(inventory.Count + 1 <= maxSize && item.tag == "Item") 
        {
            Debug.Log("Item Added: " + item.name);
            inventory.Add(item);

            item.GetComponent<ItemScript>().id = inventory.IndexOf(item);
            item.transform.position = sortPoint[inventory.IndexOf(item)].position;
            item.transform.localScale = sortPoint[inventory.IndexOf(item)].localScale;
            item.transform.parent = sortPoint[inventory.IndexOf(item)];
        }
        else 
        {
            Debug.Log("Inventory Full");
        }
    }

    public void RemoveItem(int id) 
    {
        inventory.RemoveAt(id); //only removes it from list the object still exists

        foreach (GameObject item in inventory) 
        {
            item.GetComponent<ItemScript>().id = inventory.IndexOf(item);
            item.transform.position = sortPoint[inventory.IndexOf(item)].position;
            item.transform.parent = sortPoint[inventory.IndexOf(item)];
        }
    }

    public void UnlockInventory(int amount) 
    {
        for (int i = 0; i < amount; i++)
        {
            sortPoint[maxSize].GetComponent<SortPoint>().Unlock();
            maxSize++;
        }
    }
}
