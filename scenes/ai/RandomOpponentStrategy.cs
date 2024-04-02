using System;

namespace PoIAna.scenes.ai;

public class RandomOpponentStrategy : OpponentStrategy
{
    private readonly Random _random = new();
    
    public override int ChooseCard(OnnxState state)
    {
        return _random.Next(0, state.Hand.Cards.Count);
    }

    public RandomOpponentStrategy(string modelFilename) : base(modelFilename)
    {
    }
}