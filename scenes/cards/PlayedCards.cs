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
    public readonly List<KeyValuePair<Player, CardData>> Cards = new();
    private readonly List<Card> _cardSprites = new();
    
    public async Task PlayCardOnTable(Player player, Card card)
    {
        int index = _cardSprites.Count;

        Cards.Add(new KeyValuePair<Player, CardData>(
            player,
            card.CardData
        ));

        _cardSprites.Add(card);

        float side = index == 0 ? -1.0f : 1.0f;

        Vector2 targetPosition = GlobalPosition +
                                 new Vector2(side * 48.0f, index == 0 ? 4.0f : -4.0f);

        float targetRotation =
            side * 0.055f +
            (_random.NextSingle() - 0.5f) * 0.025f;

        card.ZIndex = 20;

        Tween tween = CreateTween();
        tween.SetParallel();

        tween.TweenProperty(
                card,
                "global_position",
                targetPosition,
                0.30f
            ).SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(
                card,
                "global_rotation",
                targetRotation,
                0.30f
            ).SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        Vector2 originalScale = card.Scale;

        tween.TweenProperty(
                card,
                "scale",
                originalScale * 1.05f,
                0.20f
            ).SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        tween.Chain().TweenProperty(
            card,
            "scale",
            originalScale,
            0.08f
        );

        await ToSignal(tween, "finished");
    }
    
    public void Clean()
    {
        foreach (var child in _cardSprites)
        {
            child.QueueFree();
        }
        
        _cardSprites.Clear();
        Cards.Clear();
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