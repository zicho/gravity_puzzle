using System;
using Core;
using Godot;

namespace Entities;

public partial class Player : EntityBase
{
    public Camera2D Cam => GetNode<Camera2D>("Camera2D");

    public bool Controllable;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Controllable)
        {
            if (Input.IsActionJustPressed("rotate_right"))
            {
                Controllable = false;
                CamRotate(90);
            }

            if (Input.IsActionJustPressed("rotate_left"))
            {
                Controllable = false;
                CamRotate(-90);
            }

            if (Input.IsActionJustPressed("move_left"))
            {
                Controllable = false;
                Move(GetGravityNeutralDirection(Vector2.Left));
            }

            if (Input.IsActionJustPressed("move_right"))
            {
                Controllable = false;
                Move(GetGravityNeutralDirection(Vector2.Right));
            }
        }
    }

    private Vector2 GetGravityNeutralDirection(Vector2 dir)
    {
        if (dir == Vector2.Left && ParentMap.Gravity != Vector2.Down)
        {
            if (ParentMap.Gravity == Vector2.Left) return dir.Rotated(Mathf.DegToRad(90));
            else if (ParentMap.Gravity == Vector2.Up) return dir.Rotated(Mathf.DegToRad(180));
            else if (ParentMap.Gravity == Vector2.Right) return dir.Rotated(Mathf.DegToRad(-90));
        }

        if (dir == Vector2.Right && ParentMap.Gravity != Vector2.Down) {
            if (ParentMap.Gravity == Vector2.Left) return dir.Rotated(Mathf.DegToRad(-270));
            else if (ParentMap.Gravity == Vector2.Up) return dir.Rotated(Mathf.DegToRad(-180));
            else if (ParentMap.Gravity == Vector2.Right) return dir.Rotated(Mathf.DegToRad(270));
        }

        return dir;
    }

    private void CamRotate(int degrees)
    {
        SoundPlayer.PlaySound("swoosh");

        var tween = CreateTween();
        var tween2 = CreateTween();

        tween.Finished += () => ParentMap.UpdateRotation(degrees);

        tween2
            .TweenProperty(GetNode<Sprite2D>("Sprite"),
            "rotation",
            Cam.Rotation + Mathf.DegToRad(degrees),
            0.2f);

        tween
            .TweenProperty(Cam,
            "rotation",
            Cam.Rotation + Mathf.DegToRad(degrees),
            0.2f);

        tween2.Play();
        tween.Play();
    }

    public override void  Move(Vector2 direction) {
        base.Move(direction);
        ParentMap.TickTimer.Start();
    }
}
