using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Godot;

namespace Entities;

public partial class Player : EntityBase
{
    public Camera2D Cam => GetNode<Camera2D>("Camera2D");

    public AnimationPlayer AnimationPlayer { get; private set; }

    public bool Controllable;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Controllable)
        {
            if (Input.IsActionJustPressed("rotate_right"))
            {
                CamRotate(90);
            }

            if (Input.IsActionJustPressed("rotate_left"))
            {
                CamRotate(-90);
            }

            if (Input.IsActionJustPressed("move_left") && !Moving)
            {
                MoveWithPush(GetGravityNeutralDirection(Vector2.Left));
            }

            if (Input.IsActionJustPressed("move_right") && !Moving)
            {
                MoveWithPush(GetGravityNeutralDirection(Vector2.Right));
            }
        }
    }

    private void MoveWithPush(Vector2 gravityNeutralDirection)
    {
        var posToCheck = (Vector2i)MapPosition + (Vector2i)gravityNeutralDirection;

        bool collisionCheck = true;
        var affectedEntities = new List<EntityBase>();

        while (collisionCheck)
        {
            var entity = ParentMap.GetEntityAtTile(posToCheck);
            if (entity != null)
            {
                affectedEntities.Add(entity);
                posToCheck += (Vector2i)gravityNeutralDirection;
            }
            else
            {
                collisionCheck = false;
            }
        }

        // if(affectedEntities.Any() && affectedEntities.All(x => x is IGlider)) {
        //     SoundPlayer.PlaySound("ice_glide");
        // }

        if (affectedEntities.Count == 0)
        {
            Move(gravityNeutralDirection);
        }
        else
        {
            var lastEntity = affectedEntities.Last();
            var allCanMove = false;

            if (lastEntity?.CheckDirection(
                (Vector2i)lastEntity.MapPosition + (Vector2i)gravityNeutralDirection
            ) == true)
            {
                allCanMove = true;
            } else {
                Move(gravityNeutralDirection);
            }

            if (allCanMove)
            {
                foreach (var ent in affectedEntities)
                {
                    ent.Move(gravityNeutralDirection, true);
                }

                Move(gravityNeutralDirection, true);
            }
        }

        // var entity = ParentMap.GetEntityAtTile(posToCheck);
        // if (entity != null)
        // {
        //     entity.Move(gravityNeutralDirection);
        //     Move(gravityNeutralDirection, true);
        // } else {
        //     Move(gravityNeutralDirection);
        // }

    }

    private Vector2 GetGravityNeutralDirection(Vector2 dir)
    {
        if (dir == Vector2.Left && ParentMap.Gravity != Vector2.Down)
        {
            if (ParentMap.Gravity == Vector2.Left) return dir.Rotated(Mathf.DegToRad(90));
            else if (ParentMap.Gravity == Vector2.Up) return dir.Rotated(Mathf.DegToRad(180));
            else if (ParentMap.Gravity == Vector2.Right) return dir.Rotated(Mathf.DegToRad(-90));
        }

        if (dir == Vector2.Right && ParentMap.Gravity != Vector2.Down)
        {
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

    public override void Move(Vector2 direction, bool force = false)
    {
        base.Move(direction, force);
        ParentMap.TickTimer.Start();
    }
}
