using Godot;

namespace Entities
{
    internal interface IGlider
    {
        public RayCast2D RayUp { get; set; }
        public RayCast2D RayRight { get; set; }
        public RayCast2D RayDown { get; set; }
        public RayCast2D RayLeft { get; set; }
        public bool TouchesWall { get; }
    }
}