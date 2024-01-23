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
    public class HitReactionBehaviour : AbstractBehaviour
    {
        private float _force;
        private Vector3 _direction;
        private float _timeOfStun;
        private float _timeInStunnedCondition;

        public override BehaviourType Type => BehaviourType.ReactOnHit;
        
        public override void Act()
        {
            _timeInStunnedCondition += Time.deltaTime;
            if (_timeInStunnedCondition <= _timeOfStun)
                return;

            onBehaviourEnd.Invoke();
        }

        public override void OnStart<T>(T settings)
        {
            var hitReactionSettings = settings as HitReactionBehaviourSettings;

            actor.animator.SetTrigger("Hitted");

            if (hitReactionSettings == null)
                return;
            
            _timeOfStun = hitReactionSettings.timeOfStun;
            _direction = hitReactionSettings.direction;
            _force = hitReactionSettings.force;
            _timeInStunnedCondition = 0;

            if (_force > 0)
            {
                actor.ActorsRigidbody.isKinematic = false;
                actor.ActorsRigidbody.AddForce(_direction * _force * 10, ForceMode.Impulse);
            }
        }

        public override void OnEnd()
        {
            actor.ActorsRigidbody.isKinematic = true;
        }
    }
}