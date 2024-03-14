using Godot;

namespace PoIAna.scenes.cards;

public partial class Card : AnimatedSprite2D
{
    [Signal]
    public delegate void CardClickedEventHandler(Card self);

    public CardData CardData;
    
    public override void _Ready()
    {
        base._Ready();
        var mouseDetector = GetNode<Area2D>("MouseDetector");
        mouseDetector.InputEvent += HandleInputEvent;
    }

    private void HandleInputEvent(Node node, InputEvent inputEvent, long idx)
    {
        if (inputEvent is InputEventMouseButton && inputEvent.IsReleased())
        {
            EmitSignal(SignalName.CardClicked, this);
        }
    }

    private int GetCardFrameIndex(Suit suit, Score score)
    {
        const int maxSuitCards = 13;
        return (int) suit * maxSuitCards + (int) score;
    }

    public void SetCard(CardData cardData)
    {
        Frame = GetCardFrameIndex(cardData.Suit, cardData.Score);
        CardData = cardData;
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