using Godot;
using System;

public partial class BackButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        ButtonUp += () =>
        {
            var gameScene = GD.Load<PackedScene>("res://scenes/ui/MainMenu.tscn");
            GetTree().ChangeSceneToPacked(gameScene);
        };
    }
}
