using Assets.Code.Boot.GlobalEvents.DataObjects;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Actors.Behaviours
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class HitReactionBehaviour : AbstractBehaviour
    {
        private float force;
        private Vector3 direction;
        private float timeOfStun;
        private bool isStunned;

        public override BehaviourType Type => BehaviourType.ReactOnHit;
        
        public override void Act()
        {
            /*if(!actor.Rigidbody.isKinematic && actor.Rigidbody.velocity.magnitude <= 0.01)
                actor.Rigidbody.isKinematic = true;*/
            if (isStunned)
                return;
            OnEnd();
            onBehaviourEnd.Invoke();
        }

        public override void OnStart()
        {
            timeOfStun = 5;
            isStunned = true;
            StartCoroutine(StunPerform());
        }

        public override void OnEnd()
        {
            actor.Rigidbody.isKinematic = true;
        }

        private IEnumerator StunPerform()
        {
            yield return new WaitForSeconds(timeOfStun);
            isStunned = false;
        }
    }
}