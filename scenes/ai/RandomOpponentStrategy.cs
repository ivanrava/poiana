using System;

namespace PoIAna.scenes.ai;

public class RandomOpponentStrategy : IOpponentStrategy
{
    private readonly Random _random = new();
    
    public int ChooseCard(OnnxState state)
    {
        return _random.Next(0, state.Hand.Cards.Count);
    }
}