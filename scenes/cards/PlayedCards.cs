using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using PoIAna.scenes.autoload;

namespace PoIAna.scenes.cards;

public partial class PlayedCards : Node2D
{
    private PackedScene _cardScene = GD.Load<PackedScene>("res://scenes/cards/Card.tscn");
    private readonly Random _random = new();
    private Vector2 _otherCardPosition = Vector2.Zero;
    public readonly List<KeyValuePair<Player, CardData>> Cards = new();
    private readonly List<Card> _cardSprites = new();
    
    public async Task PlayCardOnTable(Player player, Card card)
    {
        const float speed = 0.4f;
        Cards.Add(new KeyValuePair<Player, CardData>(player, card.CardData));
        Tween tween = CreateTween();
        tween.SetParallel();
        _cardSprites.Add(card);
        _otherCardPosition = GetDodgePosition();
        tween.SetEase(Tween.EaseType.OutIn);
        tween.TweenProperty(card, "global_position", _otherCardPosition, speed);
        var rot = Mathf.LerpAngle(card.Rotation, GetDodgeRotation(), 1);
        tween.TweenProperty(card, "rotation", rot, speed);
        await ToSignal(tween, "finished");
        
    }
    
    private Vector2 GetDodgePosition()
    {
        return _otherCardPosition == Vector2.Zero ? GlobalPosition : _otherCardPosition + Vector2.Left * 100;
    }
    
    private float GetDodgeRotation()
    {
        return _random.NextSingle() * Mathf.Pi * 2;
    }
    
    public void Clean()
    {
        foreach (var child in _cardSprites)
        {
            child.QueueFree();
        }
        
        _cardSprites.Clear();
        Cards.Clear();
        _otherCardPosition = Vector2.Zero;
    }
    
    public int Score()
    {
        return _cardSprites.Sum(card => new BriscolaScore().Score(card.CardData));
    }

    public Player Winner()
    {
        return new BriscolaWinStrategy().Winner(Cards, GetNode<GameGlobals>("/root/GameGlobals"));
    }
}

public record Player(int Index);