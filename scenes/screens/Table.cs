using System;
using System.Collections.Generic;
using Godot;
using PoIAna.scenes.ai;
using PoIAna.scenes.autoload;
using PoIAna.scenes.cards;
using PoIAna.scenes.ui;

namespace PoIAna.scenes.screens;

public partial class Table : Node
{
    private Hand _playerHand, _opponentHand;
    private Deck _deck;
    private bool _isPlayerTurn;
    private OnnxOpponentStrategy _opponentStrategy;
    private Area2D _clickOverlay;
    private PlayedCards _playedCards;
    private Random _rng;
    private Button _toggleVisibilityButton;
    private int _turn;

    private static readonly Player Player = new(0);
    private static readonly Player Opponent = new(1);
    private readonly Dictionary<Player, Label> _scores = new();
    private Node2D _handCover;

    private bool RandomBool()
    {
        return _rng.NextDouble() > 0.5;
    }

    public override void _Ready()
    {
        base._Ready();
        _rng = new Random();

        var modelMeta = GetNode<GameGlobals>("/root/GameGlobals").ModelMeta;
        GetNode<Label>("ModelName").Text = modelMeta.DisplayName;
        _opponentStrategy = new OnnxOpponentStrategy(modelMeta.Filename);
        _isPlayerTurn = RandomBool();
        
        _scores.Add(Player, GetNode<Label>("PlayerScore"));
        _scores.Add(Opponent, GetNode<Label>("OpponentScore"));
        
        _playerHand = GetNode<Hand>("PlayerHand");
        _opponentHand = GetNode<Hand>("OpponentHand");
        _deck = GetNode<Deck>("Deck");
        _playedCards = GetNode<PlayedCards>("PlayedCards");
        _toggleVisibilityButton = GetNode<Button>("ToggleVisibilityButton");
        _toggleVisibilityButton.Toggled += on => _handCover.Visible = !_handCover.Visible;
        _handCover = GetNode<Node2D>("HandCover");
        GetNode<Button>("HBoxContainer/ResetButton").Pressed += () => GetTree().ReloadCurrentScene();
        GetNode<Button>("HBoxContainer/MenuButton").Pressed += () =>
            GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://scenes/ui/MainMenu.tscn"));
        
        // Initialize hands
        _playerHand.SetHand(_deck.Draw(), _deck.Draw(), _deck.Draw());
        _opponentHand.SetHand(_deck.Draw(), _deck.Draw(), _deck.Draw());

        _clickOverlay = GetNode<Area2D>("ClickOverlay");
        _clickOverlay.InputEvent += ClickOverlayHandler;
        _clickOverlay.InputPickable = false;

        if (!_isPlayerTurn)
        {
            OpponentMove();
        }
        _playerHand.CardSelected += HandleHandCardSelected;

        _turn = 1;
    }

    private OnnxState State()
    {
        return new OnnxState(
            Int32.Parse(_scores[Opponent].Text),
            Int32.Parse(_scores[Player].Text),
            _opponentHand.Cards.Count,
            _playerHand.Cards.Count,
            _deck.RemainingCards(),
            _opponentHand,
            _playedCards,
            _turn,
            GetNode<GameGlobals>("/root/GameGlobals").Briscola,
            _isPlayerTurn ? 1 : 0
        );
    }

    /**
     * Make the opponent play a card
     */
    private void OpponentMove()
    {
        Card opponentCard = _opponentHand.TakeCard(_opponentStrategy, State());
        _playedCards.PlayCardOnTable(Opponent, opponentCard.CardData);
    }

    /**
     * - Assign points to the winner score
     * - Clean played cards
     * - Update the turn player
     */
    private void ResolvePlayedCards()
    {
        var winner = _playedCards.Winner();
        _scores[winner].Text = (int.Parse(_scores[winner].Text) + _playedCards.Score()).ToString();
        
        _playedCards.Clean();

        _isPlayerTurn = winner == Player;
    }

    /**
     * Draw cards from deck
     */
    private void DrawPhase()
    {
        CardData c1 = _deck.Draw();
        CardData c2 = _deck.Draw();
        if (c1 == null && c2 == null)
            return;
        
        if (_isPlayerTurn)
        {
            _playerHand.AddCard(c1);
            _opponentHand.AddCard(c2);
        }
        else
        {
            _opponentHand.AddCard(c1);
            _playerHand.AddCard(c2);
        }

        _turn++;
    }

    private void ClickOverlayHandler(Node viewport, InputEvent @event, long shapeidx)
    {
        if (@event is InputEventMouseButton && @event.IsReleased())
        {
            ResolvePlayedCards();
            DrawPhase();

            if (!_isPlayerTurn)
            {
                OpponentMove();
            }

            _playerHand.CardSelected += HandleHandCardSelected;
            
            _clickOverlay.InputPickable = false;
            GetViewport().SetInputAsHandled();
        }
    }

    private void HandleHandCardSelected(Card card)
    {
        _playerHand.CardSelected -= HandleHandCardSelected;
        _playedCards.PlayCardOnTable(Player, card.CardData);

        if (_isPlayerTurn)
        {
            OpponentMove();
        }
        
        _clickOverlay.InputPickable = true;
    }
}