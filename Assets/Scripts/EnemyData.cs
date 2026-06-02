using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyCardData
{
    [Header("Basic Card Info")]
    public string cardName = "Enemy Card";

    [TextArea]
    public string cardDescription;

    [Header("Card Actions")]
    public List<CardActionData> actions = new List<CardActionData>();
}

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Enemy Info")]
    public string enemyName = "Enemy";
    public int maxHealth = 10;
    public Sprite artwork;

    [Header("Enemy Cards")]
    public List<EnemyCardData> enemyCards = new List<EnemyCardData>();
}
