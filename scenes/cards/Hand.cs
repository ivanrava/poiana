using System.Collections.Generic;
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

    public override void _Ready()
    {
        base._Ready();
        Cards = new List<Card>();
        _card1 = GetNode<Card>("1");
        _card2 = GetNode<Card>("2");
        _card3 = GetNode<Card>("3");
        
        _card1.CardClicked += HandleCardClicked;
        _card2.CardClicked += HandleCardClicked;
        _card3.CardClicked += HandleCardClicked;
    }

    public void SetHand(CardData card1, CardData card2, CardData card3)
    {
        _card1.SetCard(card1);
        _card2.SetCard(card2);
        _card3.SetCard(card3);
        Cards.Add(_card1);
        Cards.Add(_card2);
        Cards.Add(_card3);
    }

    private void HandleCardClicked(Card card)
    {
        _cardSelected = card;
        _cardSelected.Hide();
        Cards.Remove(_cardSelected);
        EmitSignal(SignalName.CardSelected, card);
    }

    public void AddCard(CardData drawnCard)
    {
        _cardSelected.SetCard(drawnCard);
        _cardSelected.Show();
        Cards.Add(_cardSelected);
    }

    public Card TakeCard(OpponentStrategy opponentStrategy, OnnxState state)
    {
        int idx = opponentStrategy.ChooseCard(state);
        while (idx >= Cards.Count)
        {
            idx--;
        }
        _cardSelected = Cards[idx];
        Cards.RemoveAt(idx);
        _cardSelected.Hide();
        return _cardSelected;
    }
}