using System;
using UnityEngine;

namespace Code.Actors.Behaviours.Sensor
{
    [RequireComponent(typeof(Mesh))]
    public class PlayerVisibleSensor : AbstractSensor
    {
        [SerializeField] private MeshRenderer visiblePart;
        private Color originalColor;

        private void Start()
        {
            originalColor = visiblePart.material.color;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;
            
            base.OnTriggerEnter(other);
            visiblePart.material.color = Color.red;
        }
        
        protected override void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;
            
            base.OnTriggerExit(other);
            visiblePart.material.color = originalColor;
        }
    }
}