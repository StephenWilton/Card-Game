using System;
using System.Collections.Generic;

public class MirrorCardResolver
{
    private readonly Unit player;
    private readonly Action<string> addLog;

    public MirrorCardResolver(Unit player, Action<string> addLog)
    {
        this.player = player;
        this.addLog = addLog;
    }

    public void Resolve(GridEnemy mirrorEnemy, IReadOnlyList<CardData> cardsToMirror)
    {
        if (mirrorEnemy == null || mirrorEnemy.Unit == null || !mirrorEnemy.IsAlive)
        {
            return;
        }

        if (!mirrorEnemy.HasMirrorObserved)
        {
            mirrorEnemy.MarkMirrorObserved();
            addLog($"{mirrorEnemy.Unit.UnitName} does nothing. It watches.");
            return;
        }

        if (cardsToMirror == null || cardsToMirror.Count == 0)
        {
            addLog($"{mirrorEnemy.Unit.UnitName} mirrors your hesitation.");
            return;
        }

        foreach (CardData card in cardsToMirror)
        {
            if (card == null)
            {
                continue;
            }

            addLog($"{mirrorEnemy.Unit.UnitName} mimics {card.cardName}.");
            ResolveMirroredCard(mirrorEnemy, card);

            if (player.IsDead)
            {
                return;
            }
        }
    }

    private void ResolveMirroredCard(GridEnemy mirrorEnemy, CardData card)
    {
        MirrorResolutionContext context = new MirrorResolutionContext();

        foreach (CardActionData action in card.actions)
        {
            if (!ConditionPasses(mirrorEnemy, action, context))
            {
                continue;
            }

            ResolveMirroredAction(mirrorEnemy, action, context);
        }
    }

    private bool ConditionPasses(GridEnemy mirrorEnemy, CardActionData action, MirrorResolutionContext context)
    {
        switch (action.conditionType)
        {
            case ConditionType.None:
                return true;

            case ConditionType.LastDamageWasLethal:
                return context.HasDamageResult && context.LastDamageWasLethal;

            case ConditionType.LastDamageWasNotLethal:
                return context.HasDamageResult && !context.LastDamageWasLethal;

            case ConditionType.PlayerHasStatus:
                return mirrorEnemy.Unit.HasStatus(action.requiredStatus);

            case ConditionType.EnemyHasStatus:
                return player.HasStatus(action.requiredStatus);

            default:
                return false;
        }
    }

    private void ResolveMirroredAction(GridEnemy mirrorEnemy, CardActionData action, MirrorResolutionContext context)
    {
        switch (action.actionType)
        {
            case CardActionType.Damage:
                foreach (Unit target in GetMirroredTargets(mirrorEnemy, action.target))
                {
                    bool wasAlive = !target.IsDead;
                    int damageDealt = target.TakeDamage(action.amount);
                    context.HasDamageResult = true;
                    context.LastDamageWasLethal = wasAlive && target.IsDead && damageDealt > 0;
                    addLog($"{target.UnitName} takes {damageDealt} mirrored {action.damageType} damage.");
                }
                break;

            case CardActionType.Block:
                foreach (Unit target in GetMirroredTargets(mirrorEnemy, action.target))
                {
                    target.AddBlock(action.amount);
                    addLog($"{target.UnitName} gains {action.amount} mirrored block.");
                }
                break;

            case CardActionType.Heal:
                foreach (Unit target in GetMirroredTargets(mirrorEnemy, action.target))
                {
                    target.Heal(action.amount);
                    addLog($"{target.UnitName} heals {action.amount} through the mirror.");
                }
                break;

            case CardActionType.ApplyStatus:
                foreach (Unit target in GetMirroredTargets(mirrorEnemy, action.target))
                {
                    target.ApplyStatus(action.statusToApply, action.amount);
                    addLog($"{target.UnitName} gains mirrored {action.statusToApply} {Math.Max(action.amount, 1)}.");
                }
                break;

            case CardActionType.Draw:
                addLog($"{mirrorEnemy.Unit.UnitName} cannot mirror card draw.");
                break;
        }
    }

    private List<Unit> GetMirroredTargets(GridEnemy mirrorEnemy, CardTarget target)
    {
        List<Unit> targets = new List<Unit>();

        switch (target)
        {
            case CardTarget.Player:
                targets.Add(mirrorEnemy.Unit);
                break;

            case CardTarget.Enemy:
            case CardTarget.AllEnemies:
            case CardTarget.FirstRow:
            case CardTarget.BackRow:
            case CardTarget.PierceColumn:
                targets.Add(player);
                break;

            case CardTarget.Both:
            case CardTarget.AllUnits:
                targets.Add(mirrorEnemy.Unit);
                targets.Add(player);
                break;
        }

        return targets;
    }

    private class MirrorResolutionContext
    {
        public bool HasDamageResult { get; set; }
        public bool LastDamageWasLethal { get; set; }
    }
}
