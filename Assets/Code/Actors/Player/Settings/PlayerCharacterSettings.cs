using System;
using UnityEngine;
namespace Code.Actors.Player.Settings
{
    [Serializable]
    public class PlayerCharacterSettings: AbstractSettings
    {
        [Space]
        [Header("Player stats \n-----------------------")]
        [Space]
        public float totalHP;
        public int totalDashPoints;
        public int totalDoubleJumps;

        [Space]
        [Header("Movement Settings \n-----------------------")]
        [Space]
        public float moveSpeed;
        public float jumpHeight;
        public float turnSmoothTime;
        public float gravityScale;
        public float splashGravityModifier;
        public float dashForce;

        [Space]
        [Header("Action Settings \n-----------------------")]
        [Space]
        public float punchDashForce;
        public float punchPushForce;
        public float kickDashForce;
        public float kickPushForce;
        public float kickCastRadius;
        public float punchCastRadius;
        public float magnetCastRadius;
        public float splashRadius;
        public float splashPushForce;
        public float doubleJumpCastRadius;

        [Space]
        [Header("Damage Settings \n-----------------------")]
        [Space]
        public float punchDamage;
        public float kickDamage;
        public float splashDamage;
        public float doubleJumpDamage;

        [Space]
        [Header("Timer Settings \n-----------------------")]
        [Space]
        public float dashTime;
        public float dashCoolDown;
        public float punchTime;
        public float punchCoolDown;
        public float punchStunTime;
        public float kickTime;
        public float kickCoolDown;
        public float kickStunTime;
        public float splashStunTime;
        public float dashPointsRestoreTime;
    }
}