using Godot;
using System;


namespace Entities;
public partial class IceBlock : EntityBase, IGlider
{
    public RayCast2D RayUp { get; set; }
    public RayCast2D RayRight { get; set; }
    public RayCast2D RayDown { get; set; }
    public RayCast2D RayLeft { get; set; }
    public bool TouchesWall => RayTouchesWall();

    private bool RayTouchesWall()
    {
        return
            RayUp.IsColliding() ||
            RayRight.IsColliding() ||
            RayDown.IsColliding() ||
            RayLeft.IsColliding();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        RayUp = GetNode<RayCast2D>("Up");
        RayRight = GetNode<RayCast2D>("Right");
        RayDown = GetNode<RayCast2D>("Down");
        RayLeft = GetNode<RayCast2D>("Left");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        GD.Print(TouchesWall);

        if (ParentMap.Gravity == Vector2.Up || ParentMap.Gravity == Vector2.Down)
        {
            RayRight.Enabled = true;
            RayLeft.Enabled = true;
            RayUp.Enabled = false;
            RayDown.Enabled = false;
        }
        else if (ParentMap.Gravity == Vector2.Left || ParentMap.Gravity == Vector2.Right)
        {
            RayRight.Enabled = false;
            RayLeft.Enabled = false;
            RayUp.Enabled = true;
            RayDown.Enabled = true;
        }
    }
}
