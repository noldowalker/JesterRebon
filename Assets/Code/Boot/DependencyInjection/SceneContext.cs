using Code.Actors.Player.Settings;
using Code.Boot.SceneSystems;
using Code.Boot.Systems;
using Code.ScriptableObjects;
using Code.ScriptableObjects.Npc;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Code.Boot.DependencyInjection
{
    public class SceneContext : LifetimeScope
    {
        [SerializeField] private PlayerControlledActorContainer playerControlledActorContainer; 
        [SerializeField] private NpcActorsLevelCollection npcActorsLevelCollection; 
            
        protected override void Configure(IContainerBuilder builder)
        {
            #region Global Systems for Injection
            
            builder.RegisterComponentInHierarchy<LevelSystem>();
            builder.RegisterComponentInHierarchy<NpcActorsSystem>();
            builder.RegisterComponentInHierarchy<PlayerCharacterControlSystem>();
            builder.RegisterComponentInHierarchy<EnvironmentSystem>();
            
            #endregion Global Systems for Injection
            
            #region Scriptable Objects for Injection

            
            builder.RegisterInstance(playerControlledActorContainer);
            builder.RegisterInstance(npcActorsLevelCollection);

            #endregion Scriptable Objects for Injection
            
            #region Create and register scoped exemplars
            
            builder.Register<JesterInputActionsAsset>(Lifetime.Scoped);
            
            #endregion Global Systems for Injection
        }  
    }
}