using System.Collections.Generic;
using System.Linq;
using Code.Actors.Npc.Enemies;
using Code.Actors.Player.Settings;
using Code.Boot.Logging;
using Code.Environment;
using Code.Helpers.GlobalExtensions;
using Code.ScriptableObjects.Npc;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Code.Boot.Systems
{
    public class NpcActorsSystem : BaseSystem
    {
        #region Spawn related fields
        
        public bool IsWaveDefeated;

        [SerializeField] private Transform activeEnemiesContainer;
        [SerializeField] private Transform poolEnemiesContainer;
        [Inject] private NpcActorsLevelCollection _levelCollection;

        private RoomSettings _currentRoomSettings;
        private int _nextRoomIndex;
        private int _nextWaveIndex;
        private List<SpawnPoint> _spawnPoints;
        private NpcSpawnWave _currentWave;
        private float _spawnRateTimer;
        private int _enemiesToSpawnInThisWave;
        
        #endregion Spawn related fields

        private List<TestMeleeNpcActor> _enemiesToSpawnPool;
        private List<TestMeleeNpcActor> _activeEnemies;
        

        public void Init()
        {
            _nextRoomIndex = 0;
            _nextWaveIndex = 0;
            _enemiesToSpawnPool = CreateEnemiesPool();
            _activeEnemies = new List<TestMeleeNpcActor>(_enemiesToSpawnPool.Count);
            IsWaveDefeated = true;

            /*
            _activeEnemies = GetComponentsInChildren<TestMeleeNpcActor>().ToList();
            _activeEnemies.ForEach(a => a.Init());*/
            
            DebugExtension.InitNotice("None Player Actors System - OK");
        }

        public void SetSpawnPoints(List<SpawnPoint> spawnPoints)
        {
            _spawnPoints = spawnPoints;
            if(_spawnPoints.Count == 0)
                DebugExtension.Warning("There is no spawn positions given for npc system!");
        }

        public void SetActiveNextRoom()
        {
            if (_levelCollection.rooms.Length < _nextRoomIndex)
                DebugExtension.Warning($"There is no npc data for room index {_nextRoomIndex}");

            _currentRoomSettings = _levelCollection.rooms[_nextRoomIndex];
            _nextRoomIndex++;
        }
        
        public void PrepareNextWaveForSpawn()
        {
            if (_enemiesToSpawnPool.Count == 0)
                return;
            
            if (_currentRoomSettings.waves.Length < _nextWaveIndex)
                DebugExtension.Warning($"There is no npc data for wave index {_nextWaveIndex} for room index {_nextRoomIndex - 1}");
            
            _currentWave = _currentRoomSettings.waves[_nextWaveIndex];
            _spawnRateTimer = 0;
            _enemiesToSpawnInThisWave = _currentRoomSettings.waves.Sum(w => 
                    w.spawnSettings.Sum(ss => ss.amount)
            );
            
            IsWaveDefeated = false;
            _nextWaveIndex++;
        }

        public void EnemiesAct()
        {
            if (_activeEnemies.Count == 0 && _enemiesToSpawnInThisWave == 0)
            {
                IsWaveDefeated = true;
                return;
            }
            
            _activeEnemies.ForEach(ae => ae.Act());
        }
        
        public void EnemiesSpawn()
        {
            if (_enemiesToSpawnPool.Count == 0)
                return;
            
            _spawnRateTimer += Time.deltaTime;

            var spawnPoint = _spawnPoints.Where(sp => sp.IsAvailableForSpawn()).ToList().GetRandomElement();
            if (
                spawnPoint != null && 
                _enemiesToSpawnInThisWave > 0 
                && _spawnRateTimer >= _currentWave.spawnRate
            ) {
                _enemiesToSpawnInThisWave--;
                var newEnemy = _enemiesToSpawnPool.First();
                _activeEnemies.Add(newEnemy);
                _enemiesToSpawnPool.Remove(newEnemy);
                newEnemy.transform.SetParent(activeEnemiesContainer);
                var position = spawnPoint.gameObject.transform.position;
                if (NavMesh.SamplePosition(position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                {
                    newEnemy.transform.position = hit.position;
                }
                else
                {
                    Debug.LogError("NavMeshAgent is not placed on a NavMesh");
                }
                
                newEnemy.Init();
                
                _spawnRateTimer = 0;
            }
        }

        private List<TestMeleeNpcActor> CreateEnemiesPool()
        {
            var result = new List<TestMeleeNpcActor>();
            foreach (var levelCollectionRoom in _levelCollection.rooms)
            {
                foreach (var npcSpawnWave in levelCollectionRoom.waves)
                {
                    foreach (var npcSpawnSetting in npcSpawnWave.spawnSettings)
                    {
                        for (var i = 0; i < npcSpawnSetting.amount; i++)
                        {
                            var newEnemy = Instantiate(npcSpawnSetting.prefub, poolEnemiesContainer);
                            result.Add(newEnemy);
                        }
                    }
                }
            }

            return result;
        }
    }
}