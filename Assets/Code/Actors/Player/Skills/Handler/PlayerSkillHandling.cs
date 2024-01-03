using Code.Boot.Systems;
using Code.ScriptableObjects.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSkillHandling : BaseSystem
{
    [SerializeField] private SkillEntityPresetCollection[] initialSkillPull;
    [SerializeField] private List<SkillEntity> skillPull;
    [SerializeField] private List<SkillEntity> activeSkills;

    private bool[] coolDownSkills;


    public void Init()
    {
        InitSkills();

        //Unlock starting skills
        UnlockSkill(PlayerSkillEnum.BOXING_GLOVE);
        UnlockSkill(PlayerSkillEnum.DANCING_AURA);
        //Bind starting skills
        BindActiveSkill(PlayerSkillEnum.BOXING_GLOVE, "1");
        BindActiveSkill(PlayerSkillEnum.DANCING_AURA, "2");

    }

    private void InitSkills()
    {
        skillPull = new List<SkillEntity>();
        activeSkills = new List<SkillEntity>();
        foreach(SkillEntityPresetCollection preset in initialSkillPull)
        {
            skillPull.Add(new SkillEntity(preset));
        }
        coolDownSkills = new bool[4];
    }


    public SkillEntity GetSkillByName(PlayerSkillEnum name)
    {
        foreach(SkillEntity item in skillPull)
        {
            if(item.name == name)
            {
                return item;
            }
        }
        return null;
    }

    public SkillEntity GetActiveSkillByName(PlayerSkillEnum name)
    {
        foreach (SkillEntity skill in activeSkills)
        {
            if (skill.name == name)
                return skill;
        }
        return null;
    }

    public SkillEntity GetSkillByKey(string key)
    {
        foreach (SkillEntity item in activeSkills)
        {
            if (item.keyBind == key)
            {
                return item;
            }
        }
        Debug.LogWarning("No such bound '" + key + "' from active skills list");
        return null;
    }
    public void UnlockSkill(PlayerSkillEnum name)
    {
        SkillEntity skill = GetSkillByName(name);
        skillPull.Remove(skill);
        skill.unlocked = true;
        skillPull.Add(skill);
    }

    public void UnBindActiveSkill(PlayerSkillEnum name)
    {
        var skill = GetActiveSkillByName(name);
        activeSkills.Remove(skill);
    }

    public void BindActiveSkill(PlayerSkillEnum name, string keyBind)
    {
        SkillEntity skill = GetSkillByName(name);
        if (!skill.unlocked)
        {
            Debug.LogError("Cannot bind skill, it`s still locked");
            return;
        }
        SkillEntity skillForReplace = GetSkillByKey(keyBind);
        if (skillForReplace != null)
        {
            activeSkills.Remove(skillForReplace);
        }
        skill.keyBind = keyBind;
        activeSkills.Add(skill);
    }

    public void LevelUpSkill(PlayerSkillEnum name)
    {
        var skill = GetSkillByName(name);
        if (!skill.unlocked)
        {
            Debug.LogError("Cannot upgrade skill, it`s still locked");
            return;
        }
        skillPull.Remove(skill);
        skill.currentLevel++;
        skillPull.Add(skill);
        var activeSkill = GetActiveSkillByName(name);
        if (activeSkill != null)
        {
            activeSkills.Remove(activeSkill);
            activeSkill.currentLevel++;
            activeSkills.Add(activeSkill);
        }
    }

    public int InterpretateBind(string key) //Key must be interptitated as one of the 4 active skill sockets
    {
        return int.Parse(key) - 1;
    }

    public void ActivateSkill(string key, Transform playerTransform)
    {
        var skill = GetSkillByKey(key);
        if (skill == null)
            return;
        int skillIdx = InterpretateBind(key);
        if (coolDownSkills[skillIdx])
        {
            Debug.LogWarning("Skill is cooling down. Cannot activate");
            return;
        }
        var instance = Instantiate(skill.skill, playerTransform.position + skill.spawnPointOffset, playerTransform.rotation);
        instance.SetSkillLevel(skill.currentLevel);
        StartCoolingDown(skillIdx, skill.coolDownTime);
    }

    private void StartCoolingDown(int skillIdx, float coolDownTime)
    {
        coolDownSkills[skillIdx] = true;
        switch (skillIdx)
        {
            case 0: StartCoroutine(CoolDown1stSkill(coolDownTime));
                break;
            case 1: StartCoroutine(CoolDown2ndSkill(coolDownTime));
                break;
            case 2: StartCoroutine(CoolDown3rdSkill(coolDownTime));
                break;
            case 3: StartCoroutine(CoolDown4thSkill(coolDownTime));
                break;
        }
    }

    private IEnumerator CoolDown1stSkill(float coolDownTime)
    {
        yield return new WaitForSeconds(coolDownTime);
        Debug.Log("Skill 1 is ready");
        coolDownSkills[0] = false;
    }
    private IEnumerator CoolDown2ndSkill(float coolDownTime)
    {
        yield return new WaitForSeconds(coolDownTime);
        Debug.Log("Skill 2 is ready");
        coolDownSkills[1] = false;
    }
    private IEnumerator CoolDown3rdSkill(float coolDownTime)
    {
        yield return new WaitForSeconds(coolDownTime);
        Debug.Log("Skill 3 is ready");
        coolDownSkills[2] = false;
    }
    private IEnumerator CoolDown4thSkill(float coolDownTime)
    {
        yield return new WaitForSeconds(coolDownTime);
        Debug.Log("Skill 4 is ready");
        coolDownSkills[3] = false;
    }
}