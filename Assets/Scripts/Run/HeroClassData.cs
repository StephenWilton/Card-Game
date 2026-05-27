using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero Class", menuName = "Run/Hero Class")]
public class HeroClassData : ScriptableObject
{
    public HeroClass heroClass = HeroClass.Paladin;
    public string displayName = "Paladin";
    [TextArea] public string description;
    public int maxHealth = 42;
    public List<CardData> startingDeck = new List<CardData>();
    public List<CardData> rewardPool = new List<CardData>();
}
