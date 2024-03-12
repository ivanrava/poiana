using Godot;
using PoIAna.scenes.cards;

public partial class Hand : Node2D
{
    private Card Card1, Card2, Card3;
    
    public override void _Ready()
    {
        base._Ready();
        Card1 = GetNode<Card>("1");
        Card2 = GetNode<Card>("2");
        Card3 = GetNode<Card>("3");
        
        Card1.SetBack();
        Card2.SetBack();
        Card3.SetBack();
    }

    public void SetHand(CardData card1, CardData card2, CardData card3)
    {
        Card1.SetCard(card1);
        Card2.SetCard(card2);
        Card3.SetCard(card3);
    }
}
