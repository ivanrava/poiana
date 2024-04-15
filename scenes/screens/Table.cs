using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public override async void _Ready()
    {
        base._Ready();
        _rng = new Random();

        var modelMeta = GetNode<GameGlobals>("/root/GameGlobals").ModelMeta;
        GetNode<Label>("%ModelName").Text = modelMeta.DisplayName;
        _opponentStrategy = new OnnxOpponentStrategy(modelMeta.Filename);
        _isPlayerTurn = RandomBool();
        
        _scores.Add(Player, GetNode<Label>("PlayerScore"));
        _scores.Add(Opponent, GetNode<Label>("OpponentScore"));
        
        _playerHand = GetNode<Hand>("PlayerHand");
        _opponentHand = GetNode<Hand>("OpponentHand");
        _deck = GetNode<Deck>("Deck");
        _playedCards = GetNode<PlayedCards>("PlayedCards");
        
        _handCover = GetNode<Node2D>("HandCover");
        foreach (var backCover in _handCover.GetChildren())
        {
            ((Card)backCover).SetBack();
        }
        
        GetNode<Button>("%ToggleVisibilityButton").Toggled += on => _handCover.Visible = !_handCover.Visible;
        GetNode<Button>("%ResetButton").Pressed += () => GetTree().ReloadCurrentScene();
        GetNode<Button>("%MenuButton").Pressed += () =>
            GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://scenes/ui/MainMenu.tscn"));
        GetNode<Button>("%ModelButton").Pressed += () =>
            GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://scenes/ui/SelectorMenu.tscn"));
        
        // Initialize hands
        _handCover.Hide();
        await _opponentHand.SetHand(_deck.Draw(true), _deck.Draw(true), _deck.Draw(true));
        _handCover.Show();
        await _playerHand.SetHand(_deck.Draw(false), _deck.Draw(false), _deck.Draw(false));
        
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

    private void PlayFatality()
    {
        GetNode<AnimationPlayer>("FatalityBackground/FatalityAnimationPlayer").Play("appear");
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
    private async void OpponentMove()
    {
        Card animableCard;
        Card opponentCard = _opponentHand.TakeCard(_opponentStrategy, State(), out animableCard);
        _handCover.GetNode<Card>(new NodePath(opponentCard.Name)).Hide();
        await _playedCards.PlayCardOnTable(Opponent, animableCard);
        
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
    private async Task DrawPhase()
    {
        Card c1 = _deck.Draw(!_isPlayerTurn);
        if (c1 == null)
            return;
        
        if (_isPlayerTurn)
        {
            await _playerHand.AddCard(c1);
            Card c2 = _deck.Draw(_isPlayerTurn);
            await _opponentHand.AddCard(c2);
            foreach (var node in _handCover.GetChildren())
            {
                var child = (Card)node;
                child.Show();
            }
        }
        else
        {
            await _opponentHand.AddCard(c1);
            foreach (var node in _handCover.GetChildren())
            {
                var child = (Card)node;
                child.Show();
            }
            Card c2 = _deck.Draw(_isPlayerTurn);
            await _playerHand.AddCard(c2);
        }

        _turn++;
    }
    
    private async void ClickOverlayHandler(Node viewport, InputEvent @event, long shapeidx)
    {
        if (@event is InputEventMouseButton && @event.IsReleased())
        {
            ResolvePlayedCards();
            await DrawPhase();
            
            if (!_isPlayerTurn)
            {
                OpponentMove();
            }

            _playerHand.CardSelected += HandleHandCardSelected;
            
            _clickOverlay.InputPickable = false;
            GetViewport().SetInputAsHandled();
        }
    }
    
    private async void HandleHandCardSelected(Card card)
    {
        _playerHand.CardSelected -= HandleHandCardSelected;
        await _playedCards.PlayCardOnTable(Player, card);

        if (_isPlayerTurn)
        {
            OpponentMove();
        }
        
        if (_playedCards.Score() >= 20)
            PlayFatality();
        
        _clickOverlay.InputPickable = true;
    }
}