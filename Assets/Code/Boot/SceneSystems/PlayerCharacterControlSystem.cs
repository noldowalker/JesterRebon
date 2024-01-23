using Code.Actors.Player;
using Code.Boot.Logging;
using Code.Boot.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Code.Boot.SceneSystems
{
    public class PlayerCharacterControlSystem : BaseSystem
    {
        [Inject] private JesterInputActionsAsset _inputActions;

        [SerializeField] private Transform playerContainer;
        [SerializeField] private SpringArm springArmPrefab;

        private PlayerControlledActor _actor;
        private SpringArm _springArm;

        //CameraRotation
        private float _yaw;
        private float _pitch;
        //Movement
        private Vector3 _currentMovement;
        private float _moveX;
        private float _moveY;

        //ButtonFlags
        private bool _movementPressed;
        private bool _dashPressed;

        public void Init()
        {
            DebugExtension.InitNotice("Player Character Control System - OK");

            _inputActions.actions.Enable();
            _inputActions.actions.jump.performed += OnJump;
            _inputActions.actions.jump.canceled += OnJump;
            _inputActions.actions.forward.performed += OnForward;
            _inputActions.actions.forward.canceled += OnForward;
            _inputActions.actions.right.performed += OnRight;
            _inputActions.actions.right.canceled += OnRight;
            _inputActions.actions.rotateX.performed += OnRotateX;
            _inputActions.actions.rotateX.canceled += OnRotateX;
            _inputActions.actions.rotateY.performed += OnRotateY;
            _inputActions.actions.rotateY.canceled += OnRotateY;
            _inputActions.actions.dash.started += OnDash;
            _inputActions.actions.dash.canceled += OnDash;
            _inputActions.actions.punch.performed += OnPunch;
            _inputActions.actions.punch.canceled += OnPunch;
            _inputActions.actions.kick.performed += OnKick;
            _inputActions.actions.kick.canceled += OnKick;
            _inputActions.actions._1stSkill.started += OnUse1Skill;
            _inputActions.actions._2ndSkill.started += OnUse2Skill;

            _inputActions.actions.pause.started += OnPause;

            _currentMovement = Vector3.zero;
            Cursor.visible = false;
        }

        public void SetControlledActor(PlayerControlledActor currentActor)
        {
            _actor = currentActor;
            _actor.transform.position = playerContainer.transform.position;
            _actor.transform.SetParent(playerContainer, true);

            _springArm = Instantiate(springArmPrefab);
            _springArm.SetTarget(_actor.transform);

        }

        public void Act()
        {
            Move();
            Dash();
            _springArm.Rotate(_yaw, _pitch);
            _actor.ProcessTimingActions();
        }

        public void HandlePlayerHit(float damage, float pushForce, float pushTime, Vector3 pushDirection, float stunTime)
        {
            _actor.TakeDamage(damage);
            _actor.ReactOnHit(pushForce, pushTime, pushDirection, stunTime);
        }

        private void Move()
        {
            _currentMovement.x = _moveX;
            _currentMovement.z = _moveY;
            _actor.Move(_currentMovement, _springArm.transform.localRotation);
        }

        private void Dash()
        {
            if (_dashPressed && _movementPressed)
            {
                _actor.Dash();
            }
        }
        

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (_actor.controlLocked)
                return;
            if (ctx.ReadValueAsButton())
            {
                _actor.Jump();
            }
        }

        private void OnRight(InputAction.CallbackContext ctx)
        {
            if (_actor.controlLocked)
                return;
            _moveX = ctx.ReadValue<float>();
            _movementPressed = _moveX != 0 || _moveY != 0;
        }
        
        private void OnForward(InputAction.CallbackContext ctx)
        {
            if (_actor.controlLocked)
                return;
            _moveY = ctx.ReadValue<float>();
            _movementPressed = _moveX != 0 || _moveY != 0;
        }
        private void OnRotateX(InputAction.CallbackContext ctx)
        {
            if (_actor.cameraLocked)
                return;
            _yaw = ctx.ReadValue<float>();
        }
        private void OnRotateY(InputAction.CallbackContext ctx)
        {
            if (_actor.cameraLocked)
                return;
            _pitch = ctx.ReadValue<float>();
        }
        private void OnDash(InputAction.CallbackContext ctx)
        {
            if (_actor.controlLocked)
                return;
            _dashPressed = ctx.ReadValueAsButton();
        }
        private void OnPunch(InputAction.CallbackContext ctx)
        {
            if (_actor.controlLocked)
                return;
            if (ctx.ReadValueAsButton())
            {
                if (!_actor.IsFalling())
                {
                    _actor.Punch();
                } else
                {
                    _actor.Splash();
                }
            }
        }
        private void OnKick(InputAction.CallbackContext ctx)
        {
            if (_actor.controlLocked)
                return;
            if (ctx.ReadValueAsButton())
            {
                if (!_actor.IsFalling())
                {
                    _actor.Kick();
                }
                else
                {
                    _actor.Splash();
                }
            }
        }

        private void OnUse1Skill(InputAction.CallbackContext ctx)
        {
            if (_actor.controlLocked)
                return;
            _actor.ActivateSkill("1");
        }

        private void OnUse2Skill(InputAction.CallbackContext ctx)
        {
            if (_actor.controlLocked)
                return;
            _actor.ActivateSkill("2");
        }

        private void OnPause(InputAction.CallbackContext ctx)
        {
            Debug.Log("Pause");
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                Cursor.visible = false;
            }
            else
            {
                Time.timeScale = 0;
                Cursor.visible = true;
            }
        }
    }
}
