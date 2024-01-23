using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Actors.Behaviours;
using Code.Actors.Behaviours.BehaviourSettings;
using Code.Actors.Extensions;
using Code.Actors.Player;
using Code.Actors.Player.Settings;
using Code.Boot.Logging;
using Code.Boot.Systems;
using Code.Helpers.GlobalExtensions;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Code.Actors
{
    [RequireComponent(typeof(IdleBehaviour))]
    [RequireComponent(typeof(DeadBehaviour))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public abstract class NpcActor : AbstractActor
    {
        private NpcActorsSystem _npcActorsSystem;

        public bool disableAi;
        public bool IsDying => currentBehaviour != null && (currentBehaviour.Type == BehaviourType.Dying);
        public bool IsDead => currentBehaviour != null && (currentBehaviour.Type == BehaviourType.Dead);

        [SerializeField] private float currentHp;
        [SerializeField] private float totalHp;
        [SerializeField] private Transform hitPoint;
        [SerializeField] private float attackCastRadius;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Rigidbody actorsRigidbody;
        [SerializeField] private Collider actorsCollider;


        protected AbstractBehaviour currentBehaviour;
        protected List<AbstractBehaviour> behaviours;
        protected Transform _playerTransform;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public Rigidbody ActorsRigidbody => actorsRigidbody;
        public Collider ActorsCollider => actorsCollider;
        public Vector3 PlayerPosition => _playerTransform.IsNotNull() ? _playerTransform.position : Vector3.negativeInfinity;

        public Animator animator;

        private Rigidbody[] _rigidbodies;
        

        public void Init(NpcActorsSystem systemRef)
        {
            Init(new BlankSettings());
            _npcActorsSystem = systemRef;
        }

        public override void Init(AbstractSettings settings)
        {
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            ToggleRagdoll(false);
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = true;
            behaviours = GetComponents<AbstractBehaviour>().ToList();
            BackToIdle();
        }

        public virtual void SetPlayerLink(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        public virtual void ReactOnHit(float force, float timeOfStun, Vector3 direction)
        {
            if (!IsDying)
            {
                var settings = new HitReactionBehaviourSettings()
                {
                    force = force,
                    timeOfStun = timeOfStun,
                    direction = direction,
                };

                ChangeBehaviourTo(BehaviourType.ReactOnHit, settings);
            }
        }

        public virtual void StartDancing(float duration, float damage, float damagePeriod)
        {
            if (!IsDying && (currentBehaviour == null || currentBehaviour.Type != BehaviourType.Dancing))
            {
                Debug.Log("Boom");
                var settings = new DancingBehaviourSettings()
                {
                    duration = duration,
                    damage = damage,
                    damagePeriod = damagePeriod,
                };

                ChangeBehaviourTo(BehaviourType.Dancing, settings);
            }
        }

        public virtual bool IsEnemy(Transform target)
        {
            //=====
            return target.tag == "Player" ||
                (transform.tag == "Enemy" && target.tag == "Neutral") ||
                (transform.tag == "Neutral" && target.tag == "Enemy");
            //===== todo refactor
        }

        public virtual bool InAttackDistance(Transform target)
        {
            Collider[] hits = Physics.OverlapSphere(hitPoint.position, attackCastRadius);
            foreach (Collider hit in hits)
            {
                if (hit.transform.tag == target.tag)
                    return true;
            }
            return false;
        }


        public virtual void MeleeAttackTarget(Transform target)
        {
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
            ChangeBehaviourTo(BehaviourType.MeleeAttack, new BaseAttackBehaviourSettings()
            {
                hitpoint = this.hitPoint,
                attackCastRadius = this.attackCastRadius
            });
        }

        public virtual void RangedAttackTarget()
        {

        }

        public virtual void TakeDamage(float damage)
        {
            if (!IsDying)
            {
                currentHp -= damage;
                Debug.Log(string.Format("Enemy got {0}, remainingHP: {1}", damage, currentHp));
                if (currentHp <= 0)
                {
                    Debug.Log("Enemy is dead");
                    ChangeBehaviourTo(BehaviourType.Dying);
                }
            }
            
        }

        public HiveInfo GetNpcHiveInfo()
        {
            var info = new HiveInfo();
            info.currentNpcCount = _npcActorsSystem.GetActiveNpcCount();
            info.npcIdx = _npcActorsSystem.GetActiveNpcIdx(this.name);
            return info;
        }

        public void ToggleRagdoll(bool ragdollEnabled)
        {
            animator.enabled = !ragdollEnabled;
            foreach(Rigidbody rigidbody in _rigidbodies)
            {
                rigidbody.isKinematic = !ragdollEnabled;
            }

        }

        protected void BackToIdle()
        {
            var idleSettings = new IdleBehaviourSettings()
            { disableAI = disableAi };
            ChangeBehaviourTo(BehaviourType.Idle, idleSettings);
        }

        protected void ChangeBehaviourTo(BehaviourType type, AbstractBehaviourSettings settings = null)
        {
            if (settings.IsNull())
                settings = new BlankBehaviourSettings();
            
            if (currentBehaviour != null)
            {
                currentBehaviour.onBehaviourEnd -= BackToIdle;
                currentBehaviour.OnEnd();
                
                if (IsDead)
                    return;
                
                if (IsDying)
                    type = BehaviourType.Dead;
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


        private void OnDrawGizmos()
        {
            //Gizmos.DrawSphere(hitPoint.position, attackCastRadius);
        }
        public abstract void MakeDecision();
    }
}