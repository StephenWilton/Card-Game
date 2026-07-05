using TMPro;
using UnityEngine;
using System.Collections.Generic;

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

    [Header("Cards")]
    [SerializeField] private GameObject cardPanelPrefab;
    [SerializeField] private Transform handPanel;
    private readonly List<Card> drawPile = new List<Card>();
    private readonly List<Card> hand = new List<Card>();

    private PlayerState playerState;
    private Unit playerUnit;
    private Unit enemyUnit;

    private void Start()
    {
        playerState = new PlayerState(playerData);

        playerUnit = new Unit(playerState.PlayerName, playerState.MaxHealth);
        enemyUnit = new Unit(enemyData.EnemyName, enemyData.MaxHealth);

        CreateDrawPile();
        Shuffle(drawPile);
        DrawCards(playerState.cardDraw);

        RefreshUI();
    }


    private void RefreshUI()
    {
        playerName.text = playerUnit.UnitName;
        playerHealth.text = $"{playerUnit.CurrentHealth}/{playerUnit.MaxHealth}";
        playerEnergy.text = $"{playerState.CurrentEnergy}/{playerState.MaxEnergy}";

        enemyName.text = enemyUnit.UnitName;
        enemyHealth.text = $"{enemyUnit.CurrentHealth}/{enemyUnit.MaxHealth}";
        EnemyActionData intent = enemyData.EnemyDeck[0];
        enemyIntentType.text = $"{intent.ActionType}";
        enemyIntentAmount.text = $"{intent.Amount}";
        enemyIntentName.text = $"{intent.ActionName}";


    }

    private void CreateDrawPile()
    {
        drawPile.Clear();
        hand.Clear();

        foreach (CardData cardData in playerState.Deck)
        {
            drawPile.Add(new Card(cardData));
        }
    }

    private void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DrawCard();
        }
    }

    private void DrawCard()
    {
        if (drawPile.Count == 0)
        {
            return;
        }

        Card card = drawPile[0];
        drawPile.RemoveAt(0);
        hand.Add(card);

        GameObject cardObject = Instantiate(cardPanelPrefab, handPanel);
        CardView cardView = cardObject.GetComponent<CardView>();
        cardView.SetCardInfo(card);
    }

    private void Shuffle(List<Card> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);

            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }
}