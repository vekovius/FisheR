using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public bool useNextSkill = false;
    public GameObject nextSkill;
    public bool unlocked = false;

    public string aboutSkill;

    public float cost;
    public GameObject mask;

    public GameObject confirmationPanel;
    public TextMeshProUGUI text;

    private Button yesButton;
    private Button noButton;

    public bool bought = false;

    public void ClickSkill() 
    {
        if (unlocked)
        {
            if (!bought)
            {
                confirmationPanel.SetActive(true);
                //add in switches text to show about skill

                yesButton = GameObject.FindGameObjectWithTag("YesButton").GetComponent<Button>();
                noButton = GameObject.FindGameObjectWithTag("NoButton").GetComponent<Button>();
                text.text = aboutSkill;

                yesButton.onClick.AddListener(BuySkill);
                noButton.onClick.AddListener(Exit);

                Debug.Log("Opened Confirmation Panel");
            }
            else
            {
                confirmationPanel.SetActive(true);

                yesButton.gameObject.SetActive(false);
                noButton.onClick.AddListener(Exit);
                text.text = aboutSkill;

                Debug.Log("Show about skill");
            }
        }
        else 
        {
             confirmationPanel.SetActive(true);
            yesButton = GameObject.FindGameObjectWithTag("YesButton").GetComponent<Button>();
            noButton = GameObject.FindGameObjectWithTag("NoButton").GetComponent<Button>();


            yesButton.gameObject.SetActive(false);
             noButton.onClick.AddListener(Exit);
             text.text = aboutSkill;

             Debug.Log("Show about skill");
        }
    }

    public void BuySkill() 
    {
        if (GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>().gold > cost)
        {
            GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>().gold -= cost;
            mask.SetActive(false);

            if (useNextSkill)
            {
                nextSkill.GetComponent<SkillTree>().unlocked = true;
            }

            //whatever the skill does i guess just copy the buySkill and name it BuySkillBuff or whatever

            Debug.Log("Bought skill");
            bought = true;
            yesButton.onClick.RemoveListener(BuySkill);
            noButton.onClick.RemoveListener(Exit);
            confirmationPanel.SetActive(false);
        }
        else 
        {
            Exit();
        }
    }

    public void Exit() 
    {
        Debug.Log("Exited skill");

        yesButton.gameObject.SetActive(true);
        yesButton.onClick.RemoveListener(BuySkill);
        noButton.onClick.RemoveListener(Exit);
        confirmationPanel.SetActive(false);
    }
}
