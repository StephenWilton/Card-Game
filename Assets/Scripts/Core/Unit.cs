using System.Collections.Generic;

public enum StatusType
{
    None,
    Marked,
    Burned
}

public class Unit
{
    private readonly Dictionary<StatusType, int> statuses;

    private string unitName;
    private int maxHealth;
    private int currentHealth;
    private int block;

    public string UnitName => unitName;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int Block => block;
    public bool IsDead => currentHealth <= 0;

    public Unit(string unitName, int maxHealth)
    {
        this.unitName = string.IsNullOrWhiteSpace(unitName) ? "Unit" : unitName;
        this.maxHealth = System.Math.Max(maxHealth, 1);
        this.currentHealth = this.maxHealth;
        this.block = 0;
        this.statuses = new Dictionary<StatusType, int>();
    }


    public int TakeDamage(int damageAmount)
    {
        int safeDamageAmount = System.Math.Max(damageAmount, 0);
        int blockedDamage = System.Math.Min(block, safeDamageAmount);
        int damageToHealth = safeDamageAmount - blockedDamage;

        block -= blockedDamage;
        currentHealth = System.Math.Max(currentHealth - damageToHealth, 0);

        return damageToHealth;
    }

    public void Heal(int healAmount)
    {
        int safeHealAmount = System.Math.Max(healAmount, 0);
        currentHealth = System.Math.Min(currentHealth + safeHealAmount, maxHealth);
    }

    public void AddBlock(int blockAmount)
    {
        block += System.Math.Max(blockAmount, 0);
    }

    public void ClearBlock()
    {
        block = 0;
    }

    public void ApplyStatus(StatusType statusType, int amount)
    {
        if (statusType == StatusType.None || amount <= 0)
        {
            return;
        }

        if (!statuses.ContainsKey(statusType))
        {
            statuses[statusType] = 0;
        }

        statuses[statusType] += amount;
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

}
