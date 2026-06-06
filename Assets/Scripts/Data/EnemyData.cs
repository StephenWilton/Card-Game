using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Enemy Info")]
    [SerializeField] private string enemyName;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private List<EnemyActionData> enemyDeck = new List<EnemyActionData>();
    public string EnemyName => enemyName;

    public List<EnemyActionData> EnemyDeck => enemyDeck;
    public int MaxHealth => maxHealth;
}
