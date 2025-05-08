using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bestiary : MonoBehaviour
{
    public static Bestiary bestiary { get; private set; }
    public GameObject[] entries;
    public Button[] buttons;
    private TMP_Text[] buttonTexts;
    private string[] fishNames;
    public TMP_Text ageText;
    public TMP_Text rarityText;
    public TMP_Text counterText;
    public TMP_Text sizeText;
    public GameObject statPanel;

    private Dictionary<int, FishCatchData> _catchRecords = new Dictionary<int, FishCatchData>();

    [System.Serializable]
    public class FishCatchData
    {
        public int timesCaught;
        public float largestSizeCaught;
        public Rarity rarity;
        public float age;
    }
    private Dictionary<Rarity, int> _rarity = new Dictionary<Rarity, int>()
{
    {Rarity.Common, 0},
    {Rarity.Uncommon, 1},
    {Rarity.Rare, 2},
    {Rarity.Epic, 3}
};

    private void Start()
    {
        if (bestiary == null)
        {
            bestiary = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        RenameUnknown();

    }
    private void RenameUnknown()
    {
        buttonTexts = new TMP_Text[buttons.Length];
        fishNames = new string[buttons.Length]; // Initialize storage array

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            buttonTexts[i] = buttons[i].GetComponentInChildren<TMP_Text>();

            if (buttonTexts[i] != null)
            {
                // Save original text before replacing with "?????"
                fishNames[i] = buttonTexts[i].text;
                buttonTexts[i].text = "?????";
            }
        }
    }
    public void RecordFishCaught(FishAI fish)
    {
        int fishID = fish.fishData.bestiaryID;

        // Initialize entry if this fish hasn't been caught before
        if (!_catchRecords.ContainsKey(fishID))
        {
            _catchRecords[fishID] = new FishCatchData();
        }
        // Update stats
        _catchRecords[fishID].timesCaught++;
        if (_catchRecords[fishID].largestSizeCaught < fish.transform.localScale.x)
        {
            _catchRecords[fishID].largestSizeCaught = fish.transform.localScale.x;
        }
        // Track highest rarity
        if (_rarity[fish.fishData.rarity] > _rarity[_catchRecords[fishID].rarity])
        {
            _catchRecords[fishID].rarity = fish.fishData.rarity;
        }
        if (_catchRecords[fishID].age < fish.age)
        {
            _catchRecords[fishID].age = fish.age;
        }
    }
    public FishCatchData GetCatchData(int bestiaryID)
    {
        return _catchRecords.ContainsKey(bestiaryID) ? _catchRecords[bestiaryID] : new FishCatchData();
    }

    public void EnableEntry(int id)
    {
        statPanel.SetActive(false);
        for (int i = 0; i < entries.Length; i++)
        {
            entries[i].SetActive(false);
        }
        entries[id].SetActive(true);

    }
    // Unlock a fish entry by FishAI reference
    public void UnlockFishEntry(FishAI fish)
    {
        int id = fish.fishData.bestiaryID;
        if (id >= 0 && id < buttons.Length)
        {
            buttons[id].interactable = true;
            buttonTexts[id].text = fishNames[id];
        }
        

    }
    public void UpdateStatsEntry(int fishID)
    {
        
        rarityText.text = $"<b>RARITY:</b> {_catchRecords[fishID].rarity}";
        rarityText.color = GetRarityColor(_catchRecords[fishID].rarity);

        ageText.text = $"<b>AGE:</b> {_catchRecords[fishID].age:F1} mins";

        sizeText.text = $"<b>SIZE:</b> {GetSizeCategory(_catchRecords[fishID].largestSizeCaught)} " +
                        $"| Record: {_catchRecords[fishID].largestSizeCaught:F1}x";

        counterText.text = $"<b>CAUGHT:</b> {_catchRecords[fishID].timesCaught}";
        statPanel.SetActive(true);
    }
    public void EnableDiscriptionPanel(GameObject gameObject)
    {
        statPanel.SetActive(false);
        gameObject.SetActive(true);
    }
    private string GetSizeCategory(float size)
    {
        return size switch
        {
            < 0.3f => "Small",
            < 0.6f => "Medium",
            < 0.8f => "Large",
            _ => "XLarge"
        };
    }
    private Color GetRarityColor(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => Color.gray,
            Rarity.Uncommon => Color.green,
            Rarity.Rare => Color.blue,
            Rarity.Epic => Color.magenta,
            _ => Color.yellow
        };

    }
}
