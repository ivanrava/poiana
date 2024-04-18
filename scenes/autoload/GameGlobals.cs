using Godot;
using PoIAna.scenes.cards;
using PoIAna.scenes.ui;

namespace PoIAna.scenes.autoload;

public partial class GameGlobals : Node
{
    public CardData Briscola;
    public ModelMeta ModelMeta;
    public bool IsAudioEnabled = true;
    public bool IsFatalityEnabled = true;
}