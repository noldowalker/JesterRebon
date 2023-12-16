using System;
using Code.Actors.Behaviours.Sensor;
using Code.Boot.Logging;
using UnityEngine;

namespace Code.Actors.Behaviours
{
    public class LookForPlayerBehaviour : AbstractBehaviour
    {
        [SerializeField] private float sensorCastLength;
        [SerializeField] private float sensorCastRadius;
        public override BehaviourType Type => BehaviourType.Search;
        protected bool isActiveSearching = false;


        public override void OnStart()
        {
            isActiveSearching = true;
        }

        public override void Act()
        {
            if (isActiveSearching)
                transform.Rotate(0, 25 * Time.deltaTime, 0);
            RaycastHit hit;
            if(Physics.SphereCast(transform.position,sensorCastRadius,transform.forward*sensorCastLength,out hit))
            {
                if(hit.transform.tag == "Player")
                {
                    Enter(hit.transform);
                }
            } else
            {
                Exit();
            }
        }

        public override void OnEnd()
        {
            isActiveSearching = false;
        }

        private void Enter(Transform obj)
        {
            if (!isActiveSearching)
                return;
            actor.SetPlayerLink(obj);
            OnEnd();
            onBehaviourEnd.Invoke();

        }
        
        void Exit()
        {
            actor.SetPlayerLink(null);
        }

        private void OnValidate()
        {
           /* if (sensor == null)
                DebugExtension.Warning($"{gameObject.name} has no sensor for LookForPlayerBehaviour");*/
        }
       
    }
}