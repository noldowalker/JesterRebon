using Code.ScriptableObjects.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEntity
{
    public AbstractSkill skill;
    public PlayerSkillEnum name;
    public string keyBind;
    public float coolDownTime;
    public Vector3 spawnPointOffset;
    public bool unlocked;
    public int currentLevel;
    
    public SkillEntity(SkillEntityPresetCollection preset)
    {
        skill = preset.skill;
        name = preset.skillName;
        coolDownTime = preset.coolDownTime;
        spawnPointOffset = preset.spawnPointOffset;
        keyBind = "";
        unlocked = false;
        currentLevel = 1;
    }
}
