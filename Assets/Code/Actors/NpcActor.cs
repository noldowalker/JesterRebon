using System;
using System.Collections.Generic;
using System.Linq;
using Code.Actors.Behaviours;
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
    public abstract class NpcActor : AbstractActor
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        protected AbstractBehaviour currentBehaviour; 
        protected List<AbstractBehaviour> behaviours;
        protected Transform _playerTransform;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
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
            ChangeBehaviourTo(BehaviourType.Idle);
        }

        public virtual void SetPlayerLink(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        protected void BackToIdle()
        {
            ChangeBehaviourTo(BehaviourType.Idle);
        }
        
        protected void ChangeBehaviourTo(BehaviourType type)
        {
            if (currentBehaviour != null)
                currentBehaviour.onBehaviourEnd -= BackToIdle;
            
            currentBehaviour = behaviours.GetHighestPriorityBehaviour(type);
            if (currentBehaviour.IsNull())
                return;
            
            currentBehaviour.onBehaviourEnd += BackToIdle;
            currentBehaviour.OnStart();
        }

        private void OnDestroy()
        {
            currentBehaviour.onBehaviourEnd -= BackToIdle;
        }

        private void OnValidate()
        {
            navMeshAgent ??= GetComponent<NavMeshAgent>();
        }

        public abstract void MakeDecision();
    }
}