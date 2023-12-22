using Assets.Code.Boot.GlobalEvents.DataObjects;
using System;
using System.Collections;
using Code.Boot.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Actors.Behaviours
{
    public class DeadBehaviour : AbstractBehaviour
    {
        public override BehaviourType Type => BehaviourType.Dead;
        
        public override void Act()
        {
            onBehaviourEnd?.Invoke();
        }

        public override void OnStart<T>(T settings)
        {
        }

        public override void OnEnd()
        {
        }
    }
}