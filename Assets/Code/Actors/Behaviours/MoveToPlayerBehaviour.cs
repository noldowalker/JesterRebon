using System;
using Code.Actors.Behaviours.BehaviourSettings;
using Code.Boot.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Actors.Behaviours
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MoveToPlayerBehaviour : AbstractBehaviour
    {
        [SerializeField] private float speed;
        public override BehaviourType Type => BehaviourType.Move;
        
        public override void Act()
        {
            if (!actor.PlayerPosition.Equals(Vector3.negativeInfinity) && 
                actor.NavMeshAgent.remainingDistance > actor.NavMeshAgent.stoppingDistance)
                return;

            onBehaviourEnd.Invoke();
        }

        public override void OnStart<T>(T settings)
        {
            var destination = actor.PlayerPosition;
            if (destination.Equals(Vector3.negativeInfinity))
                return;

            actor.NavMeshAgent.speed = speed;
            actor.NavMeshAgent.destination = destination;
        }

        public override void OnEnd()
        {
            actor.NavMeshAgent.ResetPath();
        }
    }
}