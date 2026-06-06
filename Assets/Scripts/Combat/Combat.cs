using TMPro;
using UnityEngine;

public class Combat : MonoBehaviour
{

    [Header("Units")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private EnemyData enemyData;

    [Header("Player Info")]
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerHealth;
    [SerializeField] private TMP_Text playerEnergy;

    [Header("Enemy Info")]
    [SerializeField] private TMP_Text enemyName;
    [SerializeField] private TMP_Text enemyHealth;
    [SerializeField] private TMP_Text enemyIntentType;
    [SerializeField] private TMP_Text enemyIntentAmount;

    [SerializeField] private TMP_Text enemyIntentName;

    private Unit playerUnit;
    private Unit enemyUnit;

    private int playerMaxEnergy;
    private int playerCurrentEnergy;

    private void Start()
    {
        playerUnit = new Unit(playerData.PlayerName, playerData.MaxHealth);
        enemyUnit = new Unit(enemyData.EnemyName, enemyData.MaxHealth);

        playerMaxEnergy = playerData.MaxEnergy;
        playerCurrentEnergy = playerMaxEnergy;

        RefreshUI();
    }


    private void RefreshUI()
    {
        playerName.text = playerUnit.UnitName;
        playerHealth.text = $"{playerUnit.CurrentHealth}/{playerUnit.MaxHealth}";
        playerEnergy.text = $"{playerCurrentEnergy}/{playerMaxEnergy}";

        enemyName.text = enemyUnit.UnitName;
        enemyHealth.text = $"{enemyUnit.CurrentHealth}/{enemyUnit.MaxHealth}";
        EnemyActionData intent = enemyData.EnemyDeck[0];
        enemyIntentType.text = $"{intent.ActionType}";
        enemyIntentAmount.text = $"{intent.Amount}";
        enemyIntentName.text = $"{intent.ActionName}";


    }
}