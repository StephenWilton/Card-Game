using System;
using UnityEngine;

[Serializable]
public class CardInstance
{
    [SerializeField] private string instanceId;
    [SerializeField] private CardData cardData;

    public string InstanceId => instanceId;
    public CardData CardData => cardData;
    public string CardName => cardData != null ? cardData.cardName : "Missing Card";
    public int EnergyCost => cardData != null ? cardData.energyCost : 0;
    public bool IsCorrupted => cardData != null && cardData.isCorrupted;
    public bool CanUpgrade => cardData != null && cardData.upgradeCardData != null;
    public bool CanCorrupt => cardData != null && cardData.corruptedCardData != null;

    public CardInstance(CardData cardData)
    {
        instanceId = Guid.NewGuid().ToString("N");
        this.cardData = cardData;
    }

    public void ReplaceData(CardData replacement)
    {
        if (replacement == null)
        {
            return;
        }

        cardData = replacement;
    }
}
