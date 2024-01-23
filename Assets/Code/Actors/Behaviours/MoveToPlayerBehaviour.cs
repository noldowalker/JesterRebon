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
        [SerializeField] private float radiusAroundTarget;
        [SerializeField] private float recalculatingPathTimer;

        private float _timeBeforeRecalculatePath;
        public override BehaviourType Type => BehaviourType.Move;
        
        public override void Act()
        {
            if (actor == null || actor.NavMeshAgent == null)
                return;
            actor.animator.SetFloat("Speed", actor.NavMeshAgent.velocity.magnitude / speed);

            if(_timeBeforeRecalculatePath >= recalculatingPathTimer)
            {
                _timeBeforeRecalculatePath = 0;
                TryToSurround();
            } else
            {
                _timeBeforeRecalculatePath += Time.deltaTime;
            }
            if (!actor.PlayerPosition.Equals(Vector3.negativeInfinity) && 
                actor.NavMeshAgent.remainingDistance > actor.NavMeshAgent.stoppingDistance)
                return;

            onBehaviourEnd.Invoke();
        }

        public override void OnStart<T>(T settings)
        {
            if (actor == null || actor.NavMeshAgent == null)
                return;
            var destination = actor.PlayerPosition;
            if (destination.Equals(Vector3.negativeInfinity))
                return;

            actor.NavMeshAgent.speed = speed;
            //actor.NavMeshAgent.destination = destination;
            _timeBeforeRecalculatePath = 0;
            TryToSurround();
        }

        public override void OnEnd()
        {
            actor.animator.SetFloat("Speed", 0);
            actor.transform.rotation.SetLookRotation(actor.PlayerPosition);
            actor.NavMeshAgent.ResetPath();
        }

        private void TryToSurround()
        {
            HiveInfo info = actor.GetNpcHiveInfo();
            if (info.npcIdx == -1)
                return;
            if (info.currentNpcCount <= 2)
            {
                actor.NavMeshAgent.destination = actor.PlayerPosition;
            }
            else
            {
                Vector3 newPosition;
                newPosition.x = actor.PlayerPosition.x + radiusAroundTarget * MathF.Cos(2 * MathF.PI * info.npcIdx / info.currentNpcCount) + UnityEngine.Random.Range(-0.5f, 0.5f);
                newPosition.y = actor.PlayerPosition.y;
                newPosition.z = actor.PlayerPosition.z + radiusAroundTarget * MathF.Sin(2 * MathF.PI * info.npcIdx / info.currentNpcCount) + UnityEngine.Random.Range(-0.5f, 0.5f);

                actor.NavMeshAgent.destination = newPosition;
            }
        }
    }
}