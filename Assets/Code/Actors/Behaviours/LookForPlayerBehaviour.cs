using System;
using Code.Actors.Behaviours.BehaviourSettings;
using Code.Actors.Behaviours.Sensor;
using Code.Actors.Player.Settings;
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

        private bool _targetFound;

        public override void OnStart<T>(T settings)
        {
            isActiveSearching = true;
            _targetFound = false;
        }

        public override void Act()
        {
            if (isActiveSearching)
                transform.Rotate(0, 45 * Time.deltaTime, 0);

            //Draw a cone of sight
            ActivateSight(-60);
            ActivateSight(-30);
            ActivateSight(0);
            ActivateSight(30);
            ActivateSight(60);
            if (!_targetFound)
            {
                Exit();
            }
        }


        public override void OnEnd()
        {
            isActiveSearching = false;
        }

        private void ActivateSight(float angle)
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, sensorCastRadius, Quaternion.AngleAxis(angle, Vector3.up) * transform.forward * sensorCastLength, out hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    _targetFound = true;
                    Enter(hit.transform);
                }
            }

        }

        private void Enter(Transform obj)
        {
            if (!isActiveSearching)
                return;
            actor.SetPlayerLink(obj);
            onBehaviourEnd.Invoke();
        }
        
        void Exit()
        {
            actor.SetPlayerLink(null);
        }

        private void OnDestroy()
        {
            
        }

        private void OnDrawGizmos()
        {
            /*Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(-60, Vector3.up) * transform.forward * sensorCastLength);
            Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(-30, Vector3.up) * transform.forward * sensorCastLength);
            Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(0, Vector3.up) * transform.forward * sensorCastLength);
            Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(30, Vector3.up) * transform.forward * sensorCastLength);
            Gizmos.DrawLine(transform.position, Quaternion.AngleAxis(60, Vector3.up) * transform.forward * sensorCastLength);
            Gizmos.DrawSphere(Quaternion.AngleAxis(-60, Vector3.up) * transform.forward * sensorCastLength, sensorCastRadius);
            Gizmos.DrawSphere(Quaternion.AngleAxis(-30, Vector3.up) * transform.forward * sensorCastLength, sensorCastRadius);
            Gizmos.DrawSphere(transform.forward * sensorCastLength, sensorCastRadius);
            Gizmos.DrawSphere(Quaternion.AngleAxis(30, Vector3.up) * transform.forward * sensorCastLength, sensorCastRadius);
            Gizmos.DrawSphere(Quaternion.AngleAxis(60, Vector3.up) * transform.forward * sensorCastLength, sensorCastRadius);*/
        }
    }
}