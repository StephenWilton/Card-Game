using UnityEngine;
using TMPro; // TextMeshPro namespace for text handling
using System.Collections.Generic;

// MonoBehaviour is the base class for all Unity scripts
// it is used to create components that can be attached to GameObjects in the Unity Editor
public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName = "Unit";
    [SerializeField] private int unitMaxHealth = 10;
    [SerializeField] private int unitCurrentHealth = 10;
    [SerializeField] private int unitBlock = 0;

    private readonly Dictionary<StatusType, int> statuses = new Dictionary<StatusType, int>();

    public string UnitName => unitName;
    public int UnitMaxHealth => unitMaxHealth;
    public int UnitCurrentHealth => unitCurrentHealth;
    public int UnitBlock => unitBlock;

    public bool IsDead => unitCurrentHealth <= 0;

    public void Initialize(string newUnitName, int maxHealth)
    {
        unitName = newUnitName;
        unitMaxHealth = Mathf.Max(maxHealth, 1);
        unitCurrentHealth = unitMaxHealth;
        unitBlock = 0;
        statuses.Clear();
    }

    public void ResetForCombat()
    {
        unitCurrentHealth = unitMaxHealth;
        unitBlock = 0;
        statuses.Clear();
    }

    public int TakeDamage(int damageAmount)
    {
        int safeDamageAmount = Mathf.Max(damageAmount, 0);
        int damageAfterBlock = Mathf.Max(safeDamageAmount - unitBlock, 0);
        unitCurrentHealth -= damageAfterBlock;
        unitCurrentHealth = Mathf.Max(unitCurrentHealth, 0);
        unitBlock = Mathf.Max(unitBlock - safeDamageAmount, 0);

        if (IsDead)
        {
            Die();
        }

        return damageAfterBlock;
    }

    public void Heal(int healAmount)
    {
        unitCurrentHealth = Mathf.Min(unitCurrentHealth + healAmount, unitMaxHealth);
    }

    public void AddBlock(int blockAmount)
    {
        unitBlock += Mathf.Max(blockAmount, 0);
    }

    public void RemoveBlock(int blockAmount)
    {
        unitBlock = Mathf.Max(unitBlock - Mathf.Max(blockAmount, 0), 0);
    }

    public void ClearBlock()
    {
        unitBlock = 0;
    }

    public void ApplyStatus(StatusType statusType, int amount)
    {
        if (statusType == StatusType.None)
        {
            return;
        }

        int safeAmount = Mathf.Max(amount, 1);

        if (!statuses.ContainsKey(statusType))
        {
            statuses[statusType] = 0;
        }

        statuses[statusType] += safeAmount;
    }

    public bool HasStatus(StatusType statusType)
    {
        return statusType != StatusType.None &&
               statuses.TryGetValue(statusType, out int amount) &&
               amount > 0;
    }

    public int GetStatusAmount(StatusType statusType)
    {
        return statuses.TryGetValue(statusType, out int amount) ? amount : 0;
    }

    public string GetStatusSummary()
    {
        if (statuses.Count == 0)
        {
            return "";
        }

        string summary = "";

        foreach (KeyValuePair<StatusType, int> status in statuses)
        {
            if (status.Value <= 0)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(summary))
            {
                summary += "  ";
            }

            summary += $"{status.Key} {status.Value}";
        }

        return summary;
    }
    
    public void Die()
    {
        // Handle unit death (e.g., play animation, remove from game, etc.)
        Debug.Log($"{gameObject.name} has died.");
    }

}
