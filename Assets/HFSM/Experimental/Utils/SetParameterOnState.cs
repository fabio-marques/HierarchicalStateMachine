using UnityEngine;

namespace HFSM.Experimental.Utils
{
    public class SetParameterOnState : StateMachineBehaviour
    {
        public enum State { OnExit, OnEnter }
        public State state;
        public string parameterName;
        public bool isTrigger;
        public bool value;
    
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state == State.OnEnter)
            {
                if (!isTrigger) animator.SetBool(parameterName, value);
                else animator.SetTrigger(parameterName);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state == State.OnExit)
            {
                if (!isTrigger) animator.SetBool(parameterName, value);
                else animator.SetTrigger(parameterName);
            }
        }
    }
}
