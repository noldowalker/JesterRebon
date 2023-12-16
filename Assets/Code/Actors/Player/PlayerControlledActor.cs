using System;
using System.Collections;
using Assets.Code.Boot.GlobalEvents.DataObjects;
using Assets.Code.Boot.GlobalEvents.Enum;
using Code.Actors.Player.Settings;
using Code.Boot.GlobalEvents;
using Code.Boot.Logging;
using UnityEngine;

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
        
        public PlayerCharacterSettings settings;

        //Local variables
        private Vector3 _currentMovement;
        private float _turnSmoothVelocity;
        private float _gravityModifier;
        //Player current stats
        private float _currentHp;
        private int _currentDashPoints;
        private int _currentDoubleJumps;
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


        public override void Init(AbstractSettings settings)
        {
            if (settings is not PlayerCharacterSettings playerSettings)
            {
                DebugExtension.Warning($"{name} can`t be initiated without PlayerCharacterSettings!");
                return;
            }
            this.settings = (PlayerCharacterSettings)settings;

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
            _isRestoringDashPoints = false;

            DebugExtension.InitNotice("Player initiated");
        }

        public void Move(Vector3 velocity, Quaternion cameraRotation)
        {
            //Handling falling
            if (IsFalling())
            {
                _currentMovement.y -= settings.gravityScale * _gravityModifier * Time.deltaTime;
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
            } else
            {
                characterController.Move(new Vector3 (_currentMovement.x,0,_currentMovement.z) * settings.dashForce * Time.deltaTime);
            }
        }

        public void Jump()
        {
            if (!IsFalling() || DoubleJump())
            {
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
                Collider[] hits = Physics.OverlapSphere(hitPoint.position, settings.doubleJumpCastRadius);
                foreach (Collider hit in hits)
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        Debug.Log("Bonk");
                        var hitData = new EnemyHitDto()
                        {
                            damage = settings.doubleJumpDamage,
                            direction = Vector3.zero,
                            force = 0,
                            enemy = hit.transform.gameObject
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
                StartCoroutine(KickPerform());
            }
            
        }

        

        public bool IsFalling()
        {
            //Todo refactor for exclude enemies for ground detection
            return !characterController.isGrounded;
        }

        public void ProcessTimingActions()
        {
            //Processing Snapping to nearest enemy
            if(_isKicking || _isPunching)
            {
                Collider[] hits = Physics.OverlapSphere(hitPoint.position, settings.magnetCastRadius);
                foreach (Collider hit in hits)
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        transform.LookAt(new Vector3(hit.transform.position.x,transform.position.y,hit.transform.position.z));
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
                        if (hit.transform.tag == "Enemy")
                        {
                            var hitData = new EnemyHitDto()
                            {
                                damage = settings.kickDamage,
                                direction = transform.forward,
                                force = settings.kickPushForce,
                                enemy = hit.transform.gameObject
                            };
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
                        if (hit.transform.tag == "Enemy")
                        {
                            var hitData = new EnemyHitDto()
                            {
                                damage = settings.punchDamage,
                                direction = transform.forward,
                                force = settings.punchPushForce,
                                enemy = hit.transform.gameObject
                            };
                            GlobalEventsSystem<EnemyHitDto>.FireEvent(GlobalEventType.ENEMY_HIT, hitData);
                            //_isPunching = false;
                            _canApplyDamage = false;
                            break;
                        }
                    }
                }
                characterController.Move(transform.forward * settings.punchDashForce * Time.deltaTime);
            }
            //Processing fall with Splash action
            if (_isSplashing && !IsFalling())
            {
                Debug.Log("Splash");
                _isSplashing = false;
                _gravityModifier = 1;
                Collider[] hits = Physics.OverlapSphere(transform.position, settings.splashRadius);
                foreach (Collider hit in hits)
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        var hitData = new EnemyHitDto()
                        {
                            damage = settings.splashDamage,
                            direction = hit.transform.position - transform.position,
                            force = settings.splashPushForce,
                            enemy = hit.transform.gameObject
                        };
                        GlobalEventsSystem<EnemyHitDto>.FireEvent(GlobalEventType.ENEMY_HIT, hitData);                      
                    }
                }
            }
            //Processing double jump reset counter
            if(_currentDoubleJumps < settings.totalDoubleJumps && !IsFalling())
            {
                _currentDoubleJumps = settings.totalDoubleJumps;
            }
            //Pricessing restoring dashPoints
            if(!_isRestoringDashPoints && _currentDashPoints < settings.totalDashPoints)
            {
                _isRestoringDashPoints = true;
                StartCoroutine(RestoreDashPoints());
            }
        }

        public void TakeDamage(float damage)
        {
            _currentHp -= damage;
            if(_currentHp <= 0)
            {
                Debug.Log("GAME OVER");
            }
        }

        public void ReactOnHit(float force, float stunTime, Vector3 direction)
        {

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
            StartCoroutine(KickCoolDown());
        }

        private IEnumerator RestoreDashPoints()
        {
            yield return new WaitForSeconds(settings.dashPointsRestoreTime);
            _currentDashPoints++;
            _isRestoringDashPoints = false;
        }


        private void OnValidate()
        {
            capsuleCollider ??= GetComponent<CapsuleCollider>();
            characterController ??= GetComponent<CharacterController>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(hitPoint.position, (transform.forward + (transform.up/6))*10);
            //Gizmos.DrawSphere(transform.position, settings.doubleJumpCastRadius);
        }
    }
}
