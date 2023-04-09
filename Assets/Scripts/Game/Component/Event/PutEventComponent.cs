using Leopotam.EcsLite;

namespace Game.Component
{
    public struct PutEventComponent
    {
        public EcsPackedEntity Collider;
        public EcsPackedEntity Sender;
    }
}