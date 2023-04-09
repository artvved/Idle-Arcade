using DefaultNamespace;
using Game.Component;

using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using ScriptableData;
using UnityEngine;
using UnityEngine.Scripting;


namespace Game.System
{
    public class MoveJoystickInputSystem : EcsUguiCallbackSystem,IEcsInitSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsCustomInject<SceneData> sceneData = default;

        private readonly EcsPoolInject<PlayerStatsComponent> playerStatsPool = default;
        private readonly EcsPoolInject<SpeedComponent> speedPool = default;
        private readonly EcsPoolInject<DirectionComponent> directionPool = default;
        private readonly EcsPoolInject<AnimatorComponent> animPool = default;
        
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
           
            playerFilter = world.Filter<PlayerTag>().Exc<CantMoveComponent>().End();
        }

          
        [Preserve]
        [EcsUguiDragStartEvent(Idents.Ui.MoveJoystick, Idents.EVENT_WORLD)]
        void OnDragStart (in EcsUguiDragStartEvent e) {
            foreach (var ent in playerFilter)
            {
                AddToMoving(ent);
            }
        }
        
        [Preserve]
        [EcsUguiDragMoveEvent(Idents.Ui.MoveJoystick, Idents.EVENT_WORLD)]
        void OnDrag (in EcsUguiDragMoveEvent e) {
            foreach (var ent in playerFilter)
            {
                UpdMoving(ent);
            }
        }

        private void AddToMoving(int ent)
        {
            var maxSpeed =  GetMaxSpeed(ent);
            var joystickDirection = GetJoystickDir();
            speedPool.Value.Add(ent).Value = joystickDirection.normalized.magnitude * maxSpeed;
            directionPool.Value.Add(ent).Value = new Vector3(joystickDirection.x, 0, joystickDirection.y);
            AnimateMove(ent,joystickDirection.normalized.magnitude * 1,1);
        }
        
        private void UpdMoving(int ent)
        {
            var maxSpeed = GetMaxSpeed(ent);
            var joystickDirection = GetJoystickDir();
            speedPool.Value.Get(ent).Value = joystickDirection.magnitude *  maxSpeed;
            directionPool.Value.Get(ent).Value = new Vector3(joystickDirection.x, 0, joystickDirection.y);
         
        }

        private float GetMaxSpeed(int ent)
        {
            return playerStatsPool.Value.Get(ent).MaxSpeed;
        }
        
        private Vector2 GetJoystickDir()
        {
            return sceneData.Value.Joystick.Direction;
        }

        private void AnimateMove(int ent,float curVel,float maxVelocity)
        {
            var animator = animPool.Value.Get(ent).Value;
            animator.SetBool("Move",true);
            animator.speed = curVel / (float) maxVelocity;
        }
        private void AnimateStopMove(int ent)
        {
            var animator = animPool.Value.Get(ent).Value;
            animator.SetBool("Move",false);
        }


        [Preserve]
        [EcsUguiDragEndEvent(Idents.Ui.MoveJoystick, Idents.EVENT_WORLD)]
        void OnDragEnd (in EcsUguiDragEndEvent e) {
            foreach (var ent in playerFilter)
            {
                speedPool.Value.Del(ent);
                directionPool.Value.Del(ent);
                AnimateStopMove(ent);
            }
        }

    }
}