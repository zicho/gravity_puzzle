using System;
using System.Linq;
using Core;
using Godot;

namespace Entities;

public abstract partial class EntityBase : Node2D
{
    public Map ParentMap => GetParent<Map>();
    public Vector2 MapPosition => ParentMap.LocalToMap(Position);

    public override void _Ready()
    {
        AddToGroup("gravitons");

        ParentMap.OnTickEventHandler += OnTickHappened;
    }
    private void OnTickHappened(object sender, EventArgs e)
    {
        if (CanFall)
        {
            Fall();
        }
    }

    public bool CanFall => DetermineMovement();

    private bool DetermineMovement()
    {
        var coords =
            new Vector2i(ParentMap.LocalToMap(Position).x +
                (int)ParentMap.Gravity.x,
                ParentMap.LocalToMap(Position).y + (int)ParentMap.Gravity.y);

        var tileIsEmpty = ParentMap.GetCellSourceId(0, coords) == -1;

        if (tileIsEmpty)
        {
            var checkEntityBelow = ParentMap.Entities.Find(x => x.MapPosition == coords);
            if (checkEntityBelow == null) return true;
            if (checkEntityBelow?.CanFall == true) return true;
            if (checkEntityBelow?.CanFall == false) return false;
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
            var tween = CreateTween();

            // HACK?
            if(this is Player) {
                SoundPlayer.PlaySound("step");
            }

            tween
                .TweenProperty(this,
                "position",
                ParentMap.MapToLocal((Vector2i)newPos),
                0.1f)
                .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
            tween.Play();
        }
    }

    public bool CheckDirection(Vector2 newPos)
    {
        var coords = new Vector2i((int)newPos.x, (int)newPos.y);

        var tileIsEmpty = ParentMap.GetCellSourceId(0, coords) == -1;

        if (tileIsEmpty)
        {
            var checkForEntity = ParentMap.Entities.Find(x => x.MapPosition == coords);
            if (checkForEntity == null) return true;
            if (checkForEntity?.CanFall == true) return true;
            if (checkForEntity?.CanFall == false) return false;
        }

        return tileIsEmpty;
    }
}