using Assets.Code.Boot.GlobalEvents.DataObjects;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Actors.Behaviours
{
    public class DeathBehaviour : AbstractBehaviour
    {
        public float timeBeforeDestroy;

        private float _remainingTime;

        public override BehaviourType Type => BehaviourType.Death;
        
        public override void Act()
        {
            _remainingTime += Time.deltaTime;
            if (_remainingTime <= timeBeforeDestroy)
                return;
            OnEnd();
            onBehaviourEnd.Invoke();
        }

        public override void OnStart<T>(T settings)
        {
            //Enable ragdoll
            //===========
            actor.NavMeshAgent.enabled = false;
            actor.transform.Rotate(new Vector3(1, 0, 0), 90f);
            //========== todo remove
            actor.ActorsRigidbody.isKinematic = true;
            actor.ActorsCollider.enabled = false;
        }

        public override void OnEnd()
        {
            //TODO REFACTOR
            Destroy(gameObject);
        }

    }
}