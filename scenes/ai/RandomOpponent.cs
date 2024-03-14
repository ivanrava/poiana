using System;
using System.Collections.Generic;
using PoIAna.scenes.cards;

namespace PoIAna.scenes.ai;

public class RandomOpponent : IOpponent
{
    private readonly Random _random = new();
    
    public Card ChooseCard(Hand opponentHand)
    {
        List<Card> cards = opponentHand.Cards;
        var randomIdx = _random.Next(0, cards.Count);
        var selectedCard = cards[randomIdx];
        cards.RemoveAt(randomIdx);
        return selectedCard;
    }
}