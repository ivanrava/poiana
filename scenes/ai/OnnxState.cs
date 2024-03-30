using System.Collections.Generic;
using System.Linq;
using Godot;
using PoIAna.scenes.cards;

namespace PoIAna.scenes.ai;

public record OnnxState(int MyPoints, int OtherPoints, int HandSize, int OtherHandSize, int RemainingDeckCards,
                                Hand Hand, PlayedCards Table, int Turn, CardData Briscola, int Order)
{
    private float[] OneHotEncoding(int value, int cardinality)
    {
        float[] toReturn = Enumerable.Repeat(0.0f, cardinality).ToArray();
        // GD.Print(toReturn.Length, value);
        toReturn[value] = 1f;
        return toReturn;
    }

    private float[] OneHotEncoding(CardData card)
    {
        float[] cardOH = OneHotEncoding((int)card.Score, 14)
            .Concat(OneHotEncoding((int)card.Suit, 5))
            .Concat(OneHotEncoding(new BriscolaScore().Score(card), 12))
            .ToArray();

        return cardOH;
    }

    private IEnumerable<CardData> PadList(IEnumerable<CardData> cards)
    {
        List<CardData> pad = new();
        while (cards.Count() + pad.Count < 3)
        {
            pad.Add(new CardData(Suit.Pad, Score.Pad));
        }

        return cards.Concat(pad);
    }
    
    public float[] FlatOneHot()
    {
        float[] myPointsOH = OneHotEncoding(MyPoints, 121);
        float[] otherPointsOH = OneHotEncoding(OtherPoints, 121);
        float[] handSizeOH = OneHotEncoding(HandSize, 4);
        float[] otherHandSizeOH = OneHotEncoding(OtherHandSize, 4);
        float[] remainingDeckCardsOH = OneHotEncoding(RemainingDeckCards, 41);

        float[] handOH = PadList(Hand.Cards.Select(card => card.CardData)).Select(OneHotEncoding).Aggregate((a, b) => a.Concat(b).ToArray());

        float[] tableOH = PadList(Table.Cards.Select(pair => pair.Value)).Select(OneHotEncoding).Aggregate((a, b) => a.Concat(b).ToArray());

        float[] turnOH = OneHotEncoding(Turn, 40);

        float[] briscolaOH = OneHotEncoding(Briscola);

        float[] orderOH = OneHotEncoding(Order, 2);

        return myPointsOH
            .Concat(otherPointsOH)
            .Concat(handSizeOH)
            .Concat(otherHandSizeOH)
            .Concat(remainingDeckCardsOH)
            .Concat(handOH)
            .Concat(tableOH)
            .Concat(turnOH)
            .Concat(briscolaOH)
            .Concat(orderOH)
            .ToArray();
    }
}