using System;
using System.Collections.Generic;
using System.Linq;
using Code.Actors.Behaviours;
using Code.Actors.Behaviours.BehaviourSettings;
using Code.Actors.Extensions;
using Code.Actors.Player;
using Code.Actors.Player.Settings;
using Code.Boot.Logging;
using Code.Helpers.GlobalExtensions;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Actors
{
    [RequireComponent(typeof(IdleBehaviour))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public abstract class NpcActor : AbstractActor
    {
        public bool disableAi;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Rigidbody actorsRigidbody;
        [SerializeField] private Collider actorsCollider;
        protected AbstractBehaviour currentBehaviour;
        protected List<AbstractBehaviour> behaviours;
        
        protected Transform _playerTransform;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public Rigidbody ActorsRigidbody => actorsRigidbody;
        public Vector3 PlayerPosition => _playerTransform.IsNotNull() ? _playerTransform.position : Vector3.negativeInfinity;

        public void Init()
        {
            Init(new BlankSettings());
        }

        public override void Init(AbstractSettings settings)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = true;
            behaviours = GetComponents<AbstractBehaviour>().ToList();

            if (!disableAi)
            {
                ChangeBehaviourTo(BehaviourType.Idle);
            }
        }

        public virtual void SetPlayerLink(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        public virtual void ReactOnHit(float force, float timeOfStun, Vector3 direction)
        {
            var settings = new HitReactionBehaviourSettings()
            {
                force = force,
                timeOfStun = timeOfStun,
                direction = direction,
            };
            
            ChangeBehaviourTo(BehaviourType.ReactOnHit, settings);
        }

        public virtual void TakeDamage(float damage)
        {
            Debug.Log($"Enemy got {damage} damage");
            
        }

        public virtual bool IsGrounded()
        {
            return Physics.Raycast(transform.position, -Vector3.up, actorsCollider.bounds.extents.y + 0.1f);
        }

        protected void BackToIdle()
        {
            ChangeBehaviourTo(BehaviourType.Idle);
        }

        protected void ChangeBehaviourTo(BehaviourType type, AbstractBehaviourSettings settings = null)
        {
            if (settings.IsNull())
                settings = new BlankBehaviourSettings();
            
            if (currentBehaviour != null)
            {
                currentBehaviour.OnEnd();
                currentBehaviour.onBehaviourEnd -= BackToIdle;
            }

            currentBehaviour = behaviours.GetHighestPriorityBehaviour(type);
            if (currentBehaviour.IsNull())
                return;

            currentBehaviour.onBehaviourEnd += BackToIdle;
            currentBehaviour.OnStart(settings);
        }


        private void OnDestroy()
        {
            if (currentBehaviour.IsNotNull())
                currentBehaviour.onBehaviourEnd -= BackToIdle;
        }

        private void OnValidate()
        {
            navMeshAgent ??= GetComponent<NavMeshAgent>();
            actorsRigidbody ??= GetComponent<Rigidbody>();
            actorsCollider ??= GetComponent<Collider>();
        }

        public abstract void MakeDecision();
    }
}