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
        private float force;
        private Vector3 direction;
        private float timeOfStun;
        private float timeInStunnedCondition;

        public override BehaviourType Type => BehaviourType.ReactOnHit;
        
        public override void Act()
        {
            timeInStunnedCondition += Time.deltaTime;
            if (timeInStunnedCondition <= timeOfStun)
                return;

            onBehaviourEnd.Invoke();
        }

        public override void OnStart<T>(T settings)
        {
            var hitReactionSettings = settings as HitReactionBehaviourSettings;

            if (hitReactionSettings == null)
                return;
            
            timeOfStun = hitReactionSettings.timeOfStun;
            direction = hitReactionSettings.direction;
            force = hitReactionSettings.force;
            timeInStunnedCondition = 0;
            
            actor.ActorsRigidbody.isKinematic = false;
            actor.ActorsRigidbody.AddForce(direction * force * 10, ForceMode.Impulse);
        }

        public override void OnEnd()
        {
            actor.ActorsRigidbody.isKinematic = true;
        }
    }
}