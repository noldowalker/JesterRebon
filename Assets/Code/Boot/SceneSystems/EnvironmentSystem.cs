using System.Collections.Generic;
using System.Linq;
using Code.Boot.Logging;
using Code.Boot.Systems;
using Code.Environment;
using UnityEngine;

namespace Code.Boot.SceneSystems
{
    public class EnvironmentSystem : BaseSystem
    {
        [SerializeField] private Transform spawnPointsContainer;
        
        private List<SpawnPoint> _spawnPoints;
        
        public void Init()
        {
            ValidateOnInit();
            CollectAndInitAllSpawnPoints();
            DebugExtension.InitNotice("Environment System - OK");
        }

        public List<SpawnPoint> GetSpawnPoints()
        {
            return _spawnPoints;
        }
        
        private void ValidateOnInit()
        {
            if (spawnPointsContainer == null)
            {
                DebugExtension.Warning("spawnPointsContainer in Environment System not set!");
                spawnPointsContainer = transform;
            }
        }

        private void CollectAndInitAllSpawnPoints()
        {
            _spawnPoints = GetComponentsInChildren<SpawnPoint>().ToList();
            if(_spawnPoints.Count == 0)
                DebugExtension.Warning("Environment System cant find any spawn points on scene!");
            
            _spawnPoints.ForEach(sp => sp.transform.SetParent(spawnPointsContainer));
        }
    }
}