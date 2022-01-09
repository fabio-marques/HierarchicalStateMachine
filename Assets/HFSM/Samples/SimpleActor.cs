using UnityEngine;

namespace HFSM.Samples
{
    public class SimpleActor : Actor
    {
        [SerializeField] private Transform target;
        [SerializeField] private Rigidbody body;
        [SerializeField] private Animator animator;

        [Header("Animation States")]
        [SerializeField] private float movementSpeed = 10;

        [Header("Animation States")]
        [SerializeField] private string idleAnim = "Idle";
        [SerializeField] private string movementAnim = "Movement";
        [SerializeField] private string celebrationAnim = "Celebration";
        [SerializeField] private string frustrationAnim = "Frustration";

        private const float TargetDistanceThreshold = 0.1f;

        private void Awake()
        {
            Blackboard = new ActorBlackboard(this, body, animator)
            {
                Target = target
            };
            
            CreateRootFsm();
        }

        private void CreateRootFsm()
        {
            //States
            var idleState = new AnimateState("Idle", idleAnim);
            var movingState = new MoveToTargetState("Moving To Target", movementSpeed, movementAnim);
            var celebrationState = new AnimateState("Celebrating", celebrationAnim);
            var frustrationState = new AnimateState("Frustrated", frustrationAnim, 2);
            
            //Root
            RootStateMachine = new StateMachine("Root", idleState);

            //Transitions
            RootStateMachine.AddTransition(idleState, movingState, HasTarget);
            RootStateMachine.AddTransition(movingState, idleState, IsTargetNull);
            RootStateMachine.AddTransition(movingState, celebrationState, IsCloseToTarget, DrawDistanceToTarget);
            RootStateMachine.AddTransition(celebrationState, frustrationState, IsFarFromTarget);

            //Conditions
            bool HasTarget() => Blackboard.Target;
            bool IsTargetNull() => !Blackboard.Target;
            bool IsCloseToTarget() => Blackboard.GetBool(MoveToTargetState.ArrivedAtTargetKey);
            bool IsFarFromTarget() => !Blackboard.GetBool(MoveToTargetState.ArrivedAtTargetKey);
        }

        private void Start()
        {
            RootStateMachine.Enter(Blackboard);
        }

        private void Update()
        {
            UpdateDistanceToTarget();
            RootStateMachine.Tick(Blackboard);
        }

        private void UpdateDistanceToTarget()
        {
            Blackboard.Set(MoveToTargetState.ArrivedAtTargetKey,
                Blackboard.Target && Vector3.Distance(Blackboard.Target.position, transform.position) < TargetDistanceThreshold);
        }

        private void DrawDistanceToTarget()
        {
#if UNITY_EDITOR
            var targetPos = Blackboard.Target.position;
            
            var defaultColor = Gizmos.color;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, targetPos);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(targetPos, targetPos + Vector3.up * 2);
            UnityEditor.Handles.Label(targetPos + Vector3.up * 2, (targetPos - transform.position).magnitude.ToString("F"));
            
            Gizmos.color = defaultColor;
#endif
        }

        private void OnValidate()
        {
            if (!body) body = GetComponent<Rigidbody>();
        }
    }
}