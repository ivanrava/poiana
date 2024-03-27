using PoIAna.scenes.cards;

namespace PoIAna.scenes.ai;

public interface IOpponentStrategy
{
    Card ChooseCard(Hand opponentHand);
}