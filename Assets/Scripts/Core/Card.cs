using System.Collections.Generic;

public class Card
{
    private readonly CardData cardData;

    public CardData CardData => cardData;
    public string CardName => cardData.CardName;
    public string CardDescription => cardData.CardDescription;
    public int EnergyCost => cardData.EnergyCost;
    public int Amount => cardData.Amount;
    public CardType CardType => cardData.CardType;
    public TargetType TargetType => cardData.TargetType;

    public Card(CardData cardData)
    {
        this.cardData = cardData;
    }

}
