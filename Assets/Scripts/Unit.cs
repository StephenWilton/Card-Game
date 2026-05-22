using UnityEngine;
using TMPro; // TextMeshPro namespace for text handling

// MonoBehaviour is the base class for all Unity scripts
// it is used to create components that can be attached to GameObjects in the Unity Editor
public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName = "Unit";
    [SerializeField] private int unitMaxHealth = 10;
    [SerializeField] private int unitCurrentHealth = 10;
    [SerializeField] private int unitBlock = 0;

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
    }

    public void ResetForCombat()
    {
        unitCurrentHealth = unitMaxHealth;
        unitBlock = 0;
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
    
    public void Die()
    {
        // Handle unit death (e.g., play animation, remove from game, etc.)
        Debug.Log($"{gameObject.name} has died.");
    }

}
