using System.Collections.Generic;

public class RunState
{
    public HeroClassData HeroClass { get; private set; }
    public PatronData Patron { get; private set; }
    public DeckRuntime Deck { get; } = new DeckRuntime();
    public int PatronInfluence { get; private set; }

    public string HeroDisplayName => HeroClass != null ? HeroClass.displayName : "Paladin";
    public string PatronDisplayName => Patron != null ? Patron.displayName : "The Devourer";
    public int HeroMaxHealth => HeroClass != null ? HeroClass.maxHealth : 42;

    public void Initialize(HeroClassData heroClass, PatronData patron, IEnumerable<CardData> startingDeck)
    {
        HeroClass = heroClass;
        Patron = patron;
        PatronInfluence = 0;
        Deck.Initialize(startingDeck);
    }

    public void GainPatronInfluence(int amount)
    {
        int maxInfluence = Patron != null ? Patron.maxInfluence : int.MaxValue;
        PatronInfluence = System.Math.Min(PatronInfluence + System.Math.Max(amount, 0), maxInfluence);
    }

    public bool TrySpendPatronInfluence(int amount)
    {
        if (amount < 0 || PatronInfluence < amount)
        {
            return false;
        }

        PatronInfluence -= amount;
        return true;
    }
}
