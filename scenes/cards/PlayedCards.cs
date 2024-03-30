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
    public readonly List<KeyValuePair<Player, CardData>> Cards = new();

    public void PlayCardOnTable(Player player, CardData cardData)
    {
        Cards.Add(new KeyValuePair<Player, CardData>(player, cardData));
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

        Cards.Clear();
        _lastCard = null;
    }

    public int Score()
    {
        return GetChildren().Cast<Card>().Sum(card => new BriscolaScore().Score(card.CardData));
    }

    public Player Winner()
    {
        return new BriscolaWinStrategy().Winner(Cards, GetNode<GameGlobals>("/root/GameGlobals"));
    }
}

public record Player(int Index);