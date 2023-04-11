using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Component
{
    public struct QueueComponent
    {
        public List<EcsPackedEntity> Value;
        public Transform Target;
    }
}