using System;
using Code.Actors.Npc.Enemies;
using UnityEngine;

namespace Code.ScriptableObjects.Player
{
    [CreateAssetMenu(fileName = "DancingAuraSkill", menuName = "Scriptable Objects/Settings/Create DancingAura Skill Collection", order = 0)]
    public class DancingAuraSkillCollection : ScriptableObject
    {
        [SerializeField] public DancingAuraSettings[] settings;

        public DancingAuraSettings GetByLevel(int level)
        {
            foreach (DancingAuraSettings item in settings)
            {
                if (item.level == level)
                {
                    return item;
                }
            }
            return null;
        }
        
    }

}