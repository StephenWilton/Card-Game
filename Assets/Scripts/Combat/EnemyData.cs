using UnityEngine;
using System.Collections.Generic;

public enum EnemyIntentType
{
    Attack,
    Block,
    ApplyStatus
}

public enum EnemySpecialBehavior
{
    Standard,
    MirrorMiniBoss
}

[System.Serializable]
public class EnemyIntentData
{
    public EnemyIntentType intentType = EnemyIntentType.Attack;
    public int amount = 6;
    public StatusType statusToApply = StatusType.None;
}

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "Enemy";
    public int maxHealth = 18;
    public int attackDamage = 6;
    public bool isMiniBoss = false;
    public EnemySpecialBehavior specialBehavior = EnemySpecialBehavior.Standard;
    public List<EnemyIntentData> intents = new List<EnemyIntentData>();
}
