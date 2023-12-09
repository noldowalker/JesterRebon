using System;
using Code.Actors.Npc.Enemies;
using UnityEngine;

namespace Code.ScriptableObjects.Npc
{
    [CreateAssetMenu(fileName = "UntitledNpcSpawnGroup", menuName = "Scriptable Objects/Collection/Npc/Create Npc Spawn Group", order = 0)]
    public class NpcSpawnWave : ScriptableObject
    {
        [SerializeField] public float spawnRate;
        [SerializeField] public NpcSpawnSetting[] spawnSettings;
    }

    [Serializable]
    public class NpcSpawnSetting
    {
        public TestMeleeNpcActor prefub;
        public int amount;
    }
}