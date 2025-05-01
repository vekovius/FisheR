using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public float cost;
    public GameObject mask;

    public GameObject confirmationPanel;
    private Button yesButton;
    private Button noButton;

    public bool bought = false;

    public void ClickSkill() 
    {
        if (!bought)
        {
            confirmationPanel.SetActive(true);
            //add in switches text to show about skill

            yesButton = GameObject.FindGameObjectWithTag("YesButton").GetComponent<Button>();
            noButton = GameObject.FindGameObjectWithTag("NoButton").GetComponent<Button>();

            yesButton.onClick.AddListener(BuySkill);
            noButton.onClick.AddListener(Exit);

            Debug.Log("Opened Confirmation Panel");
        }
        else 
        {
            Debug.Log("Show about skill");
        }
    }

    public void BuySkill() 
    {

        //if have money then skill
        mask.SetActive(false);

        Debug.Log("Bought skill");
        bought = true;
        yesButton.onClick.RemoveListener(BuySkill);
        noButton.onClick.RemoveListener(Exit);
        confirmationPanel.SetActive(false);
    }

    public void Exit() 
    {
        Debug.Log("Exited skill");

        yesButton.onClick.RemoveListener(BuySkill);
        noButton.onClick.RemoveListener(Exit);
        confirmationPanel.SetActive(false);
    }
}
