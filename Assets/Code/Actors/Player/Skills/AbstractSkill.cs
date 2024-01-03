using Code.Actors.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public abstract class AbstractSkill : MonoBehaviour
{
    [Inject] protected PlayerControlledActor _actor;
    [SerializeField] protected int currentLevel;

    public void FixedUpdate()
    {
        OnPerform();
    }

    public void Start()
    {
        OnStart();
    }

    public void OnDestroy()
    {
        OnEnd();
    }

    public abstract void OnStart();

    public abstract void OnPerform();

    public abstract void OnEnd();

    public abstract void SetSkillLevel(int newLevel);

}
