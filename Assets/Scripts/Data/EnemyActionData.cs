using UnityEngine;

public enum ActionType
{
    Attack,
    Skill,
    Power
}

public enum EnemyTargetType
{
    Self,
    Player
}

[CreateAssetMenu(fileName = "New Enemy Action", menuName = "Enemy Actions/Enemy Action Data")]
public class EnemyActionData : ScriptableObject
{
    [Header("Basic Enemy Action Info")]
    [SerializeField] private ActionType actionType;
    [SerializeField] private EnemyTargetType enemyTargetType;
    [SerializeField] private string actionName;
    [SerializeField] private int amount = 0;

    public ActionType ActionType => actionType;
    public EnemyTargetType EnemyTargetType => enemyTargetType;
    public string ActionName => actionName;
    public int Amount => amount;
}
