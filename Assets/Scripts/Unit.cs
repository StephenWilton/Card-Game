using UnityEngine;
using TMPro; // TextMeshPro namespace for text handling

// MonoBehaviour is the base class for all Unity scripts
// it is used to create components that can be attached to GameObjects in the Unity Editor
public class Unit : MonoBehaviour
{
    [SerializeField] private int unitMaxHealth = 10;
    [SerializeField] private int unitCurrentHealth = 10;
    [SerializeField] private int unitBlock = 0;

    public int UnitMaxHealth => unitMaxHealth;
    public int UnitCurrentHealth => unitCurrentHealth;
    public int UnitBlock => unitBlock;

    public bool IsDead => unitCurrentHealth <= 0;

    public void TakeDamage(int damageAmount)
    {
        int damageAfterBlock = Mathf.Max(damageAmount - unitBlock, 0);
        unitCurrentHealth -= damageAfterBlock;
        unitBlock = Mathf.Max(unitBlock - damageAmount, 0);

        if (IsDead)
        {
            Die();
        }

    }

    public void Heal(int healAmount)
    {
        unitCurrentHealth = Mathf.Min(unitCurrentHealth + healAmount, unitMaxHealth);
    }

    public void AddBlock(int blockAmount)
    {
        unitBlock += blockAmount;
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
