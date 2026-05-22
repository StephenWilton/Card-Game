using System.Collections.Generic;
using UnityEngine;

public class DeckRuntime
{
    public List<CardData> Deck { get; } = new List<CardData>();
    public List<CardData> DrawPile { get; } = new List<CardData>();
    public List<CardData> Hand { get; } = new List<CardData>();
    public List<CardData> DiscardPile { get; } = new List<CardData>();

    public void Initialize(IEnumerable<CardData> startingCards)
    {
        Deck.Clear();
        DrawPile.Clear();
        Hand.Clear();
        DiscardPile.Clear();

        Deck.AddRange(startingCards);
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

            CardData drawnCard = DrawPile[0];
            DrawPile.RemoveAt(0);
            Hand.Add(drawnCard);
        }
    }

    public void PlayCard(CardData card)
    {
        if (!Hand.Remove(card))
        {
            return;
        }

        DiscardPile.Add(card);
    }

    public void AddCard(CardData card)
    {
        if (card == null)
        {
            return;
        }

        Deck.Add(card);
        DiscardPile.Add(card);
    }

    public void ReplaceCard(int index, CardData replacement)
    {
        if (index < 0 || index >= Deck.Count || replacement == null)
        {
            return;
        }

        CardData oldCard = Deck[index];
        Deck[index] = replacement;

        ReplaceCardReference(DrawPile, oldCard, replacement);
        ReplaceCardReference(Hand, oldCard, replacement);
        ReplaceCardReference(DiscardPile, oldCard, replacement);
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

    private void ReplaceCardReference(List<CardData> cards, CardData oldCard, CardData replacement)
    {
        int index = cards.IndexOf(oldCard);
        if (index >= 0)
        {
            cards[index] = replacement;
        }
    }

    private void Shuffle(List<CardData> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardData temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }
}
