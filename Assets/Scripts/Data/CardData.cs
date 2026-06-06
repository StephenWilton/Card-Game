using UnityEngine;

public enum CardType
{
    Attack,
    Skill,
    Power
}

public enum TargetType
{
    Self,
    Enemy
}

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Basic Card Info")]
    [SerializeField] private CardType cardType;
    [SerializeField] private TargetType targetType;
    [SerializeField] private string cardName;
    [SerializeField, TextArea] private string cardDescription;
    [SerializeField] private int energyCost = 1;

    [SerializeField] private int amount = 0;

    public CardType CardType => cardType;
    public TargetType TargetType => targetType;
    public string CardName => cardName;
    public string CardDescription => cardDescription;
    public int EnergyCost => energyCost;
    public int Amount => amount;
}
