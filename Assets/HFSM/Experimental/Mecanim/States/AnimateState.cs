using UnityEngine;

namespace HFSM.Experimental.Mecanim.States
{
    public class AnimateState : MecanimFSMBehaviour
    {
        [Header("Animation")]
        [SerializeField] private string playAnimationState;

        [Header("( Optional )")]
        [SerializeField] private string setTriggerAfterDuration;
        [SerializeField, Min(0)] private float duration;

        private Animator _animator;
        private bool _durationEnded;
        private float _currentTime;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _animator = animator;
            _durationEnded = false;
            _currentTime = 0;
            
            Owner.PlayAnimation(playAnimationState);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_durationEnded || string.IsNullOrWhiteSpace(setTriggerAfterDuration)) return;

            _currentTime += Time.deltaTime;
            
            if (_currentTime > duration)
            {
                _durationEnded = true;
                _animator.SetTrigger(setTriggerAfterDuration);
            }
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Owner.ResetAnimations();
        }
        
        private void OnDestroy()
        {
            _animator = null;
        }
    }
}