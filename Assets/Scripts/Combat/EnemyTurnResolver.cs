using System;

public class EnemyTurnResolver
{
    private readonly Unit player;
    private readonly EnemyFormation formation;
    private readonly Action<string> addLog;

    public EnemyTurnResolver(Unit player, EnemyFormation formation, Action<string> addLog)
    {
        this.player = player;
        this.formation = formation;
        this.addLog = addLog;
    }

    public void Resolve()
    {
        foreach (GridEnemy enemy in formation.Enemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            int damageDealt = player.TakeDamage(enemy.AttackDamage);
            addLog($"{enemy.Unit.UnitName} attacks for {damageDealt}.");
        }
    }
}
