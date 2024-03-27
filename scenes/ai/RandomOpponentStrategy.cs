using System;
using System.Collections.Generic;
using PoIAna.scenes.cards;

namespace PoIAna.scenes.ai;

public class RandomOpponentStrategy : IOpponentStrategy
{
    private readonly Random _random = new();
    
    public int ChooseCard(List<Card> hand)
    {
        return _random.Next(0, hand.Count);
    }
}