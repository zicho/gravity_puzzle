using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Godot;
using Godot.Collections;

namespace Core;

public partial class Map : TileMap
{
    public Player Player => GetNode<Player>("Player");

    public List<EntityBase> Entities { get; private set; }

    public Vector2 Gravity;

    public Label GravityLabel { get; private set; }

    public List<Vector2i> UsedTiles;

    private int _selectedGravity = 0;

    public static readonly Vector2[] Gravities = new[] {
        Vector2.Down,
        Vector2.Left,
        Vector2.Up,
        Vector2.Right,
    };

    public Timer TickTimer { get; set; } = new Timer();
    public EventHandler OnTickEventHandler { get; set; }
    public EventHandler OnGravCheckFinishedEventHandler { get; set; }
    
    // NOTE: ENTITY HAS MOVED IS ONLY USED TOP KEEP TRACK OF IF ANY ENTITET HAS MOVED DURING A "TICK"
    // IT'S A HACK TO DETERMINE IF A SOUND NEEDS TO BE PLAYED!
    public bool AnyEntityHasMovedThisTick { get;  set; }

    public override void _Ready()
    {
        UsedTiles = GetUsedCells(0).ToList();
        Gravity = Gravities[_selectedGravity];

        GravityLabel = GetNode<Label>("GravityLabel");
        GravityLabel.Text = "Gravity: Down";

        Entities =
            GetTree().GetNodesInGroup("gravitons").Cast<EntityBase>().ToList();

        TickTimer.Connect("timeout", new Callable(this, nameof(OnTick)));
        TickTimer.WaitTime = 0.2f;
        AddChild(TickTimer);
        TickTimer.Start();
    }

    private void OnTick()
    {
        if (Entities.All(x => !x.CanFall))
        {
            Player.Controllable = true;

            if(AnyEntityHasMovedThisTick) {
                SoundPlayer.PlaySound("land");
                AnyEntityHasMovedThisTick = false;
            }

            TickTimer.Stop();
        }
        else
        {
            Player.Controllable = false;
            OnTickEventHandler.Invoke(this, null);
            SoundPlayer.PlaySound("fall");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void UpdateRotation(int degrees)
    {
        if (degrees > 0)
            _selectedGravity++;
        else
            _selectedGravity--;

        if (_selectedGravity > Gravities.Length - 1)
            _selectedGravity = 0;
        else if (_selectedGravity < 0)
            _selectedGravity = Gravities.Length - 1;

        Gravity = Gravities[_selectedGravity];

        switch (Gravity)
        {
            case Vector2 v when v.Equals(Vector2.Down):
                GravityLabel.Text = "Gravity: Down";
                break;
            case Vector2 v when v.Equals(Vector2.Right):
                GravityLabel.Text = "Gravity: Right";
                break;
            case Vector2 v when v.Equals(Vector2.Up):
                GravityLabel.Text = "Gravity: Up";
                break;
            case Vector2 v when v.Equals(Vector2.Left):
                GravityLabel.Text = "Gravity: Left";
                break;
        }

        TickTimer.Start();
    }

    public EntityBase GetEntityAtTile(Vector2i coords)
        => Entities.Find(x => x.MapPosition == coords);
}