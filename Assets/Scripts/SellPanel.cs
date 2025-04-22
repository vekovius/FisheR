using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class SellPanel : MonoBehaviour
{
    public TextMeshProUGUI cost;
    public TextMeshProUGUI currentMoney;
    private float value;
    public MoneyUI moneyUI;
    public GameObject sellButton;
    public GameObject moneyPanel;
    public GameObject sellPanelObject;
    public SellFishSlot sellFishSlot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        moneyPanel = GameObject.FindGameObjectWithTag("MoneyPanel");
        sellPanelObject = GameObject.FindGameObjectWithTag("SellPanel");
        cost = moneyPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (moneyUI == null)
        {
            moneyUI = FindAnyObjectByType<MoneyUI>();
        }

        sellButton = GameObject.FindGameObjectWithTag("SellButton");

        sellFishSlot = GameObject.FindGameObjectWithTag("SellFishSlot").GetComponent<SellFishSlot>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Display(GameObject item)
    {
        //item.transform.position = point.TransformPoint(point.transform.position

    }

    public void SellItem()
    {
        if (sellFishSlot == null || sellFishSlot.gameObject.GetComponentInChildren<InventoryItem>().itemFish == null)
        {
            Debug.LogError("SellFishSlot is null. Cannot sell.");
            return;
        }
        value = sellFishSlot.gameObject.GetComponentInChildren<InventoryItem>().itemFish.value;
        

        Debug.Log("Selling item: " + sellFishSlot.gameObject.name);
        Debug.Log("Item value: " + value);

        InventoryItem fishItem = sellFishSlot.GetComponentInChildren<InventoryItem>();
        if (fishItem == null)
        {
            Debug.LogError("No fish item found in SellFishSlot.");
            return;
        }

        if (PlayerClass.instance != null)
        {
            if (moneyUI != null)
            {
                PlayerClass.instance.gold += Mathf.RoundToInt(value);
                moneyUI.SetMoneyText();
            }
            Destroy(fishItem.gameObject);
            //currentMoney.text = PlayerClass.instance.gold.ToString() + "$";
        }
        else
        {
            Debug.LogError("Could not find PlayerClass to add money");
        }
    }

}
