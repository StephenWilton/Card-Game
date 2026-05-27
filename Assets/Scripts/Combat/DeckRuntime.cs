using System.Collections.Generic;
using UnityEngine;

public class DeckRuntime
{
    public List<CardInstance> Deck { get; } = new List<CardInstance>();
    public List<CardInstance> DrawPile { get; } = new List<CardInstance>();
    public List<CardInstance> Hand { get; } = new List<CardInstance>();
    public List<CardInstance> DiscardPile { get; } = new List<CardInstance>();

    public void Initialize(IEnumerable<CardData> startingCards)
    {
        Deck.Clear();
        DrawPile.Clear();
        Hand.Clear();
        DiscardPile.Clear();

        if (startingCards == null)
        {
            return;
        }

        foreach (CardData card in startingCards)
        {
            if (card != null)
            {
                Deck.Add(new CardInstance(card));
            }
        }

        DrawPile.AddRange(Deck);
        Shuffle(DrawPile);
    }

    public void DrawNewHand(int handSize)
    {
        DiscardHand();
        DrawCards(handSize);
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (DrawPile.Count == 0)
            {
                ReshuffleDiscardIntoDraw();
            }

            if (DrawPile.Count == 0)
            {
                return;
            }

            CardInstance drawnCard = DrawPile[0];
            DrawPile.RemoveAt(0);
            Hand.Add(drawnCard);
        }
    }

    public void PlayCard(CardInstance card)
    {
        if (!Hand.Remove(card))
        {
            return;
        }

        DiscardPile.Add(card);
    }

    public CardInstance AddCard(CardData card)
    {
        if (card == null)
        {
            return null;
        }

        CardInstance cardInstance = new CardInstance(card);
        Deck.Add(cardInstance);
        DiscardPile.Add(cardInstance);
        return cardInstance;
    }

    public void ReplaceCard(int index, CardData replacement)
    {
        if (index < 0 || index >= Deck.Count || replacement == null)
        {
            return;
        }

        Deck[index].ReplaceData(replacement);
    }

    public void DiscardHand()
    {
        DiscardPile.AddRange(Hand);
        Hand.Clear();
    }

    private void ReshuffleDiscardIntoDraw()
    {
        DrawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
        Shuffle(DrawPile);
    }

    private void Shuffle(List<CardInstance> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardInstance temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }
}
