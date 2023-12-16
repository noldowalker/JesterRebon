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
            _currentMovement = Vector3.zero;
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
            _actor.ProcessTimingActions();
            _springArm.Rotate(_yaw, _pitch);
        }

        public void HandlePlayerHit(float damage, float force, float stunTime, Vector3 direction)
        {
            _actor.TakeDamage(damage);
            _actor.ReactOnHit(force, stunTime, direction);
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
            if (ctx.ReadValueAsButton())
            {
                _actor.Jump();
            }
        }

        private void OnRight(InputAction.CallbackContext ctx)
        {
            _moveX = ctx.ReadValue<float>();
            _movementPressed = _moveX != 0 || _moveY != 0;
        }
        
        private void OnForward(InputAction.CallbackContext ctx)
        {
            _moveY = ctx.ReadValue<float>();
            _movementPressed = _moveX != 0 || _moveY != 0;
        }
        private void OnRotateX(InputAction.CallbackContext ctx)
        {
            _yaw = ctx.ReadValue<float>();
        }
        private void OnRotateY(InputAction.CallbackContext ctx)
        {
            _pitch = ctx.ReadValue<float>();
        }
        private void OnDash(InputAction.CallbackContext ctx)
        {
            _dashPressed = ctx.ReadValueAsButton();
        }
        private void OnPunch(InputAction.CallbackContext ctx)
        {
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
    }
}
