using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using PoIAna.scenes.autoload;

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
        return new BriscolaWinStrategy().Winner(_playedCards, GetNode<GameGlobals>("/root/GameGlobals"));
    }
}

public record Player(int Index);

public interface IWinStrategy
{
    Player Winner(List<KeyValuePair<Player, CardData>> playedCards, GameGlobals gameGlobals);
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
    public Player Winner(List<KeyValuePair<Player, CardData>> playedCards, GameGlobals gameGlobals)
    {
        BriscolaScore scoreStrategy = new();
        var first = playedCards.First();
        var second = playedCards.Last();
        var firstCard = first.Value;
        var secondCard = second.Value;

        if (firstCard.Suit == gameGlobals.Briscola)
        {
            if (secondCard.Suit == gameGlobals.Briscola)
            {
                return scoreStrategy.Score(secondCard) > scoreStrategy.Score(firstCard) ? second.Key : first.Key;
            }
        }
        else if (secondCard.Suit == gameGlobals.Briscola)
        {
            return second.Key;
        }
        
        if (firstCard.Suit != secondCard.Suit)
        {
            return first.Key;
        }

        if (scoreStrategy.Score(secondCard) + scoreStrategy.Score(firstCard) == 0)
        {
            return firstCard.Score > secondCard.Score ? first.Key : second.Key;
        }

        return scoreStrategy.Score(secondCard) > scoreStrategy.Score(firstCard) ? second.Key : first.Key;
    }
}