using Godot;
using PoIAna.scenes.cards;

public partial class Hand : Node2D
{
    [Signal]
    public delegate void CardSelectedEventHandler(Card card);
    private Card _card1, _card2, _card3;
    
    public override void _Ready()
    {
        base._Ready();
        _card1 = GetNode<Card>("1");
        _card2 = GetNode<Card>("2");
        _card3 = GetNode<Card>("3");
        
        _card1.SetBack();
        _card2.SetBack();
        _card3.SetBack();
        
        _card1.CardClicked += HandleCardClicked;
        _card2.CardClicked += HandleCardClicked;
        _card3.CardClicked += HandleCardClicked;
    }

    public void SetHand(CardData card1, CardData card2, CardData card3)
    {
        _card1.SetCard(card1);
        _card2.SetCard(card2);
        _card3.SetCard(card3);
    }

    private void HandleCardClicked(Card card)
    {
        EmitSignal(SignalName.CardSelected, card);
    }
}
