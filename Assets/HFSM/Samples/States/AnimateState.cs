using HFSM.Samples;
using UnityEngine;

namespace HFSM.Samples
{
    public class AnimateState : IState
    {
        public string Id { get; }
        public StateMachine Parent { get; set; }

        private readonly string _animState;
        
        private readonly bool _useExitTime;
        private readonly float _exitTime;
        
        private float _time;
        
        public AnimateState(string id, string animState)
        {
            Id = id;
            _animState = animState;
        }

        public AnimateState(string id, string animState, float exitTime)
        {
            Id = id;
            _animState = animState;
            _useExitTime = true;
            _exitTime = exitTime >= 0 ? exitTime : 0;
        }

        public void Tick(ActorBlackboard blackboard)
        {
            if (!_useExitTime) return;
            
            _time += Time.deltaTime;
            if (_time >= _exitTime)
            {
                Parent.PopCurrent();
            }
        }

        public void Enter(ActorBlackboard blackboard)
        {
            _time = 0;
            blackboard.Animator.Play(_animState);
        }

        public void Exit(ActorBlackboard blackboard)
        {
            blackboard.Animator.Rebind();
            blackboard.Animator.Update(0f);
        }
    }
}