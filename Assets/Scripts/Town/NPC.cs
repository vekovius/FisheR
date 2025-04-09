using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NPC : MonoBehaviour
{
    public string npcName = "NPC";
    [TextArea(3, 10)]
    public string dialogueText = "Hello, traveler!";

    private bool isPlayerInRange = false;
    public GameObject dialogueUI;
    public TMP_Text dialogueTextComponent;
    
    [SerializeField] GameObject Player;
    [SerializeField] GameObject CharacterNP;

    private Rigidbody2D RB;
    private Rigidbody2D RBN;

    void Start()
    {
        RB = Player.GetComponent<Rigidbody2D>();
        RBN = CharacterNP.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueUI.activeSelf)
            {
                dialogueUI.SetActive(false);
            }
            else
            {
                dialogueTextComponent.text = npcName + ": " + dialogueText;
                dialogueUI.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueUI.SetActive(false);
        }
    }
}