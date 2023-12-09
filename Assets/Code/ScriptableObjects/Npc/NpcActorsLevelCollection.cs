using System;
using Code.Actors.Npc.Enemies;
using Code.Actors.Npc.Enemies.Settings;
using UnityEngine;

namespace Code.ScriptableObjects.Npc
{
    [CreateAssetMenu(fileName = "UntitledNpcActorsLevelCollection", menuName = "Scriptable Objects/Settings/Create Npc Actors Level Collection", order = 0)]
    public class NpcActorsLevelCollection: ScriptableObject
    {
        [SerializeField] public RoomSettings[] rooms;
    }

    [Serializable]
    public class RoomSettings
    {
        public NpcSpawnWave[] waves;
    }
}