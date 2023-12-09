using System;
using System.Collections.Generic;
using System.Linq;
using Code.Boot.Logging;
using UnityEngine;

namespace Code.Environment
{
    [RequireComponent(typeof(SphereCollider))]
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private SphereCollider contactCollider;
        private List<Collider> _occupiedBy;
        
        public bool IsAvailableForSpawn()
        {
            return !_occupiedBy.Any();
        }

        private void Start()
        {
            _occupiedBy = new List<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Enemy"))
                return;
            
            _occupiedBy.Add(other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Enemy"))
                return;
            
            _occupiedBy.Remove(other);
        }

        private void OnValidate()
        {
            contactCollider ??= GetComponent<SphereCollider>();
            if (!contactCollider.isTrigger)
                contactCollider.isTrigger = true;
        }
    }
}