using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TMP_Text moneyText;
    public GameObject moneyPanel;
    private GameObject inventory;

   
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("InventoryManager");
        SetMoneyText();
    }

    public void SetMoneyText()
    {
        moneyText.text = inventory.GetComponent<InventoryManager>().gold + "$";
    }
}
