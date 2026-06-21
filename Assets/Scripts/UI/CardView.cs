using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [Header("Card Info")]
    [SerializeField] private CardData cardData;
    [SerializeField] private TMP_Text cardName;

    [SerializeField] private TMP_Text cardDescription;
    [SerializeField] private TMP_Text energyCost;
    private Card card;

    private void Start()
    {
        if (cardData == null)
        {
            return;
        }

        SetCardInfo(new Card(cardData));
    }

    public void SetCardInfo(Card card)
    {
        cardName.text = card.CardName;
        cardDescription.text = card.CardDescription;
        energyCost.text = card.EnergyCost.ToString();

    }
}