using Godot;
using System;


namespace Entities;
public partial class IceBlock : EntityBase
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void Move(Vector2 direction, bool force = false)
    {
        var newPos = ParentMap.LocalToMap(Position) + direction;

        var canGlide = true;

        if (CheckDirection(newPos) || force)
        {
            var tween = CreateTween();

            tween
                .TweenProperty(this,
                "position",
                ParentMap.MapToLocal((Vector2i)newPos),
                0.1f)
                .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
            tween.Play();

			tween.Finished += () => Move(direction);
        }
    }
}
