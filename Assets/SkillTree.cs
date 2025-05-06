using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public string skillId;

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

    public AudioSource yesClip;
    public AudioSource noClip;

    public GameObject moneyPanel;

    private void Start()
    {
        yesClip = GameObject.FindGameObjectWithTag("YesSound").GetComponent<AudioSource>();
        noClip = GameObject.FindGameObjectWithTag("NoSound").GetComponent<AudioSource>();

        moneyPanel = GameObject.FindGameObjectWithTag("MoneyPanel");
    }

    public void ClickSkill() 
    {
        yesClip.Play();

        if (unlocked)
        {
            if (!bought)
            {
                confirmationPanel.SetActive(true);
                //add in switches text to show about skill

                yesButton = GameObject.FindGameObjectWithTag("YesButton").GetComponent<Button>();
                noButton = GameObject.FindGameObjectWithTag("NoButton").GetComponent<Button>();
                text.text = aboutSkill + "(" + cost + ")";

                yesButton.onClick.AddListener(BuySkill);
                noButton.onClick.AddListener(Exit);

                Debug.Log("Opened Confirmation Panel");
            }
            else
            {
                confirmationPanel.SetActive(true);

                yesButton.gameObject.SetActive(false);
                noButton.onClick.AddListener(Exit);
                text.text = aboutSkill + "(" + cost + ")";

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
             text.text = aboutSkill + "(" + cost + ")";

            Debug.Log("Show about skill");
        }
    }

    public void BuySkill()
    {
        if (GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>().gold >= cost)
        {
            yesClip.Play();

            GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>().gold -= cost;
            mask.SetActive(false);

            moneyPanel.GetComponent<MoneyUI>().SetMoneyText();

            if (useNextSkill)
            {
                nextSkill.GetComponent<SkillTree>().unlocked = true;
            }

            if (skillId == "Buff")
            {
                HookedState.tensionDecreaseRate += 7f;
            }
            else if (skillId == "Fire")
            {
                ParticleScriptChecker.fire = true;
            }
            else if (skillId == "IceCube")
            {
                ParticleScriptChecker.ice = true;
            }
            else if (skillId == "Lightning")
            {
                ParticleScriptChecker.lightning = true;
            }
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
        noClip.Play();
        Debug.Log("Exited skill");

        yesButton.gameObject.SetActive(true);
        yesButton.onClick.RemoveListener(BuySkill);
        noButton.onClick.RemoveListener(Exit);
        confirmationPanel.SetActive(false);
    }
}
