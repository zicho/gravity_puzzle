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
    private bool _adjustPosition;

    public override void _Ready()
    {
        if (Active)
        {
            AddToGroup("gravitons");
            // ParentMap.OnTickEventHandler += OnTickHappened;
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

    public Rect2 GetSpriteRect() => GetNode<Sprite2D>("Sprite").GetRect();

    public bool AllIsWithinBounds()
    {
        var rect = GetSpriteRect();

        var edge = new Vector2i(
                        (int)GlobalPosition.x + (int)rect.Size.x - 128,
                        (int)GlobalPosition.y + (int)rect.Size.y - 128
                        );

        if (Name == "Box")
        {
            GD.Print(GlobalPosition);
            GD.Print(edge);
            GD.Print(ParentMap.LocalToMap((Vector2i)GlobalPosition) == ParentMap.LocalToMap(edge));
            GD.Print("Edge: " + ParentMap.LocalToMap(edge));
            GD.Print("Pos: " + ParentMap.LocalToMap((Vector2i)GlobalPosition));
        }

        return ParentMap.LocalToMap((Vector2i)GlobalPosition) == ParentMap.LocalToMap(edge);

        // if (Name == "Box")
        // {
        //     GD.Print(ParentMap.LocalToMap((Vector2i)GlobalPosition));

        //     var edge = new Vector2i(
        //         (int)GlobalPosition.x + (int)rect.Size.x,
        //         (int)GlobalPosition.y + (int)rect.Size.y
        //         );

        //     GD.Print(ParentMap.LocalToMap((Vector2i)GlobalPosition));
        //     GD.Print(ParentMap.LocalToMap(edge));
        // }

        // return
        //     ParentMap.MapToLocal((Vector2i)rect.Position) == MapPosition &&
        //     ParentMap.MapToLocal((Vector2i)rect.End) == MapPosition;
    }

    public override void _Process(double delta)
    {
        if (!ParentMap.PlayerControl && CanFall)
        {
            _adjustPosition = true;
            Position += ParentMap.Gravity * 32;
        }

        if(_adjustPosition && !CanFall) {
            Position += ParentMap.Gravity * 32;
            _adjustPosition = false;
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
            var checkEntityBelow = ParentMap.GetEntityAtTile(coords);
            if (checkEntityBelow == null) return true;
            else return checkEntityBelow.CanFall;
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
            tween.Play();
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
            if (checkEntity != null) return checkEntity.CanFall;
        }

        return tileIsEmpty;
    }
}