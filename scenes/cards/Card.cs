using System;
using Godot;

namespace PoIAna.scenes.cards;

public partial class Card : AnimatedSprite2D
{
    [Signal]
    public delegate void CardClickedEventHandler(Card self);

    public CardData CardData;
    
    private Area2D _mouseDetector;
    private Tween _hoverTween;

    private Vector2 _restPosition;
    private Vector2 _restScale;

    private bool _interactive;
    
    public override void _Ready()
    {
        base._Ready();

        _mouseDetector = GetNode<Area2D>("MouseDetector");

        _mouseDetector.InputEvent += HandleInputEvent;
        _mouseDetector.MouseEntered += HandleMouseEntered;
        _mouseDetector.MouseExited += HandleMouseExited;

        _mouseDetector.InputPickable = false;

        _restPosition = Position;
        _restScale = Scale;
    }
    
    public void SetInteractive(bool enabled)
    {
        _interactive = enabled;
        _mouseDetector.InputPickable = enabled;

        if (!enabled)
            ResetHover();
    }
    
    private void HandleMouseEntered()
    {
        if (_interactive)
            AnimateHover(true);
    }

    private void HandleMouseExited()
    {
        if (_interactive)
            AnimateHover(false);
    }
    
    private void AnimateHover(bool hovered)
    {
        if (_hoverTween != null && _hoverTween.IsRunning())
            _hoverTween.Kill();

        Vector2 targetPosition = _restPosition +
                                 (hovered ? new Vector2(0, -28) : Vector2.Zero);

        Vector2 targetScale = _restScale *
                              (hovered ? 1.06f : 1.0f);

        _hoverTween = CreateTween();
        _hoverTween.SetParallel();

        _hoverTween.TweenProperty(
                this,
                "position",
                targetPosition,
                0.12f
            ).SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        _hoverTween.TweenProperty(
                this,
                "scale",
                targetScale,
                0.12f
            ).SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
    }

    public void ResetHover()
    {
        if (_hoverTween != null && _hoverTween.IsRunning())
            _hoverTween.Kill();

        Position = _restPosition;
        Scale = _restScale;
    }

    private void HandleInputEvent(Node node, InputEvent inputEvent, long idx)
    {
        if (!_interactive)
            return;

        if (inputEvent is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false })
        {
            EmitSignal(SignalName.CardClicked, this);
        }
    }

    private int GetCardFrameIndex(Suit suit, Score score)
    {
        int suitIndex = suit switch
        {
            Suit.Swords => 3,
            Suit.Coins => 0,
            Suit.Cups => 2,
            Suit.Batons => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(suit), suit, null)
        };
        const int maxSuitCards = 13;
        return suitIndex * maxSuitCards + ((int) score-1);
    }

    public void SetCard(CardData cardData)
    {
        Frame = GetCardFrameIndex(cardData.Suit, cardData.Score);
        CardData = cardData;
    }

    public void SetBack(CardData overrideCardInside)
    {
        CardData = overrideCardInside;
        SetBack();
    }
    
    public void SetBack()
    {
        Frame = GetBackFrameIndex();
    }

    private int GetBackFrameIndex()
    {
        const int maxCards = 55;
        return maxCards - 2;
    }
}

public enum Score
{
    Pad,
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Knight,
    King,
}

public enum Suit
{
    Pad,
    Swords,
    Coins,
    Cups,
    Batons,
}