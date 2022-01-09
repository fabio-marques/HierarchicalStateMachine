using System;
using UnityEngine;

namespace HFSM.Samples
{
    public class MoveToTargetState : IState
    {
        public string Id { get; }
        public StateMachine Parent { get; set; }

        private readonly float _speed;
        private readonly string _animState;

        public const string ArrivedAtTargetKey = "ArrivedAtTarget";

        public MoveToTargetState(string id, float speed, string animState = null)
        {
            Id = id;
            _speed = speed;
            _animState = animState;
        }
        
        public void Tick(ActorBlackboard blackboard)
        {
            var currentPos = blackboard.Owner.transform.position;
            var dir = (blackboard.Target.position - currentPos).normalized;
            
            blackboard.Owner.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            blackboard.RigidBody.MovePosition(currentPos + dir * (_speed * Time.deltaTime));
        }

        public void Enter(ActorBlackboard blackboard)
        {
            blackboard.Animator.Play(_animState);
        }

        public void Exit(ActorBlackboard blackboard)
        {
            blackboard.RigidBody.velocity = Vector3.zero;
            
            blackboard.Animator.Rebind();
            blackboard.Animator.Update(0f);
        }
    }
}