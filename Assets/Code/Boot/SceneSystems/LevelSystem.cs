using System;
using System.Collections;
using Code.Boot.Logging;
using Code.Boot.SceneSystems;
using Code.ScriptableObjects;
using UnityEngine;
using VContainer;

namespace Code.Boot.Systems
{
    public class LevelSystem : BaseSystem
    {
        [SerializeField] 
        [Tooltip("Время до инициализации уровня")]
        private float timeUntilInit = 1f;

        [Inject] private NpcActorsSystem _npcActorsSystem;
        [Inject] private EnvironmentSystem _environmentSystem;
        [Inject] private PlayerCharacterControlSystem _playerCharacterControlSystem;
        [Inject] private PlayerControlledActorContainer _playerControlledActorContainer;

        private bool initiated;
        
        private void Start()
        {
            DebugExtension.InitNotice("Level init started");
            // Экран загрузки
            StartCoroutine(InitStartCoroutine());
        }

        private IEnumerator InitStartCoroutine()
        {
            yield return new WaitForSeconds(timeUntilInit);
            Init();
        }
        
        private void Init()
        {
            DebugExtension.InitNotice("Systems initialization");
            
            _environmentSystem.Init();
            
            _npcActorsSystem.Init();
            _npcActorsSystem.SetSpawnPoints(_environmentSystem.GetSpawnPoints());
            _npcActorsSystem.SetActiveNextRoom();
            
            _playerCharacterControlSystem.Init();
            var playerActor = _playerControlledActorContainer.CreateInstance();
            _playerCharacterControlSystem.SetControlledActor(playerActor);

            initiated = true;
            
            DebugExtension.InitNotice("All systems initialized");
            
            // Убираем экран загрузки
        }

        private void Update()
        {
            if (!initiated)
                return;

            ProcessPlayer();
            ProcessNpc();
        }

        private void ProcessPlayer()
        {
            _playerCharacterControlSystem.Act();
        }
        
        private void ProcessNpc()
        {
            if (_npcActorsSystem.IsWaveDefeated)
                _npcActorsSystem.PrepareNextWaveForSpawn();
            
            _npcActorsSystem.EnemiesSpawn();
            _npcActorsSystem.EnemiesAct();
        }
    }
}
