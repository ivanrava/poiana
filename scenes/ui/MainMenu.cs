using Godot;
using PoIAna.scenes.autoload;

namespace PoIAna.scenes.ui;

public partial class MainMenu : TextureRect
{
    public override void _Ready()
    {
        GetNode<Button>("%Play").ButtonUp += () =>
        {
            var gameScene = GD.Load<PackedScene>("res://scenes/screens/Table.tscn");
            GetNode<GameGlobals>("/root/GameGlobals").ModelMeta = GetNode<Selector>("ModelSelector").SelectedModel();
            GetTree().ChangeSceneToPacked(gameScene);
        };
    }
}