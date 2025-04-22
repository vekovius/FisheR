using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class SellPanel : MonoBehaviour
{
    public TextMeshProUGUI cost;
    private float value;
    public MoneyUI moneyUI;
    public Button sellButton;
    public GameObject moneyPanel;
    SellFishSlot sellFishSlot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        moneyPanel = GameObject.FindGameObjectWithTag("MoneyPanel");
        sellFishSlot = GameObject.FindGameObjectWithTag("SellFishSlot").GetComponent<SellFishSlot>();

        if (moneyUI == null)
        {
            moneyUI = FindAnyObjectByType<MoneyUI>();
        }

        if (sellButton != null)
        {
            sellButton.clicked += SellItem;
        }
        else
        {
            Debug.LogError("Sell button is not assigned in the inspector.");
        }

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
        if (sellFishSlot == null)
        {
            Debug.LogError("SellFishSlot is null. Cannot sell.");
            return;
        }

        PlayerClass player = FindAnyObjectByType<PlayerClass>();

        if (player != null)
        {
            if (moneyUI != null)
            {
                player.gold += Mathf.RoundToInt(value);
                moneyUI.money = player.gold;
                moneyUI.SetMoneyText(player.gold);
            }
            sellFishSlot = null;
            cost.text = "0$";
        }
        else
        {
            Debug.LogError("Could not find PlayerClass to add money");
        }
    }

}
