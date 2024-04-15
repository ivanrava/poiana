using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using PoIAna.scenes.ai;

namespace PoIAna.scenes.cards;

public partial class Hand : Node2D
{
    [Signal]
    public delegate void CardSelectedEventHandler(Card card);
    private Card _card1, _card2, _card3;
    public List<Card> Cards;
    private Card _cardSelected;

    private float TweenSpeed()
    {
	    return 0.4f;
    }

    public override void _Ready()
    {
        base._Ready();
        Cards = new List<Card>();
        _card1 = GetNode<Card>("1");
        _card2 = GetNode<Card>("2");
        _card3 = GetNode<Card>("3");
        
        _card1.Hide();
        _card2.Hide();
        _card3.Hide();
        
        _card1.CardClicked += HandleCardClicked;
        _card2.CardClicked += HandleCardClicked;
        _card3.CardClicked += HandleCardClicked;
    }

    public async Task SetHand(Card card1, Card card2, Card card3)
    {
		Tween tween = CreateTween();
		tween.SetParallel();
		tween.TweenProperty(card1, "global_position", _card1.GlobalPosition, TweenSpeed());
		tween.TweenProperty(card1, "rotation", _card1.Rotation, TweenSpeed());
		tween.TweenProperty(card2, "global_position", _card2.GlobalPosition, TweenSpeed());
		tween.TweenProperty(card2, "rotation", _card2.Rotation, TweenSpeed());
		tween.TweenProperty(card3, "global_position", _card3.GlobalPosition, TweenSpeed());
		tween.TweenProperty(card3, "rotation", _card3.Rotation, TweenSpeed());
        await ToSignal(tween, "finished");
        _card1.SetCard(card1.CardData);
        _card2.SetCard(card2.CardData);
        _card3.SetCard(card3.CardData);
        card1.QueueFree();
        _card1.Show();
        card2.QueueFree();
        _card2.Show();
        card3.QueueFree();
        _card3.Show();
        Cards.Add(_card1);
        Cards.Add(_card2);
        Cards.Add(_card3);
    }

    private void HandleCardClicked(Card card)
    {
        _cardSelected = card;
        _cardSelected.Hide();
        
		Card animableCard = (Card)GD.Load<PackedScene>("res://scenes/cards/Card.tscn").Instantiate();
		animableCard.SetCard(card.CardData);
		AddChild(animableCard);
        animableCard.Position = _cardSelected.Position;
        animableCard.Rotation = _cardSelected.Rotation;
        
        Cards.Remove(_cardSelected);
        EmitSignal(SignalName.CardSelected, animableCard);
    }

    public async Task AddCard(Card drawnCard)
    {
		Tween tween = CreateTween();
		tween.SetParallel();
		tween.SetEase(Tween.EaseType.OutIn);
		tween.TweenProperty(drawnCard, "global_position", _cardSelected.GlobalPosition, TweenSpeed());
		tween.TweenProperty(drawnCard, "rotation", _cardSelected.Rotation, TweenSpeed());
        await ToSignal(tween, "finished");
        _cardSelected.SetCard(drawnCard.CardData);
        drawnCard.QueueFree();
        _cardSelected.Show();
        Cards.Add(_cardSelected);
    }

    public Card TakeCard(OpponentStrategy opponentStrategy, OnnxState state, out Card animableCard)
    {
        int idx = opponentStrategy.ChooseCard(state);
        while (idx >= Cards.Count)
        {
            idx--;
        }
        _cardSelected = Cards[idx];
        Cards.RemoveAt(idx);
        
		animableCard = (Card)GD.Load<PackedScene>("res://scenes/cards/Card.tscn").Instantiate();
		animableCard.SetCard(_cardSelected.CardData);
		AddChild(animableCard);
        animableCard.Position = _cardSelected.Position;
        animableCard.Rotation = _cardSelected.Rotation;
        
        _cardSelected.Hide();
        return _cardSelected;
    }
}