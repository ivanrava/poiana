using System;
using System.Collections.Generic;
using Godot;

namespace PoIAna.scenes.cards;

public partial class Deck : Node2D
{
    private ICardExtractor _deck;
    private Label _remainingCards;
    
    public override void _Ready()
    {
        base._Ready();
        _deck = new DeckBriscola();
        GetNode<Card>("Back").SetBack();
        GetNode<Card>("Briscola").SetCard(_deck.Extract());
        _remainingCards = GetNode<Label>("RemainingCards");
        UpdateRemainingCards();
    }

    private void UpdateRemainingCards()
    {
        _remainingCards.Text = (_deck.Count() + 1).ToString();
    }

    public new CardData Draw()
    {
        CardData extracted = _deck.Extract();
        UpdateRemainingCards();
        return extracted;
    }
}

internal interface ICardExtractor
{
    CardData Extract();
    int Count();
}

public record CardData(Suit Suit, Score Score);

internal class DeckBriscola : ICardExtractor
{
    private readonly List<CardData> _cards = new();

    public DeckBriscola()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Score score in Enum.GetValues(typeof(Score)))
            {
                if (score != Score.Eight && score != Score.Nine && score != Score.Ten)
                {
                    _cards.Add(new CardData(suit, score));
                }
            }
        }
        _cards.Shuffle();
    }

    public CardData Extract()
    {
        CardData card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public int Count()
    {
        return _cards.Count;
    }
}

static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}