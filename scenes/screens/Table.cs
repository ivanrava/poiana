using Godot;
using PoIAna.scenes.cards;

namespace PoIAna.scenes.screens;

public partial class Table : Node
{
    private Hand _playerHand, _opponentHand;
    private Deck _deck;
    private Marker2D _playerCardPosition, _opponentCardPosition;
    
    public override void _Ready()
    {
        base._Ready();
        _playerHand = GetNode<Hand>("PlayerHand");
        _opponentHand = GetNode<Hand>("OpponentHand");
        _deck = GetNode<Deck>("Deck");
        _playerHand.SetHand(_deck.Draw(), _deck.Draw(), _deck.Draw());
        _playerHand.CardSelected += HandleHandCardSelected;
        _playerCardPosition = GetNode<Marker2D>("PlayerCardPosition");
        _opponentCardPosition = GetNode<Marker2D>("OpponentCardPosition");
    }
    
    private void HandleHandCardSelected(Card card)
    {
        card.Rotation = 0;
        card.GlobalPosition = _playerCardPosition.Position;
    }
}