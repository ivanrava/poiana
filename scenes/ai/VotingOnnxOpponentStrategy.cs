using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using PoIAna.scenes.ui;

namespace PoIAna.scenes.ai;

public class VotingOnnxOpponentStrategy : OnnxOpponentStrategy
{
    private readonly List<(OnnxOpponentStrategy Strategy, float Score)> _opponentStrategies;

    public override int ChooseCard(OnnxState state)
    {
        float[] cardScores = [0f, 0f, 0f];

        foreach (var (strategy, score) in _opponentStrategies)
        {
            int card = strategy.ChooseCard(state);
            GD.Print(card);
            cardScores[card] += score;
        }

        int maxIndex = 0;
        for (int i = 1; i < cardScores.Length; i++)
        {
            if (cardScores[i] > cardScores[maxIndex])
                maxIndex = i;
        }

        return maxIndex;
    }

    public VotingOnnxOpponentStrategy(int votingQuorum, List<ModelMeta> allModels) : base(allModels[0].Filename)
    {
        _opponentStrategies = allModels
            .OrderByDescending(meta => meta.WinRate)
            .Take(votingQuorum)
            .Select(meta => (
                Strategy: new OnnxOpponentStrategy(meta.Filename),
                Score: meta.WinRate))
            .ToList();
    }
}
