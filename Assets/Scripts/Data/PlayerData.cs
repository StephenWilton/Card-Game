using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Player", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Basic Player Info")]
    [SerializeField] private string playerName;
    [SerializeField] private int maxHealth = 75;
    [SerializeField] private int maxEnergy = 3;

    [SerializeField] private int cardDraw = 5;

    [Header("Starting Deck")]
    [SerializeField] private List<CardData> startingDeck = new List<CardData>();
    public string PlayerName => playerName;
    public int MaxHealth => maxHealth;
    public int MaxEnergy => maxEnergy;

    public int CardDraw => cardDraw;
    public List<CardData> StartingDeck => startingDeck;
}
