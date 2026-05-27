public class CombatPlayValidator
{
    public bool CanPlay(
        CombatFlowState state,
        DeckRuntime deck,
        int currentEnergy,
        CardInstance card,
        GridEnemy selectedEnemy,
        out string failureReason)
    {
        failureReason = null;

        if (state != CombatFlowState.Combat)
        {
            failureReason = "Cards can only be played during combat.";
            return false;
        }

        if (card == null || card.CardData == null)
        {
            failureReason = "That card is missing data.";
            return false;
        }

        if (deck == null || !deck.Hand.Contains(card))
        {
            failureReason = $"{card.CardName} is not in hand.";
            return false;
        }

        if (currentEnergy < card.EnergyCost)
        {
            failureReason = $"Not enough energy for {card.CardName}.";
            return false;
        }

        if (TargetResolver.RequiresEnemySelection(card))
        {
            if (selectedEnemy == null || !selectedEnemy.IsAlive)
            {
                failureReason = $"{card.CardName} needs a living enemy target.";
                return false;
            }
        }

        return true;
    }
}
