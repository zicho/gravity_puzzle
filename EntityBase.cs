using System;
using System.Linq;
using Core;
using Godot;

namespace Entities;

public abstract partial class EntityBase : Node2D
{
    public Map ParentMap => GetParent<Map>();
    public Vector2 MapPosition => ParentMap.LocalToMap(Position);

    [Export]
    private bool Active = true;

    public override void _Ready()
    {
        if (Active)
        {
            AddToGroup("gravitons");
            ParentMap.OnTickEventHandler += OnTickHappened;
        }
        else
        {
            Modulate = new Color("#79797974");
        }
    }

    private void OnTickHappened(object sender, EventArgs e)
    {
        if (CanFall)
        {
            Fall();
        }
    }

    public bool CanFall => DetermineMovement();

    public bool Moving { get; set; }

    private bool DetermineMovement()
    {
        var coords =
            new Vector2i(ParentMap.LocalToMap(Position).x +
                (int)ParentMap.Gravity.x,
                ParentMap.LocalToMap(Position).y + (int)ParentMap.Gravity.y);

        var tileIsEmpty = ParentMap.GetCellSourceId(0, coords) == -1;

        if (tileIsEmpty)
        {
            var checkEntityBelow = ParentMap.GetEntityAtTile(coords);
            if (checkEntityBelow == null) return true;
            if (checkEntityBelow != null) return checkEntityBelow.CanFall;
        }

        return tileIsEmpty;
    }

    public void Fall()
    {
        ParentMap.AnyEntityHasMovedThisTick = true;
        var newPos = ParentMap.LocalToMap(Position) + ParentMap.Gravity;

        // Position = ParentMap.MapToLocal((Vector2i) newPos);
        var tween = CreateTween();

        tween
            .TweenProperty(this,
            "position",
            ParentMap.MapToLocal((Vector2i)newPos),
            0.1f)
            .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        tween.Play();
    }

    public virtual void Move(Vector2 direction, bool force = false)
    {
        var newPos = ParentMap.LocalToMap(Position) + direction;

        if (CheckDirection(newPos) || force)
        {
            Moving = true;

            var tween = CreateTween();

            // HACK?
            if (this is Player player)
            {
                SoundPlayer.PlaySound("step");
                player.AnimationPlayer.Play("step_right");
            }

            tween
                .TweenProperty(this,
                "position",
                ParentMap.MapToLocal((Vector2i)newPos),
                0.1f)
                .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);

            tween.Finished += () => Moving = false;

            tween.Play();

            if (this is IGlider && CheckDirection(newPos))
            {
                

                tween.Finished += () =>
                {
                    if (!CanFall)
                    {
                        Move(direction);
                    }
                    else
                    {
                        ParentMap.TickTimer.Start();
                        Moving = false;
                    }
                };
            }
        }
    }

    public bool CheckDirection(Vector2 newPos)
    {
        var coords = new Vector2i((int)newPos.x, (int)newPos.y);

        var tileIsEmpty = ParentMap.GetCellSourceId(0, coords) == -1;

        if (tileIsEmpty)
        {
            var checkEntity = ParentMap.GetEntityAtTile(coords);
            if (checkEntity == null) return true;
            if (checkEntity != null) return checkEntity.CanFall || checkEntity is IGlider;
        }

        return tileIsEmpty;
    }
}