using System;
using System.Collections.Generic;

public class EnemyTurnResolver
{
    private readonly Unit player;
    private readonly EnemyFormation formation;
    private readonly Action<string> addLog;
    private readonly MirrorCardResolver mirrorCardResolver;

    public EnemyTurnResolver(Unit player, EnemyFormation formation, Action<string> addLog)
    {
        this.player = player;
        this.formation = formation;
        this.addLog = addLog;
        mirrorCardResolver = new MirrorCardResolver(player, addLog);
    }

    public void Resolve(IReadOnlyList<CardData> playerTurnCardsToMimic)
    {
        foreach (GridEnemy enemy in formation.Enemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            if (enemy.SpecialBehavior == EnemySpecialBehavior.MirrorMiniBoss)
            {
                mirrorCardResolver.Resolve(enemy, playerTurnCardsToMimic);
                continue;
            }

            foreach (EnemyIntentData intent in enemy.Intents)
            {
                ResolveIntent(enemy, intent);
            }
        }
    }

    private void ResolveIntent(GridEnemy enemy, EnemyIntentData intent)
    {
        switch (intent.intentType)
        {
            case EnemyIntentType.Attack:
                int damageDealt = player.TakeDamage(intent.amount);
                addLog($"{enemy.Unit.UnitName} attacks for {damageDealt}.");
                break;

            case EnemyIntentType.Block:
                enemy.Unit.AddBlock(intent.amount);
                addLog($"{enemy.Unit.UnitName} gains {intent.amount} block.");
                break;

            case EnemyIntentType.ApplyStatus:
                player.ApplyStatus(intent.statusToApply, intent.amount);
                addLog($"{enemy.Unit.UnitName} applies {intent.statusToApply} {System.Math.Max(intent.amount, 1)}.");
                break;
        }
    }
}
