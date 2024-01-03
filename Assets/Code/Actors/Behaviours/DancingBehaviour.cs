using Assets.Code.Boot.GlobalEvents.DataObjects;
using System;
using System.Collections;
using Code.Actors.Behaviours.BehaviourSettings;
using Code.Boot.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Actors.Behaviours
{
    public class DancingBehaviour : AbstractBehaviour
    {
        private float _duration;
        private float _damage;
        private float _damagePeriod;
        private float _dancingTimer;
        private float _damageTimer;

        public override BehaviourType Type => BehaviourType.Dancing;
        
        public override void Act()
        {
            actor.transform.RotateAround(actor.transform.position, Vector3.up, 120 * Time.deltaTime);
            if (_damage > 0)
            {
                if (_damageTimer >= _damagePeriod)
                {
                    actor.TakeDamage(_damage);
                    _damageTimer = 0;
                }
                _damageTimer += Time.deltaTime;
            }
            _dancingTimer += Time.deltaTime;
            if (_dancingTimer <= _duration)
                return;

            onBehaviourEnd.Invoke();
        }

        public override void OnStart<T>(T settings)
        {
            var dancingBehaviourSettings = settings as DancingBehaviourSettings;

            if (dancingBehaviourSettings == null)
                return;

            _duration = dancingBehaviourSettings.duration;
            _damage = dancingBehaviourSettings.damage;
            _damagePeriod = dancingBehaviourSettings.damagePeriod;
            _damageTimer = 0;
            _dancingTimer = 0;
        }

        public override void OnEnd()
        {
        }
    }
}