using Assets.Code.Boot.GlobalEvents.DataObjects;
using Assets.Code.Boot.GlobalEvents.Enum;
using Code.Actors.Behaviours.BehaviourSettings;
using Code.Boot.GlobalEvents;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Actors.Behaviours
{
    public class BaseAttackBehaviour : AbstractBehaviour
    {
        
        [SerializeField] private float attackTime;
        [SerializeField] private float attackStunTime;
        [SerializeField] private float damage;
        [SerializeField] private float attackDashForce;
        [SerializeField] private float attackPushForce;
        [SerializeField] private float attackPushTime;
        [SerializeField] private float attackCoolDown;

        private Transform _hitPoint;
        private float _attackCastRadius;
        private float _currentAttackTime;
        private float _currentCoolDownTime;
        private bool _canApplyDamage;

        public override BehaviourType Type => BehaviourType.MeleeAttack;
        
        public override void Act()
        {
            _currentAttackTime += Time.deltaTime;
            if (_currentAttackTime <= attackTime)
            {
                HandleAttack();
                return;
            }
            _currentCoolDownTime += Time.deltaTime;
            if (_currentCoolDownTime <= attackCoolDown)
                return;
            OnEnd();
            onBehaviourEnd.Invoke();
        }

        public override void OnStart<T>(T settings)
        {
            var newSettings = settings as BaseAttackBehaviourSettings;
            _hitPoint = newSettings.hitpoint;
            _attackCastRadius = newSettings.attackCastRadius;
            _currentCoolDownTime = 0;
            _currentAttackTime = 0;
            _canApplyDamage = true;
            actor.ActorsRigidbody.isKinematic = false;
        }

        public override void OnEnd()
        {
            actor.ActorsRigidbody.isKinematic = true;
        }

        private void HandleAttack()
        {
            if (_canApplyDamage)
            {
                Collider[] hits = Physics.OverlapSphere(_hitPoint.position, _attackCastRadius);
                foreach (Collider hit in hits)
                {
                    if (hit.transform.tag == "Player") //actor.IsEnemy(hit.transform)
                    {
                        var hitData = new HitDto()
                        {
                            damage = damage,
                            direction = transform.forward,
                            force = attackPushForce,
                            pushTime = attackPushTime,
                            timeOfStun = attackStunTime,
                        };
                        GlobalEventsSystem<HitDto>.FireEvent(GlobalEventType.PLAYER_HIT, hitData);
                        _canApplyDamage = false;
                        break;
                    }
                }
            }
            actor.ActorsRigidbody.AddForce(transform.forward*attackPushForce,ForceMode.Impulse);
        }


    }
}