using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Game.Component;
using Game.Service;
using Game.System;
using Game.System.Timing;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity.Ugui;
using ScriptableData;
using UnityEngine;

public class Startup : MonoBehaviour
{
    private EcsWorld world;
    private EcsSystems systems;
    private EcsSystems phisSystems;
    

    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private StaticData staticData;
    void Start()
    {
        world = new EcsWorld();
        var eventWorld = new EcsWorld();
        systems = new EcsSystems(world);
        phisSystems = new EcsSystems(world);
        EcsPhysicsEvents.ecsWorld = eventWorld;
        
        phisSystems.AddWorld(eventWorld,Idents.EVENT_WORLD)
            
            .Add(new MoveApplySystem())
            .Add(new LookAtPlayerSystem())
            .Add(new CustomerRotationSystem())
            .Add(new RotationApplySystem())
            .Add(new CollisionSystem())
            
            .Add(new CustomerReachedSystem())
            .Add(new MoveToStackSystem())
           
            
            .DelHerePhysics(Idents.EVENT_WORLD)
          
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (Idents.EVENT_WORLD))
#endif
            .Inject(new Fabric(world,eventWorld,staticData,sceneData))
            .Inject(sceneData)
            .Inject(staticData)
            .Inject(new MovementService(world))
            .Inject(new AnimationService(world))
            .Init();
        
        systems
            .AddWorld(eventWorld,Idents.EVENT_WORLD)
            .Add(new SpawnPlayerWithCameraSystem())
            .Add(new SpawnLevelSystem())
            .Add(new MoveJoystickInputSystem())
            
            .Add(new SpawnCustomerTickSystem())
            .Add(new SpawnFetusTickSystem())
            

            .Add(new TargetCustomerSystem())
            .Add(new HarvestSystem())
            .Add(new StackingSystem())
            .Add(new StackFinishedReactionSystem())
            .Add(new FullBoxSystem())
          
            .Add(new ReachedSystem())
            
            .Add(new GetMoneySystem())
            .Add(new DestroyDeadSystem())
          

            //.Add(new TickSystem())
            .Add(new UpdateCoinsSystem())
            .Add(new UpdateUIViewSystem())
          
            
            .DelHere<CoinsChangedEventComponent>(Idents.EVENT_WORLD)
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (Idents.EVENT_WORLD))
#endif
            .Inject(new Fabric(world,eventWorld,staticData,sceneData))
            .Inject(sceneData)
            .Inject(staticData)
            .Inject(new MovementService(world))
            .Inject(new AnimationService(world))
            .InjectUgui(sceneData.EcsUguiEmitter,Idents.EVENT_WORLD)
            .Init();
    }

    private int i ;
    private int j ;
    void Update()
    {
        systems?.Run();
    }
    
    void FixedUpdate()
    {
        phisSystems?.Run();
    }

    private void OnDestroy()
    {
        if (systems!=null)
        {
            systems.Destroy();
            systems = null;
        }
        if (phisSystems!=null)
        {
            phisSystems.Destroy();
            phisSystems = null;
        }

        if (world!=null)
        {
            world.Destroy();
            world = null;
        }  
        EcsPhysicsEvents.ecsWorld = null;

      
    }
}
