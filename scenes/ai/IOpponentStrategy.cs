using System.Collections.Generic;
using PoIAna.scenes.cards;

namespace PoIAna.scenes.ai;

public interface IOpponentStrategy
{
    int ChooseCard(List<Card> hand);
}