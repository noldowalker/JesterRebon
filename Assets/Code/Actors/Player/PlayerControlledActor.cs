using System;
using System.Collections;
using Assets.Code.Boot.GlobalEvents.DataObjects;
using Assets.Code.Boot.GlobalEvents.Enum;
using Code.Actors.Player.Settings;
using Code.Boot.GlobalEvents;
using Code.Boot.Logging;
using UnityEngine;
using UnityEngine.VFX;
using VContainer;

namespace Code.Actors.Player
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Transform))]

    public class PlayerControlledActor : AbstractActor
    {
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform hitPoint;

        //Temporal
        [SerializeField] private VisualEffect impactVfx;
        [SerializeField] private VisualEffect splashVfx;

        public bool controlLocked;
        public bool cameraLocked;
        public PlayerCharacterSettings settings;

        private Animator _animator;

        //Local variables
        private Vector3 _currentMovement;
        private float _turnSmoothVelocity;
        private float _gravityModifier;
        //Player current stats
        private float _currentHp;
        private int _currentDashPoints;
        private int _currentDoubleJumps;
        private int _currentAttackCount;
        //Flags
        private bool _canMove;
        private bool _canDash;
        private bool _isDashing;
        private bool _isPunching;
        private bool _canPunch;
        private bool _isKicking;
        private bool _canKick;
        private bool _isSplashing;
        private bool _canApplyDamage;
        private bool _isRestoringDashPoints;
        private bool _isAttacked;
        private bool _isPerformFinisher;
        private bool _isJumping;

        //Temporal constants
        private Vector3 _tempAttackPush;
        private float _currentComboBreakTimer;


        public override void Init(AbstractSettings settings)
        {
            if (settings is not PlayerCharacterSettings playerSettings)
            {
                DebugExtension.Warning($"{name} can`t be initiated without PlayerCharacterSettings!");
                return;
            }
            this.settings = (PlayerCharacterSettings)settings;

            _animator = GetComponentInChildren<Animator>();

            _gravityModifier = 1;

            InitStats();
            _canMove = true;
            _canDash = true;
            _canPunch = true;
            _canKick = true;
            _canApplyDamage = true;

            _isDashing = false;
            _isPunching = false;
            _isKicking = false;
            _isSplashing = false;
            _isAttacked = false;
            _isRestoringDashPoints = false;
            _isPerformFinisher = false;
            _isJumping = false;

            DebugExtension.InitNotice("Player initiated");
        }

        public void Move(Vector3 velocity, Quaternion cameraRotation)
        {
            //Handling falling
            if (IsFalling())
            {
                _currentMovement.y -= settings.gravityScale * _gravityModifier * Time.deltaTime;
            }
            else if(_currentMovement.y < 0)
            {
                _animator.SetTrigger("LandingTrigger");
                _currentMovement.y = 0;
            }
            //Handling movement
            Vector3 direction = new Vector3(velocity.x, 0, velocity.z).normalized;
            if (direction.magnitude < 0.1 || !_canMove)
            {
                _currentMovement = new Vector3(0, _currentMovement.y, 0);
            }
            else
            {
                float targetAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg) + cameraRotation.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, settings.turnSmoothTime);
                transform.localRotation = Quaternion.Euler(0, angle, 0);
                var newMovement = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                _currentMovement.x = newMovement.x;
                _currentMovement.z = newMovement.z;
            }
            if (!_isDashing)
            {
                characterController.Move(_currentMovement * settings.moveSpeed * Time.deltaTime);
                var flatMovement = new Vector3(_currentMovement.x, 0, _currentMovement.z);
                _animator.SetFloat("Speed", flatMovement.magnitude);
            }
            else
            {
                characterController.Move(new Vector3(_currentMovement.x, 0, _currentMovement.z) * settings.dashForce * Time.deltaTime);
            }
        }

        public void Jump()
        {
            if (!IsFalling() || DoubleJump())
            {
                _isJumping = true;
                _animator.SetTrigger("JumpTrigger");
                _currentMovement.y = settings.jumpHeight;
            }
        }

        public void Splash()
        {
            _gravityModifier = settings.splashGravityModifier;
            _isSplashing = true;
        }

        public bool DoubleJump()
        {
            if (_currentDoubleJumps > 0 && IsFalling())
            {
                //Check enemy under player. If exists then it take damage, and player perform double jump
                Collider[] hits = Physics.OverlapSphere(transform.position, settings.doubleJumpCastRadius);
                foreach (Collider hit in hits)
                {
                    if (hit.CompareTag("Enemy"))
                    {
                        Debug.Log("Bonk");
                        var hitData = new EnemyHitDto()
                        {
                            damage = settings.doubleJumpDamage,
                            direction = Vector3.zero,
                            force = 0,
                            enemy = hit.transform.gameObject,
                            timeOfStun = 5,
                        };
                        GlobalEventsSystem<EnemyHitDto>.FireEvent(GlobalEventType.ENEMY_HIT, hitData);
                        _currentDoubleJumps--;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Dash()
        {
            if (_canDash && _currentDashPoints > 0)
            {
                _canDash = false;
                _isDashing = true;
                _currentDashPoints--;
                StartCoroutine(DashPerform());
            }
        }

        public void Punch()
        {
            if (_canPunch && _canKick)
            {
                _canPunch = false;
                _canMove = false;
                _isPunching = true;
                _animator.SetBool("IsPunching",true);
                _animator.SetTrigger("Punch");
                _animator.SetInteger("CurrentAttackCount", _currentAttackCount);
                if (_isPerformFinisher)
                {
                    _isPerformFinisher = false;
                }
                if (_currentAttackCount > settings.attacksBeforeFinisher)
                {
                    _currentAttackCount = 0;
                    _currentComboBreakTimer = 0;
                    _isPerformFinisher = true;
                }
                StartCoroutine(PunchPerform());
            }
        }

        public void Kick()
        {
            if (_canPunch && _canKick)
            {
                _canKick = false;
                _canMove = false;
                _isKicking = true;
                _animator.SetBool("IsKicking", true);
                StartCoroutine(KickPerform());
            }

        }



        public bool IsFalling()
        {
            //Todo refactor for exclude enemies for ground detection
            //return !characterController.isGrounded;
             
            Collider[] hits = Physics.OverlapSphere(transform.position, settings.doubleJumpCastRadius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("ground"))
                {
                    return false;
                }
            }
            return true;
        }

        public void ProcessTimingActions()
        {
            //Processing Snapping to nearest enemy
            if (_isKicking || _isPunching)
            {
                Collider[] hits = Physics.OverlapSphere(hitPoint.position, settings.magnetCastRadius);
                foreach (Collider hit in hits)
                {
                    if (hit.CompareTag("Enemy"))
                    {
                        transform.LookAt(new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z));
                        break;
                    }
                }
            }

            //Processing Kick action
            if (_isKicking)
            {
                if (_canApplyDamage)
                {
                    Collider[] hits = Physics.OverlapSphere(hitPoint.position, settings.kickCastRadius);
                    foreach (Collider hit in hits)
                    {
                        if (hit.CompareTag("Enemy"))
                        {
                            var hitData = new EnemyHitDto()
                            {
                                damage = settings.kickDamage,
                                direction = transform.forward,
                                force = settings.kickPushForce,
                                enemy = hit.transform.gameObject,
                                timeOfStun = settings.kickStunTime,
                            };
                            impactVfx.transform.position = hit.transform.position;
                            impactVfx.Play();
                            GlobalEventsSystem<EnemyHitDto>.FireEvent(GlobalEventType.ENEMY_HIT, hitData);
                            //_isKicking = false;
                            _canApplyDamage = false;
                            break;
                        }
                    }
                }
                characterController.Move(transform.forward * settings.kickDashForce * Time.deltaTime);
            }

            //Processing Punch action
            if (_isPunching)
            {
                if (_canApplyDamage)
                {
                    Collider[] hits = Physics.OverlapSphere(hitPoint.position, settings.punchCastRadius);
                    foreach (Collider hit in hits)
                    {
                        if (hit.CompareTag("Enemy"))
                        {
                            var hitData = new EnemyHitDto()
                            {
                                damage = !_isPerformFinisher ? settings.punchDamage : settings.finisherDamage,
                                direction = transform.forward,
                                force = !_isPerformFinisher ? settings.punchPushForce : settings.finisherPushForce,
                                enemy = hit.transform.gameObject,
                                timeOfStun = !_isPerformFinisher ? settings.punchStunTime : settings.finisherStunTime,
                            };
                            impactVfx.transform.position = hit.transform.position;
                            impactVfx.Play();
                            GlobalEventsSystem<EnemyHitDto>.FireEvent(GlobalEventType.ENEMY_HIT, hitData);
                            _canApplyDamage = false;
                            break;
                        }
                    }
                }
                characterController.Move(transform.forward * (!_isPerformFinisher ? settings.punchDashForce : settings.finisherDashForce) * Time.deltaTime);
            }
            //Processing fall with Splash action
            if (_isSplashing && !IsFalling())
            {
                splashVfx.Play();
                _isSplashing = false;
                _gravityModifier = 1;
                Collider[] hits = Physics.OverlapSphere(transform.position, settings.splashRadius);
                foreach (Collider hit in hits)
                {
                    if (hit.CompareTag("Enemy"))
                    {
                        var hitData = new EnemyHitDto()
                        {
                            damage = settings.splashDamage,
                            direction = hit.transform.position - transform.position,
                            force = settings.splashPushForce,
                            enemy = hit.transform.gameObject,
                            timeOfStun = settings.kickStunTime,
                        };
                        GlobalEventsSystem<EnemyHitDto>.FireEvent(GlobalEventType.ENEMY_HIT, hitData);
                    }
                }
            }
            //Processing double jump reset counter
            if (_currentDoubleJumps < settings.totalDoubleJumps && !IsFalling())
            {
                _currentDoubleJumps = settings.totalDoubleJumps;
            }
            //Processing restoring dashPoints
            if (!_isRestoringDashPoints && _currentDashPoints < settings.totalDashPoints)
            {
                _isRestoringDashPoints = true;
                StartCoroutine(RestoreDashPoints());
            }
            //Processing actor being pushed by enemy attack
            if (_isAttacked)
            {
                characterController.Move(_tempAttackPush * Time.deltaTime);
            }
            //Processing reseting attack count on exceeding combo break timer
            if (_currentAttackCount > 0)
            {
                _currentComboBreakTimer += Time.deltaTime;
                if (_currentComboBreakTimer >= settings.comboBreakTime)
                {
                    _currentComboBreakTimer = 0;
                    _currentAttackCount = 0;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            if (_isDashing)
                return;
            _currentHp -= damage;
            Debug.Log(string.Format("Player got {0}, remainingHP: {1}", damage, _currentHp));
            if (_currentHp <= 0)
            {
                Time.timeScale = 0;
                Debug.Log("GAME OVER");
            }
        }

        public void ReactOnHit(float pushForce, float pushTime, Vector3 pushDirection, float stunTime)
        {
            if (_isDashing)
                return;
            if (!_isAttacked)
            {
                _isAttacked = true;
                _tempAttackPush = pushForce * pushDirection;
                StartCoroutine(BeingAttacked(pushTime));
                if (stunTime > 0)
                {
                    //handle stun
                    controlLocked = true;
                    StartCoroutine(BeingStunned(stunTime));
                }
            }
        }

        public void ActivateSkill(string key)
        {
            var skillData = new SkillDto()
            {
                button = key,
                playerTransform = transform
            };
            GlobalEventsSystem<SkillDto>.FireEvent(GlobalEventType.ACTIVATE_SKILL, skillData);
        }

        private void InitStats()
        {
            _currentHp = settings.totalHP;
            _currentDashPoints = settings.totalDashPoints;
            _currentDoubleJumps = settings.totalDoubleJumps;
        }


        private IEnumerator DashCoolDown()
        {
            yield return new WaitForSeconds(settings.dashCoolDown);
            _canDash = true;
        }
        private IEnumerator DashPerform()
        {
            yield return new WaitForSeconds(settings.dashTime);
            _isDashing = false;
            StartCoroutine(DashCoolDown());
        }
        private IEnumerator PunchCoolDown()
        {
            yield return new WaitForSeconds(settings.punchCoolDown);
            _canPunch = true;
            _canApplyDamage = true;
        }
        private IEnumerator PunchPerform()
        {
            yield return new WaitForSeconds(settings.punchTime);
            _isPunching = false;
            _canMove = true;
            _animator.SetBool("IsPunching", false);
            _currentAttackCount++;
            _currentComboBreakTimer = 0;
            StartCoroutine(PunchCoolDown());
        }
        private IEnumerator KickCoolDown()
        {
            yield return new WaitForSeconds(settings.kickCoolDown);
            _canKick = true;
            _canApplyDamage = true;
        }
        private IEnumerator KickPerform()
        {
            yield return new WaitForSeconds(settings.kickTime);
            _isKicking = false;
            _canMove = true;
            _animator.SetBool("IsKicking", false);
            StartCoroutine(KickCoolDown());
        }

        private IEnumerator RestoreDashPoints()
        {
            yield return new WaitForSeconds(settings.dashPointsRestoreTime);
            _currentDashPoints++;
            _isRestoringDashPoints = false;
        }

        private IEnumerator BeingStunned(float timeOfStun)
        {
            yield return new WaitForSeconds(timeOfStun);
            controlLocked = false;
        }

        private IEnumerator BeingAttacked(float timeOfAttack)
        {
            yield return new WaitForSeconds(timeOfAttack);
            _isAttacked = false;
            _tempAttackPush = Vector3.zero;
        }


        private void OnValidate()
        {
            capsuleCollider ??= GetComponent<CapsuleCollider>();
            characterController ??= GetComponent<CharacterController>();
        }

        private void OnDrawGizmos()
        {
            //Gizmos.DrawRay(hitPoint.position, (transform.forward + (transform.up/6))*10);
            Gizmos.DrawSphere(transform.position, settings.doubleJumpCastRadius);
        }
    }
}
