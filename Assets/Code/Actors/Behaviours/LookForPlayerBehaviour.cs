using System;
using Code.Actors.Behaviours.Sensor;
using Code.Boot.Logging;
using UnityEngine;

namespace Code.Actors.Behaviours
{
    public class LookForPlayerBehaviour : AbstractBehaviour
    {
        [SerializeField] private PlayerVisibleSensor sensor; 
        public override BehaviourType Type => BehaviourType.Search;
        protected bool isActiveSearching = false;

        private void Start()
        {
            sensor.SubOnEnter(Enter);
            sensor.SubOnExit(Exit);
        }

        public override void Act()
        {
            if (isActiveSearching)
                transform.Rotate(0, 25 * Time.deltaTime, 0);
        }

        public override void OnStart()
        {
            isActiveSearching = true;
        }

        public override void OnEnd()
        {
            sensor.SubOnEnter(Enter);
            sensor.SubOnExit(Exit);
            isActiveSearching = false;
        }

        private void Enter(Collider other)
        {
            if (!isActiveSearching)
                return;
            
            if (other.gameObject.CompareTag("Player"))
            {
                actor.SetPlayerLink(other.gameObject.transform);
                OnEnd();
                onBehaviourEnd.Invoke();
            }
        }
        
        void Exit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                actor.SetPlayerLink(null);
            }
        }

        private void OnValidate()
        {
            if (sensor == null)
                DebugExtension.Warning($"{gameObject.name} has no sensor for LookForPlayerBehaviour");
        }
        
        private void OnDestroy()
        {
            sensor.UnsubOnEnter(Enter);
            sensor.UnsubOnExit(Exit);
        }
    }
}