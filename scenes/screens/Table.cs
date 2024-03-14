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

    private int _playerScore = 0, _opponentScore = 0;
    private const int PlayerIndex = 0, OpponentIndex = 1;
    private static readonly Player Player = new Player(PlayerIndex);
    private static readonly Player Opponent = new Player(OpponentIndex);

    public override void _Ready()
    {
        base._Ready();
        
        _opponent = new RandomOpponent();
        _isPlayerTurn = true;
        
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
            GD.Print(_playedCards.Score());
            GD.Print(_playedCards.Winner());
            
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