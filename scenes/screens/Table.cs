using Godot;
using PoIAna.scenes.cards;

public partial class Table : Node
{
    private Hand _playerHand, _opponentHand;
    private Deck _deck;
    
    public override void _Ready()
    {
        base._Ready();
        _playerHand = GetNode<Hand>("PlayerHand");
        _opponentHand = GetNode<Hand>("OpponentHand");
        _deck = GetNode<Deck>("Deck");
        _playerHand.SetHand(_deck.Draw(), _deck.Draw(), _deck.Draw());
    }
}
