using Assets.Code.Boot.GlobalEvents.DataObjects;
using Assets.Code.Boot.GlobalEvents.Enum;
using Code.Boot.GlobalEvents;
using Code.ScriptableObjects.Player;
using System.Collections.Generic;
using UnityEngine;

public class DancingAuraSkill : AbstractSkill
{
    [SerializeField] private DancingAuraSkillCollection skillCollection;
    private DancingAuraSettings _settings;
    private float _auraTimer;
    private float _recaptureTimer;


    public override void OnPerform()
    {
        if(_recaptureTimer > 0.5f)
        {
            _recaptureTimer = 0;
            Collider[] hits = Physics.OverlapSphere(transform.position, _settings.auraRadius);
            foreach (Collider hit in hits)
            {
                if (hit.transform.tag == "Enemy")
                {
                    var danceData = new EnemyDanceDto()
                    {
                        enemy = hit.transform.gameObject,
                        duration = _settings.totalDuration - _auraTimer,
                        damage = _settings.periodicDamage,
                        damagePeriod = _settings.damagePeriod,
                    };
                    GlobalEventsSystem<EnemyDanceDto>.FireEvent(GlobalEventType.ENEMY_DANCING, danceData);
                }
            }
        }
        _recaptureTimer += Time.deltaTime;
        _auraTimer += Time.deltaTime;
        if (_auraTimer <= _settings.totalDuration)
            return;
        Destroy(gameObject);
    }
    public override void OnEnd()
    {
    }


    public override void OnStart()
    {
        _auraTimer = 0;
        _recaptureTimer = 0;
       
    }

    public override void SetSkillLevel(int newLevel)
    {
        currentLevel = newLevel;
        _settings = skillCollection.GetByLevel(newLevel);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, _settings.auraRadius);
    }
}
