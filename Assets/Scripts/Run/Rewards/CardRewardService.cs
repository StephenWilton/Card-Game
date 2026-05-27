using System.Collections.Generic;
using UnityEngine;

public class CardRewardService
{
    public CardData PickReward(HeroClassData heroClass, IReadOnlyList<CardData> fallbackRewards)
    {
        List<CardData> rewardPool = new List<CardData>();

        if (heroClass != null)
        {
            AddCards(rewardPool, heroClass.rewardPool);
        }

        if (rewardPool.Count == 0)
        {
            AddCards(rewardPool, fallbackRewards);
        }

        if (rewardPool.Count == 0)
        {
            return null;
        }

        return rewardPool[Random.Range(0, rewardPool.Count)];
    }

    private void AddCards(List<CardData> target, IReadOnlyList<CardData> cards)
    {
        if (cards == null)
        {
            return;
        }

        foreach (CardData card in cards)
        {
            if (card != null)
            {
                target.Add(card);
            }
        }
    }
}
