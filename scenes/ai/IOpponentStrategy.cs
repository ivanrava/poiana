namespace PoIAna.scenes.ai;

public interface IOpponentStrategy
{
    int ChooseCard(OnnxState state);
}