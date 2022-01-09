using UnityEngine;

namespace HFSM.Samples
{
    public class AlertActor : Actor
    {
        [SerializeField] private WorldBlackboard worldBlackboard;
        [SerializeField] private Transform[] targets;
        [SerializeField] private Rigidbody body;
        [SerializeField] private Animator animator;

        [Header("Animation States")]
        [SerializeField] private float movementSpeed = 10;

        [Header("Animation States")]
        [SerializeField] private string idleAnim = "Idle";
        [SerializeField] private string movementAnim = "Movement";
        [SerializeField] private string celebrationAnim = "Celebration";
        [SerializeField] private string frustrationAnim = "Frustration";
        [SerializeField] private string workAnim = "Work";

        private const float TargetDistanceThreshold = 0.1f;

        private int _currentTargetIndex = 0;
        private Transform CurrentTarget => targets[_currentTargetIndex];

        private void Awake()
        {
            Blackboard = new ActorBlackboard(this, body, animator)
            {
                Target = CurrentTarget
            };
            
            CreateRootFsm();
        }

        private void CreateRootFsm()
        {
            //
            var workSubMachine = CreateWorkFsm();
            var partyingState = new AnimateState("Partying", celebrationAnim);
            var headacheState = new AnimateState("Headache", frustrationAnim, 2);
            
            //
            RootStateMachine = new StateMachine("Root", workSubMachine)
                .AddTransition(workSubMachine, partyingState, IsPartyActive)
                .AddTransition(partyingState, headacheState, IsPartyOver);

            //
            bool IsPartyActive() => worldBlackboard.Blackboard.GetBool(Party.PartyKey);
            bool IsPartyOver() => !worldBlackboard.Blackboard.GetBool(Party.PartyKey);
            
            //
            StateMachine CreateWorkFsm()
            {
                var idleState = new AnimateState("Idle", idleAnim);
                var movingState = new MoveToTargetState("Moving To Station", movementSpeed, movementAnim);
                var workingState = new AnimateState("Working", workAnim, 3);

                return new StateMachine("Work", idleState)
                    .AddTransitionFromAny(idleState, IsTargetNull)
                    .AddTransition(idleState, movingState, HasTarget)
                    .AddTransition(movingState, workingState, ReachedTarget);

                bool HasTarget() => CurrentTarget;
                bool IsTargetNull() => !CurrentTarget;
                bool ReachedTarget() => Blackboard.GetBool(MoveToTargetState.ArrivedAtTargetKey);
            }
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
            var arrived = CurrentTarget && Vector3.Distance(CurrentTarget.position, transform.position) < TargetDistanceThreshold;
            Blackboard.Set(MoveToTargetState.ArrivedAtTargetKey, arrived);
            if (arrived) ChangeCurrentTarget();
        }
        
        private void ChangeCurrentTarget()
        {
            _currentTargetIndex++;
            if (_currentTargetIndex >= targets.Length) _currentTargetIndex = 0;
            Blackboard.Target = CurrentTarget;
        }

        private void OnValidate()
        {
            if (!body) body = GetComponent<Rigidbody>();
        }
    }
}