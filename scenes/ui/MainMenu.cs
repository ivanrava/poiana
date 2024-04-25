using Godot;
using PoIAna.scenes.autoload;

namespace PoIAna.scenes.ui;

public partial class MainMenu : TextureRect
{
    public override void _Ready()
    {
        GetNode<Button>("%Play").ButtonUp += () =>
        {
            var gameScene = GD.Load<PackedScene>("res://scenes/ui/SelectorMenu.tscn");
            GetTree().ChangeSceneToPacked(gameScene);
        };
        GetNode<Button>("%Rules").ButtonUp += () =>
        {
            var gameScene = GD.Load<PackedScene>("res://scenes/ui/Rules.tscn");
            GetTree().ChangeSceneToPacked(gameScene);
        };
        GetNode<Button>("%Options").ButtonUp += () =>
        {
            var optionsScene = GD.Load<PackedScene>("res://scenes/ui/OptionsMenu.tscn");
            GetTree().ChangeSceneToPacked(optionsScene);
        };
        GetNode<Button>("%Quit").ButtonUp += () =>
        {
            GetTree().Quit();
        };
    }
}