using System.Collections.Generic; // Lets us use List<T>
using UnityEngine;

public enum HeroClass
{
    Paladin
}

public enum PatronType
{
    None,
    TheDevourer
}

public enum CardType
{
    Attack,
    Skill,
    Power
}

public enum CardTarget
{
    None,
    Player,
    Enemy,
    Both,
    AllEnemies,
    AllUnits,
    FirstRow,
    BackRow,
    PierceColumn
}

public enum DamageType
{
    None,
    Physical,
    Holy
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare
}

public enum StatusType
{
    None,
    Marked,
    Burned
}

public enum ConditionType
{
    None,
    PlayerHasStatus,
    EnemyHasStatus
}

public enum CardActionType
{
    Damage,
    Block,
    Heal,
    Draw,
    ApplyStatus
}

// This class represents ONE thing a card can do.
// Example: deal damage, gain block, apply status, heal, draw cards.
[System.Serializable]
public class CardActionData
{
    [Header("Action Info")]

    // What kind of action is this?
    // Damage, Block, Heal, Draw, or ApplyStatus.
    public CardActionType actionType;

    // Who does this action affect?
    // Example: Smite targets Enemy. Shield targets Player.
    public CardTarget target = CardTarget.None;


    [Header("Amount")]

    // Generic number used by this action.
    // Damage = damage amount.
    // Block = block amount.
    // Heal = heal amount.
    // Draw = number of cards drawn.
    public int amount = 0;


    [Header("Damage Settings")]

    // Only used if actionType is Damage.
    // Example: Smite has Physical damage and Holy damage as separate actions.
    public DamageType damageType = DamageType.None;


    [Header("Status Settings")]

    // Only used if actionType is ApplyStatus.
    // Example: Vow of Enmity applies Marked.
    public StatusType statusToApply = StatusType.None;


    [Header("Condition")]

    // Optional condition.
    // If this is None, the action always happens.
    public ConditionType conditionType = ConditionType.None;

    // The required status for the condition.
    // Example: EnemyHasStatus + Marked.
    public StatusType requiredStatus = StatusType.None;

}

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Basic Card Info")]

    // The class this card belongs to.
    public HeroClass heroClass = HeroClass.Paladin;

    // None for normal class cards. The Devourer for corrupted patron cards.
    public PatronType patronType = PatronType.None;

    // True when this is a patron-influenced version of a class card.
    public bool isCorrupted = false;

    // Attack, Skill, or Power.
    public CardType cardType;

    // Common, Uncommon, or Rare.
    public Rarity rarity;

    // The name shown on the card.
    public string cardName;

    // The description shown on the card.
    [TextArea]
    public string cardDescription;

    // How much energy the card costs to play.
    public int energyCost = 1;

    [Header("Card Actions")]

    // The list of things this card does.
    // A card can have one action or multiple actions.
    public List<CardActionData> actions = new List<CardActionData>();


    [Header("Card Progression")]

    // The upgraded version of this card.
    public CardData upgradeCardData;

    // The corrupted/patron-influenced version of this card.
    public CardData corruptedCardData;
}
