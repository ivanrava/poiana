using System.Collections.Generic;
using Godot;
using PoIAna.scenes.ai;
using PoIAna.scenes.cards;

namespace PoIAna.scenes.screens;

public partial class Table : Node
{
    private Hand _playerHand, _opponentHand;
    private Deck _deck;
    private bool _isPlayerTurn;
    private IOpponent _opponent;
    private Area2D _clickOverlay;
    private PlayedCards _playedCards;

    private static readonly Player Player = new(0);
    private static readonly Player Opponent = new(1);
    private readonly Dictionary<Player, Label> _scores = new();

    public override void _Ready()
    {
        base._Ready();
        
        _opponent = new RandomOpponent();
        _isPlayerTurn = true;
        
        _scores.Add(Player, GetNode<Label>("PlayerScore"));
        _scores.Add(Opponent, GetNode<Label>("OpponentScore"));
        
        _playerHand = GetNode<Hand>("PlayerHand");
        _opponentHand = GetNode<Hand>("OpponentHand");
        _deck = GetNode<Deck>("Deck");
        _playedCards = GetNode<PlayedCards>("PlayedCards");
        // Initialize hands
        _playerHand.SetHand(_deck.Draw(), _deck.Draw(), _deck.Draw());
        _playerHand.CardSelected += HandleHandCardSelected;
        _opponentHand.SetHand(_deck.Draw(), _deck.Draw(), _deck.Draw());

        _clickOverlay = GetNode<Area2D>("ClickOverlay");
        _clickOverlay.InputEvent += ClickOverlayHandler;
        _clickOverlay.InputPickable = false;
    }

    private void ClickOverlayHandler(Node viewport, InputEvent @event, long shapeidx)
    {
        if (@event is InputEventMouseButton && @event.IsReleased())
        {
            _scores[_playedCards.Winner()].Text = (int.Parse(_scores[_playedCards.Winner()].Text) + _playedCards.Score()).ToString();
            
            _playedCards.Clean();
            _isPlayerTurn = true;

            _playerHand.AddCard(_deck.Draw());
            _opponentHand.AddCard(_deck.Draw());
            
            _clickOverlay.InputPickable = false;
            GetViewport().SetInputAsHandled();
        }
    }

    private void HandleHandCardSelected(Card card)
    {
        if (_isPlayerTurn)
        {
            _playedCards.PlayCard(Player, card.CardData);
            _isPlayerTurn = false;
            Card opponentCard = _opponentHand.ChooseCard(_opponent);
            _playedCards.PlayCard(Opponent, opponentCard.CardData);
            _clickOverlay.InputPickable = true;
        }
    }
}