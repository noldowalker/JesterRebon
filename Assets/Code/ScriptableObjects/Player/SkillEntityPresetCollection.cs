using System;
using System.Collections;
using Code.Actors.Npc.Enemies;
using UnityEngine;

namespace Code.ScriptableObjects.Player
{
    [CreateAssetMenu(fileName = "SkillEntityPreset", menuName = "Scriptable Objects/Settings/Create SkillEntity Preset Collection", order = 0)]
    public class SkillEntityPresetCollection : ScriptableObject
    {
        public AbstractSkill skill;
        public PlayerSkillEnum skillName;
        public Vector3 spawnPointOffset;
        public float coolDownTime;
    }

}