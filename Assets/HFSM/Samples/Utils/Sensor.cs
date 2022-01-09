using System;
using UnityEngine;

namespace HFSM.Samples.Utils
{
    public abstract class Sensor : MonoBehaviour
    {
        private bool _detected;
        public bool Detected
        {
            get => _detected;
            protected set
            {
                if (_detected == value) return;
                _detected = value;
                OnDetected?.Invoke(Detected);
            }
        }
    
        public event Action<bool> OnDetected;
    }
}