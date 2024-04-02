namespace PoIAna.scenes.ai;

public abstract class OpponentStrategy
{
    protected readonly string ModelFilename;

    protected OpponentStrategy(string modelFilename)
    {
        ModelFilename = modelFilename;
    }
    
    public abstract int ChooseCard(OnnxState state);
}