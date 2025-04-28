using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TMP_Text moneyText;
    public GameObject moneyPanel;

    public void SetMoneyText(){
        
    //    moneyText.text = PlayerClass.instance.gold.ToString();
    }
    private void Start(){
        SetMoneyText();
    }
}
