using System;

public class CardResolver
{
    private readonly Unit player;
    private readonly EnemyFormation formation;
    private readonly DeckRuntime deck;
    private readonly Action<string> addLog;

    public CardResolver(Unit player, EnemyFormation formation, DeckRuntime deck, Action<string> addLog)
    {
        this.player = player;
        this.formation = formation;
        this.deck = deck;
        this.addLog = addLog;
    }

    public void Resolve(CardInstance card, GridEnemy selectedEnemy)
    {
        if (card == null || card.CardData == null)
        {
            return;
        }

        CardResolutionContext context = new CardResolutionContext();

        foreach (CardActionData action in card.CardData.actions)
        {
            if (!ConditionPasses(action, context))
            {
                continue;
            }

            ResolveAction(action, selectedEnemy, context);
        }
    }

    private bool ConditionPasses(CardActionData action, CardResolutionContext context)
    {
        switch (action.conditionType)
        {
            case ConditionType.None:
                return true;

            case ConditionType.LastDamageWasLethal:
                return context.HasDamageResult && context.LastDamageWasLethal;

            case ConditionType.LastDamageWasNotLethal:
                return context.HasDamageResult && !context.LastDamageWasLethal;

            default:
                return false;
        }
    }

    private void ResolveAction(CardActionData action, GridEnemy selectedEnemy, CardResolutionContext context)
    {
        switch (action.actionType)
        {
            case CardActionType.Damage:
                foreach (Unit target in TargetResolver.GetTargets(action.target, player, formation, selectedEnemy))
                {
                    bool wasAlive = !target.IsDead;
                    int damageDealt = target.TakeDamage(action.amount);
                    context.HasDamageResult = true;
                    context.LastDamageWasLethal = wasAlive && target.IsDead && damageDealt > 0;
                    addLog($"{target.UnitName} takes {damageDealt} {action.damageType} damage.");
                }
                break;

            case CardActionType.Block:
                foreach (Unit target in TargetResolver.GetTargets(action.target, player, formation, selectedEnemy))
                {
                    target.AddBlock(action.amount);
                    addLog($"{target.UnitName} gains {action.amount} block.");
                }
                break;

            case CardActionType.Heal:
                foreach (Unit target in TargetResolver.GetTargets(action.target, player, formation, selectedEnemy))
                {
                    target.Heal(action.amount);
                    addLog($"{target.UnitName} heals {action.amount}.");
                }
                break;

            case CardActionType.Draw:
                deck.DrawCards(action.amount);
                addLog($"Drew {action.amount} card(s).");
                break;

            case CardActionType.ApplyStatus:
                addLog($"{action.statusToApply} is not implemented yet.");
                break;
        }
    }

    private class CardResolutionContext
    {
        public bool HasDamageResult { get; set; }
        public bool LastDamageWasLethal { get; set; }
    }
}
