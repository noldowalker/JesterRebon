using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoxingGloveSettings : AbstractSkillLevelSettings
{
    public float gloveSpeed;
    public float gloveTime;
    public float gloveDamage;
    public float glovePushForce;
    public float gloveStunTime;
    public float timeBeforeDestroy;
}
