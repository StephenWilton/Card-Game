public class RestSiteService
{
    public bool CanUpgrade(DeckRuntime deck, int deckIndex)
    {
        return deckIndex >= 0 &&
               deckIndex < deck.Deck.Count &&
               deck.Deck[deckIndex].CanUpgrade;
    }

    public bool CanCorrupt(RunState runState, int deckIndex, int influenceCost)
    {
        return runState != null &&
               deckIndex >= 0 &&
               deckIndex < runState.Deck.Deck.Count &&
               runState.PatronInfluence >= influenceCost &&
               runState.Deck.Deck[deckIndex].CanCorrupt;
    }

    public bool TryUpgrade(DeckRuntime deck, int deckIndex, out string message)
    {
        message = null;

        if (!CanUpgrade(deck, deckIndex))
        {
            return false;
        }

        CardInstance card = deck.Deck[deckIndex];
        string oldName = card.CardName;
        card.ReplaceData(card.CardData.upgradeCardData);
        message = $"{oldName} upgraded to {card.CardName}.";
        return true;
    }

    public bool TryCorrupt(RunState runState, int deckIndex, int influenceCost, out string message)
    {
        message = null;

        if (!CanCorrupt(runState, deckIndex, influenceCost))
        {
            return false;
        }

        CardInstance card = runState.Deck.Deck[deckIndex];
        string oldName = card.CardName;

        if (!runState.TrySpendPatronInfluence(influenceCost))
        {
            return false;
        }

        card.ReplaceData(card.CardData.corruptedCardData);
        message = $"{oldName} becomes {card.CardName}.";
        return true;
    }
}
