using Leopotam.EcsLite;

namespace Game.Component
{
    public struct HarvestEventComponent
    {
        public EcsPackedEntity Collider;
        public EcsPackedEntity Sender;
    }
}