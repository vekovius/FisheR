using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public float cost;
    public GameObject mask;

    public GameObject confirmationPanel;
    public Button yesButton;
    public Button noButton;

    public bool bought = false;

    public void ClickSkill() 
    {
        if (!bought)
        {
            confirmationPanel.SetActive(true);
            yesButton.onClick.AddListener(BuySkill);
            noButton.onClick.AddListener(Exit);
        }

    }

    public void BuySkill() 
    {
        //if have money then skill
        mask.SetActive(false);


        yesButton.onClick.RemoveListener(BuySkill);
        noButton.onClick.RemoveListener(Exit);
        confirmationPanel.SetActive(false);
    }

    public void Exit() 
    {
        yesButton.onClick.RemoveListener(BuySkill);
        noButton.onClick.RemoveListener(Exit);
        confirmationPanel.SetActive(false);
    }
}
