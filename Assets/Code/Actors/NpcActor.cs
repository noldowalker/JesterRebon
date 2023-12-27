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
using Code.Helpers.GlobalExtensions;
using Code.Vfx;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Actors
{
    [RequireComponent(typeof(IdleBehaviour))]
    [RequireComponent(typeof(DeadBehaviour))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(VfxSystem))]
    public abstract class NpcActor : AbstractActor
    {
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
        [SerializeField] private VfxSystem vfxSystem;
        
        protected AbstractBehaviour currentBehaviour;
        protected List<AbstractBehaviour> behaviours;
        
        protected Transform _playerTransform;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public Rigidbody ActorsRigidbody => actorsRigidbody;

        public Collider ActorsCollider => actorsCollider;
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
            
            vfxSystem.Init();
        }

        public virtual void Act()
        {
            vfxSystem.ObserveActiveEffects();
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
                if (currentHp <= 0)
                {
                    Debug.Log("Enemy is dead");
                    ChangeBehaviourTo(BehaviourType.Dying);
                }
            }
            
        }

        /* public virtual bool IsGrounded()
         {
             return Physics.Raycast(transform.position, -Vector3.up, collider.bounds.extents.y + 0.1f);
         }
 */
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