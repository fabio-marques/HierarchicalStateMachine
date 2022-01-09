using System;
using UnityEngine;

namespace HFSM.Experimental.Mecanim.States
{
    public class MoveToTargetState : MecanimFSMBehaviour
    {
        [SerializeField] private float speed = 10;

        [Header("Animation")]
        [SerializeField] private string playAnimationState = "Movement";
        
        [Header("( Optional )")]
        [SerializeField] private string setTriggerOnReached;
        
        private const float SqrTargetDistanceThreshold = 0.01f;

        private Animator _animator;
        private bool _reached;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _animator = animator;
            _reached = false;
            Stop();
            Owner.PlayAnimation(playAnimationState);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Owner.Blackboard.Target) return;
            
            var currentPos = Owner.transform.position;
            var offset = Owner.Blackboard.Target.position - currentPos;
            var dir = offset.normalized;
            
            Owner.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            Owner.Blackboard.RigidBody.MovePosition(currentPos + dir * (speed * Time.deltaTime));

            if (!_reached && offset.sqrMagnitude <= SqrTargetDistanceThreshold)
            {
                Reached();
            }
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //ResetTriggers();
            Stop();
            Owner.ResetAnimations();
        }

        private void Stop()
        {
            Owner.Blackboard.RigidBody.velocity = Vector3.zero;
            Owner.Blackboard.RigidBody.angularVelocity = Vector3.zero;
        }

        private void Reached()
        {
            Stop();
            Owner.OnReachedTarget();

            if (!string.IsNullOrWhiteSpace(setTriggerOnReached))
            {
                _animator.SetTrigger(setTriggerOnReached);
            }
        }

        private void ResetTriggers()
        {
            if (!string.IsNullOrWhiteSpace(setTriggerOnReached))
            {
                _animator.ResetTrigger(setTriggerOnReached);
            }
        }

        private void OnDestroy()
        {
            _animator = null;
        }
    }
}
