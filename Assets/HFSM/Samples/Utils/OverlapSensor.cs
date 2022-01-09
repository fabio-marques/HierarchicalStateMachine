using UnityEngine;

namespace HFSM.Samples.Utils
{
    public class OverlapSensor : Sensor
    {
        private enum Type
        {
            Sphere,
            Box
        }
        
        private enum DetectionMode
        {
            Collider,
            Position
        }

        [SerializeField] private Type type;

        //Conditional: Sphere
        [SerializeField, ConditionalField(nameof(type), false, Type.Sphere)]
        private float radius = 1;

        //Conditional: Box
        [SerializeField, ConditionalField(nameof(type), false, Type.Box)]
        private Vector3 halfSize = Vector3.one;

        [Header("Filters")] [SerializeField] private LayerMask layerMask = ~0;

        [SerializeField] private Transform transformTarget;
        [SerializeField, ConditionalField(nameof(transformTarget))]
        private DetectionMode detectionMode = DetectionMode.Collider;

        //Conditional: Tag
        [SerializeField, ConditionalField(nameof(transformTarget), true)]
        private string checkForTag;

        private const int BufferSize = 4;
        private const string Untagged = "Untagged";

        private readonly Collider[] _detected = new Collider[BufferSize];

        private void FixedUpdate()
        {
            switch (type)
            {
                case Type.Sphere:
                    Detected = CheckSphere();
                    break;

                case Type.Box:
                    Detected = CheckBox();
                    break;
            }
        }

        private bool CheckSphere()
        {
            if (transformTarget && detectionMode == DetectionMode.Position)
            {
                return Vector3.Distance(transformTarget.position, transform.position) <= radius;
            }
            
            var overlapCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                radius,
                _detected,
                layerMask,
                QueryTriggerInteraction.Ignore);
            
            return IsValid(overlapCount);
        }

        private bool CheckBox()
        {
            if (transformTarget && detectionMode == DetectionMode.Position)
            {
                var bounds = new Bounds(transform.position, halfSize * 2);
                return bounds.Contains(transformTarget.position);
            }
            
            var overlapCount = Physics.OverlapBoxNonAlloc(
                transform.position,
                halfSize,
                _detected, Quaternion.identity,
                layerMask,
                QueryTriggerInteraction.Ignore);

            return IsValid(overlapCount);
        }

        protected bool IsValid(int overlapCount)
        {
            if (transformTarget)
            {
                for (int i = 0; i < overlapCount; i++)
                {
                    var col = _detected[i];
                    if (col && col.transform == transformTarget) return true;
                }

                return false;
            }

            if (string.IsNullOrWhiteSpace(checkForTag) || checkForTag == Untagged)
                return overlapCount > 0;

            for (int i = 0; i < overlapCount; i++)
            {
                var col = _detected[i];
                if (col && col.CompareTag(checkForTag)) return true;
            }

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Detected ? Color.green : Color.red;

            switch (type)
            {
                case Type.Sphere:
                    Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 0));
                    Gizmos.DrawWireSphere(Vector3.zero, radius);
                    break;

                case Type.Box:
                    Gizmos.DrawWireCube(transform.position, halfSize * 2);
                    break;
            }
        }
    }
}