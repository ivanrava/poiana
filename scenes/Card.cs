using Godot;

namespace PoIAna.scenes;

public partial class Card : Node2D
{
    private AnimatedSprite2D _sprite;
    
    public override void _Ready()
    {
        base._Ready();
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    private int GetCardFrameIndex(Suit suit, Score score)
    {
        const int maxSuitCards = 13;
        return (int) suit * maxSuitCards + (int) score;
    }
}

internal enum Score
{
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

internal enum Suit
{
    Coins,
    Batons,
    Cups,
    Swords,
}