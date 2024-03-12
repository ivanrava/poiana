using Godot;

namespace PoIAna.scenes;

public partial class Card : AnimatedSprite2D
{
    private int GetCardFrameIndex(Suit suit, Score score)
    {
        const int maxSuitCards = 13;
        return (int) suit * maxSuitCards + (int) score;
    }

    public void SetCard(Suit suit, Score score)
    {
        Frame = GetCardFrameIndex(suit, score);
    }

    public void SetBack()
    {
        Frame = GetBackFrameIndex();
    }

    private int GetBackFrameIndex()
    {
        const int maxCards = 52;
        return maxCards;
    }
}

public enum Score
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

public enum Suit
{
    Coins,
    Batons,
    Cups,
    Swords,
}