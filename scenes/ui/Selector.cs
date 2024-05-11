using System.Collections.Generic;
using Godot;

namespace PoIAna.scenes.ui;

public partial class Selector : PanelContainer
{
    private Button _previous;
    private Button _next;
    private Label _name;
    private Label _description;
    private int _idxOfSelected;
    private readonly List<ModelMeta> _modelMetas = new();

    private ProgressBar _winRate;

    public override void _Ready()
    {
        base._Ready();
        _previous = GetNode<Button>("%Previous");
        _next = GetNode<Button>("%Next");
        _name = GetNode<Label>("%Name");
        _description = GetNode<Label>("%Description");
        _winRate = GetNode<ProgressBar>("%WinRate");
        
        _modelMetas.Sort((meta1, meta2) => meta1.WinRate.CompareTo(meta2.WinRate));
        
        UpdateLabels(_modelMetas[_idxOfSelected]);
        _previous.Pressed += () => UpdateLabels(_modelMetas[--_idxOfSelected % _modelMetas.Count]);
        _next.Pressed += () => UpdateLabels(_modelMetas[++_idxOfSelected % _modelMetas.Count]);
    }

    private void UpdateLabels(ModelMeta selection)
    {
        _name.Text = selection.DisplayName;
        _description.Text = selection.Description;
        _winRate.Value = selection.WinRate * 100;

    }

    public ModelMeta SelectedModel()
    {
        return _modelMetas[_idxOfSelected % _modelMetas.Count];
    }
}

public record ModelMeta(string Icon, string Name, float WinRate)
{
    public string Description;
    public string Filename => Name + ".onnx";
    public string DisplayName => Icon + " " + Name;
}