using System;
using Code.Boot.Logging;
using UnityEngine;

namespace Code.Actors.Behaviours.Sensor
{
    [RequireComponent(typeof(Collider))]
    public abstract class AbstractSensor : MonoBehaviour
    {
        protected Action<Collider> sensorEnter;
        protected Action<Collider> sensorExit;

        public void SubOnEnter(Action<Collider> callback)
        {
            sensorEnter += callback;
        }

        public void UnsubOnEnter(Action<Collider> callback)
        {
            sensorEnter -= callback;
        }

        public void SubOnExit(Action<Collider> callback)
        {
            sensorExit += callback;
        }

        public void UnsubOnExit(Action<Collider> callback)
        {
            sensorExit -= callback;
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            sensorEnter?.Invoke(other);
        }
        
        protected virtual void OnTriggerExit(Collider other)
        {
            sensorExit?.Invoke(other);
        }
    }
}