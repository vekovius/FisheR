using UnityEngine;

public class PlayerClass: MonoBehaviour
{
    public static PlayerClass instance;
    [SerializeField] private string playerName = "DefaultName";
    [SerializeField] private int playerMight;
    [SerializeField] private int playerDexterity;
    [SerializeField] private int playerMagica;
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private int playerExperiencePoints;
    [SerializeField] public int gold = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}
