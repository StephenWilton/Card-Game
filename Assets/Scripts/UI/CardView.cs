using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text cardName;
    
    [SerializeField] private TMP_Text cardType;

    [SerializeField] private TMP_Text cardDescription;
    [SerializeField] private TMP_Text energyCost;
    private Card card;

    public void SetCardInfo(Card card)
    {
        this.card = card;
        cardName.text = card.CardName;
        cardDescription.text = card.CardDescription;
        energyCost.text = card.EnergyCost.ToString();
        cardType.text = card.CardType.ToString();
        

    }
}