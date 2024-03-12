using Godot;
using PoIAna.scenes;

public partial class Deck : Node2D
{
    public override void _Ready()
    {
        base._Ready();
        GetNode<Card>("Back").SetBack();
    }
}
