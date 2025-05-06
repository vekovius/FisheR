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
    private GameObject inventory;

    public AudioSource yesClip;
    public AudioSource noClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        yesClip = GameObject.FindGameObjectWithTag("YesSound").GetComponent<AudioSource>();
        noClip = GameObject.FindGameObjectWithTag("NoSound").GetComponent<AudioSource>();

        moneyPanel = GameObject.FindGameObjectWithTag("MoneyPanel");
        sellPanelObject = GameObject.FindGameObjectWithTag("SellPanel");
        inventory = GameObject.FindGameObjectWithTag("InventoryManager");
        cost = moneyPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (moneyUI == null)
        {
            moneyUI = FindAnyObjectByType<MoneyUI>();
        }

        sellButton = GameObject.FindGameObjectWithTag("SellButton");

        sellFishSlot = GameObject.FindGameObjectWithTag("SellFishSlot").GetComponent<SellFishSlot>();

    }

    public void Display(GameObject item)
    {
        //item.transform.position = point.TransformPoint(point.transform.position

    }

    public void SellItem()
    {
        if (sellFishSlot == null || sellFishSlot.gameObject.GetComponentInChildren<InventoryItem>().itemFish == null)
        {
            noClip.Play();
            Debug.LogError("SellFishSlot is null. Cannot sell.");
            return;
        }
        value = sellFishSlot.gameObject.GetComponentInChildren<InventoryItem>().itemFish.value;
        

        Debug.Log("Selling item: " + sellFishSlot.gameObject.name);
        Debug.Log("Item value: " + value);

        InventoryItem fishItem = sellFishSlot.GetComponentInChildren<InventoryItem>();
        if (fishItem == null)
        {
            noClip.Play();
            Debug.LogError("No fish item found in SellFishSlot.");
            return;
        }

        if (PlayerClass.instance != null)
        {
            if (moneyUI != null)
            {
                yesClip.Play();
                inventory.GetComponent<InventoryManager>().gold += Mathf.RoundToInt(value);
                //PlayerClass.instance.gold += Mathf.RoundToInt(value);
                moneyUI.SetMoneyText();
            }
            Destroy(fishItem.gameObject);

            currentMoney.text = inventory.GetComponent<InventoryManager>().gold + "$";
        }
        else
        {
            Debug.LogError("Could not find PlayerClass to add money");
        }
    }

}
