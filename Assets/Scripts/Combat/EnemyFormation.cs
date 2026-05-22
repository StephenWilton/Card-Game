using System.Collections.Generic;
using UnityEngine;

public class EnemyFormation
{
    public List<GridEnemy> Enemies { get; } = new List<GridEnemy>();

    private readonly EncounterData encounterData;

    public EnemyFormation(EncounterData encounterData)
    {
        this.encounterData = encounterData;
    }

    public void Spawn()
    {
        Clear();

        if (encounterData == null)
        {
            Debug.LogError("EnemyFormation cannot spawn: no EncounterData assigned.");
            return;
        }

        foreach (EncounterEnemySlot slot in encounterData.enemies)
        {
            if (slot.enemyData == null)
            {
                continue;
            }

            GameObject enemyObject = new GameObject(slot.enemyData.enemyName);
            Unit enemyUnit = enemyObject.AddComponent<Unit>();
            enemyUnit.Initialize(slot.enemyData.enemyName, slot.enemyData.maxHealth);

            Enemies.Add(new GridEnemy(
                enemyUnit,
                Mathf.Clamp(slot.row, 0, 1),
                Mathf.Clamp(slot.column, 0, GridColumns - 1),
                Mathf.Max(slot.enemyData.attackDamage, 0)));
        }
    }

    public int GridColumns => encounterData != null ? Mathf.Max(encounterData.gridColumns, 1) : 1;

    public void Clear()
    {
        foreach (GridEnemy enemy in Enemies)
        {
            if (enemy.Unit != null)
            {
                Object.Destroy(enemy.Unit.gameObject);
            }
        }

        Enemies.Clear();
    }

    public void ClearBlock()
    {
        foreach (GridEnemy enemy in Enemies)
        {
            if (enemy.Unit != null)
            {
                enemy.Unit.ClearBlock();
            }
        }
    }

    public bool AllEnemiesDefeated()
    {
        foreach (GridEnemy enemy in Enemies)
        {
            if (enemy.IsAlive)
            {
                return false;
            }
        }

        return true;
    }

    public GridEnemy GetEnemyAt(int row, int column)
    {
        foreach (GridEnemy enemy in Enemies)
        {
            if (enemy.Row == row && enemy.Column == column)
            {
                return enemy;
            }
        }

        return null;
    }

    public void AddAllLivingEnemies(List<Unit> targets)
    {
        foreach (GridEnemy enemy in Enemies)
        {
            if (enemy.IsAlive)
            {
                targets.Add(enemy.Unit);
            }
        }
    }

    public void AddLivingEnemiesInRow(List<Unit> targets, int row)
    {
        foreach (GridEnemy enemy in Enemies)
        {
            if (enemy.IsAlive && enemy.Row == row)
            {
                targets.Add(enemy.Unit);
            }
        }
    }

    public void AddLivingEnemiesInColumn(List<Unit> targets, int column)
    {
        for (int row = 0; row <= 1; row++)
        {
            foreach (GridEnemy enemy in Enemies)
            {
                if (enemy.IsAlive && enemy.Column == column && enemy.Row == row)
                {
                    targets.Add(enemy.Unit);
                }
            }
        }
    }
}
