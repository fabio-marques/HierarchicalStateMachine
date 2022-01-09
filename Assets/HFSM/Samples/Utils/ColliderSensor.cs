using System.Collections.Generic;
using UnityEngine;

namespace HFSM.Samples.Utils
{
    public abstract class ColliderSensor : Sensor
    {
        protected readonly HashSet<GameObject> detectedObjects = new HashSet<GameObject>();
    
        protected Bounds colliderBounds;
    
        protected bool UpdateDetected()
        {
            return Detected = detectedObjects.Count > 0;
        }
    
        protected virtual void Awake()
        {
            if (TryGetComponent<Collider>(out var col))
            {
                colliderBounds = col.bounds;
            }
            else if (TryGetComponent<Collider2D>(out var col2D))
            {
                colliderBounds = col2D.bounds;
            }
        }
    
        private void OnTriggerEnter(Collider other) => TriggerEnter(other.gameObject);
        private void OnTriggerEnter2D(Collider2D other) => TriggerEnter(other.gameObject);
        private void OnTriggerExit(Collider other) => TriggerExit(other.gameObject);
        private void OnTriggerExit2D(Collider2D other) => TriggerExit(other.gameObject);

        protected virtual void TriggerEnter(GameObject obj)
        {
            if (!enabled || !IsValid(obj) || !detectedObjects.Add(obj)) return;
            UpdateDetected();
        }

        protected virtual void TriggerExit(GameObject obj)
        {
            if (!enabled || !detectedObjects.Remove(obj)) return;
            UpdateDetected();
        }

        private void OnDisable()
        {
            detectedObjects.Clear();
            UpdateDetected();
        }
    
        protected abstract bool IsValid(GameObject collidingObj);
    }
}