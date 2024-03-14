using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PoIAna.scenes.cards;

public partial class PlayedCards : Node2D
{
    private PackedScene _cardScene = GD.Load<PackedScene>("res://scenes/cards/Card.tscn");
    private readonly Random _random = new();
    private Card _lastCard;
    private readonly List<KeyValuePair<Player, CardData>> _playedCards = new();

    public void PlayCard(Player player, CardData cardData)
    {
        _playedCards.Add(new KeyValuePair<Player, CardData>(player, cardData));
        InstantiatePlayedCard(cardData);
    }

    private void InstantiatePlayedCard(CardData cardData)
    {
        Card card = (Card)_cardScene.Instantiate();
        card.SetCard(cardData);
        AddChild(card);
        DodgeCard(card);
    }

    private void DodgeCard(Card card)
    {
        card.Position = _lastCard == null ? card.Position : _lastCard.Position + Vector2.Left * 100;
        card.Rotation = _random.NextSingle() * Mathf.Pi * 2;
        _lastCard = card;
    }

    public void Clean()
    {
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }

        _lastCard = null;
    }

    public int Score()
    {
        return GetChildren().Cast<Card>().Sum(card => new BriscolaScore().Score(card.CardData));
    }

    public Player Winner()
    {
        return new BriscolaWinStrategy().Winner(_playedCards);
    }
}

public record Player(int Index);

public interface IWinStrategy
{
    Player Winner(List<KeyValuePair<Player, CardData>> playedCards);
}

public interface IScoreStrategy
{
    int Score(CardData cardData);
}

public class BriscolaScore : IScoreStrategy
{
    public int Score(CardData cardData)
    {
        return cardData.Score switch
        {
            cards.Score.Ace => 11,
            cards.Score.Three => 10,
            cards.Score.Jack => 2,
            cards.Score.Knight => 3,
            cards.Score.King => 4,
            _ => 0
        };
    }
}

public class BriscolaWinStrategy : IWinStrategy
{
    public Player Winner(List<KeyValuePair<Player, CardData>> playedCards)
    {
        BriscolaScore scoreStrategy = new();
        var first = playedCards.First();
        var second = playedCards.Last();
        var firstCard = first.Value;
        var secondCard = second.Value;

        // TODO: add briscola win condition
        if (firstCard.Suit != secondCard.Suit)
        {
            return first.Key;
        }

        return scoreStrategy.Score(firstCard) > scoreStrategy.Score(secondCard) ? first.Key : second.Key;
    }
}