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
        [Inject] private InputActionAsset _inputActions;

        [SerializeField] private Transform playerContainer;
        
        private PlayerControlledActor _actor;
        private Vector3 velocity;
        
        public void Init()
        {
            DebugExtension.InitNotice("Player Character Control System - OK");
            
            _inputActions.FindActionMap("actions").Enable();
            _inputActions.FindActionMap("actions").FindAction("jump").performed += OnJump;
            _inputActions.FindActionMap("actions").FindAction("forward").performed += OnForward;
            _inputActions.FindActionMap("actions").FindAction("forward").canceled += OnForward;
            _inputActions.FindActionMap("actions").FindAction("right").performed += OnRight;
            _inputActions.FindActionMap("actions").FindAction("right").canceled += OnRight;
            
            velocity = Vector3.zero;
        }

        public void SetControlledActor(PlayerControlledActor currentActor)
        {
            _actor = currentActor;
            _actor.transform.position = playerContainer.transform.position;
            _actor.transform.SetParent(playerContainer, true);
        }

        public void Act()
        {
            if (!velocity.Equals(Vector3.zero))
                _actor.Move(velocity);
        }
        
        private void OnJump(InputAction.CallbackContext ctx)
        {
            // _actor.Jump();
        }

        private void OnRight(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            velocity.x = value;
        }
        
        private void OnForward(InputAction.CallbackContext ctx)
        {
            var value = ctx.ReadValue<float>();
            velocity.y = value;
        }
    }
}
