using System;
using System.Collections.Generic;
using Godot;
using PoIAna.scenes;

public partial class Deck : Node2D
{
    private ICardExtractor _extractor;
    
    public override void _Ready()
    {
        base._Ready();
        _extractor = new DeckBriscola();
        GetNode<Card>("Back").SetBack();
        GetNode<Card>("Briscola").SetCard(_extractor.Extract());
    }
}

internal interface ICardExtractor
{
    CardData Extract();
}

public record CardData(Suit Suit, Score Score);

internal class DeckBriscola : ICardExtractor
{
    private readonly List<CardData> _cards;

    public DeckBriscola()
    {
        _cards = new List<CardData>();
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