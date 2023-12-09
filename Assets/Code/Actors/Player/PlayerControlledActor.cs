using System;
using Code.Actors.Player.Settings;
using Code.Boot.Logging;
using UnityEngine;

namespace Code.Actors.Player
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerControlledActor : AbstractActor
    {
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private CharacterController characterController;
        public float JumpHeight { get; private set; }
        public float MoveSpeed { get; private set; }
        public override void Init(AbstractSettings settings)
        {
            if (settings is not PlayerCharacterSettings playerSettings)
            {
                DebugExtension.Warning($"{name} can`t be initiated without PlayerCharacterSettings!");
                return;
            }
            
            JumpHeight = playerSettings.jumpHeight;
            MoveSpeed = 5;
            DebugExtension.InitNotice("Player initiated");
        }

        public void Move(Vector3 direction)
        {
            var movement = new Vector3(direction.x * MoveSpeed, direction.z * JumpHeight, direction.y * MoveSpeed);
            characterController.Move(movement * Time.deltaTime);
        }

        private void OnValidate()
        {
            capsuleCollider ??= GetComponent<CapsuleCollider>();
            characterController ??= GetComponent<CharacterController>();
        }
    }
}
