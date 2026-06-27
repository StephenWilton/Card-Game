using System.Collections.Generic;

public class PlayerState
{
    public string PlayerName { get; private set; }
    public int MaxHealth { get; private set; }
    public int MaxEnergy { get; private set; }

    public int CurrentEnergy { get; set; }
    
    public int cardDraw { get; private set; }

    public List<CardData> Deck { get; private set; }

    public PlayerState(PlayerData playerData)
    {
        PlayerName = playerData.PlayerName;
        MaxHealth = playerData.MaxHealth;
        CurrentEnergy = playerData.MaxEnergy;
        MaxEnergy = playerData.MaxEnergy;
        cardDraw = playerData.CardDraw;


        Deck = new List<CardData>(playerData.StartingDeck);

    }

    public void RefillEnergy()
    {
        CurrentEnergy = MaxEnergy;
    }

    public bool SpendEnergy(int amount)
    {
        if (CurrentEnergy >= amount)
        {
            CurrentEnergy -= amount;
            return true;
        }
        return false;
    }
}