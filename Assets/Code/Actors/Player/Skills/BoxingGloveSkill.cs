using Assets.Code.Boot.GlobalEvents.DataObjects;
using Assets.Code.Boot.GlobalEvents.Enum;
using Code.Boot.GlobalEvents;
using Code.ScriptableObjects.Player;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGloveSkill : AbstractSkill
{
    [SerializeField] private BoxingGloveSkillCollection skillCollection;
    private BoxingGloveSettings _settings;
    private float _gloveTimer;
    private float _timeBeforeDestroy;

    private List<string> _hittedEnemies;


    public override void OnPerform()
    {
        if (_gloveTimer <= _settings.gloveTime)
        {
            transform.position += transform.forward * _settings.gloveSpeed * Time.deltaTime;
            _gloveTimer += Time.deltaTime;
            return;
        }
        _timeBeforeDestroy += Time.deltaTime;
        if (_timeBeforeDestroy <= _settings.timeBeforeDestroy)
            return;
        Destroy(gameObject);
    }
    public override void OnEnd()
    {
        _hittedEnemies.Clear();
    }


    public override void OnStart()
    {
        _timeBeforeDestroy = 0;
        _gloveTimer = 0;
        _hittedEnemies = new List<string>();
    }

    public override void SetSkillLevel(int newLevel)
    {
        currentLevel = newLevel;
        _settings = skillCollection.GetByLevel(newLevel);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy" && !_hittedEnemies.Contains(other.name))
        {
            _hittedEnemies.Add(other.name);
            var hitData = new EnemyHitDto()
            {
                damage = _settings.gloveDamage,
                direction = other.transform.position - transform.position,
                force = _settings.glovePushForce,
                enemy = other.transform.gameObject,
                timeOfStun = _settings.gloveStunTime,
            };
            GlobalEventsSystem<EnemyHitDto>.FireEvent(GlobalEventType.ENEMY_HIT, hitData);
        }
    }
}
