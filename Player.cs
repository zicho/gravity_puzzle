using System;
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

            if(Input.IsActionJustPressed("move_left")) {
                Move(Vector2.Left);
            }

            if(Input.IsActionJustPressed("move_right")) {
                Move(Vector2.Right);
            }
        }
    }

    private void CamRotate(int degrees)
    {
        var tween = CreateTween();
        tween.Finished += () => ParentMap.UpdateRotation(degrees);

        tween
            .TweenProperty(Cam,
            "rotation",
            Cam.Rotation + Mathf.DegToRad(degrees),
            0.2f);
        tween.Play();
    }
}
