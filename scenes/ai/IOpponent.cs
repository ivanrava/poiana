using PoIAna.scenes.cards;

namespace PoIAna.scenes.ai;

public interface IOpponent
{
    Card ChooseCard(Hand opponentHand);
}