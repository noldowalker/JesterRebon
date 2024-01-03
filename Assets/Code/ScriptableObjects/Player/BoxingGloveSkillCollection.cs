using System;
using Code.Actors.Npc.Enemies;
using UnityEngine;

namespace Code.ScriptableObjects.Player
{
    [CreateAssetMenu(fileName = "BoxingGloveSkill", menuName = "Scriptable Objects/Settings/Create BoxingGlove Skill Collection", order = 0)]
    public class BoxingGloveSkillCollection : ScriptableObject
    {
        [SerializeField] public BoxingGloveSettings[] settings;

        public BoxingGloveSettings GetByLevel(int level)
        {
            foreach (BoxingGloveSettings item in settings)
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