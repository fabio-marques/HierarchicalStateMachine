using UnityEngine;

namespace HFSM.Experimental.Mecanim.Conditions
{
    public interface ICondition
    {
        public string TriggerName { get; set; }
        public void Evaluate(ActorBlackboard blackboard, Animator animator);
        public bool EvaluateCondition(ActorBlackboard blackboard, Animator animator);
        public void ResetTrigger(Animator animator);
    }
    
    public abstract class BaseCondition : ICondition
    {
        [SerializeField] private string _triggerName;
        [SerializeField] private bool _isBoolean;

        public string TriggerName
        {
            get => _triggerName;
            set => _triggerName = value;
        }

        public void Evaluate(ActorBlackboard blackboard, Animator animator)
        {
            bool conditionMet = EvaluateCondition(blackboard, animator);
            
            if (_isBoolean) animator.SetBool(TriggerName, conditionMet);
            else if (conditionMet) animator.SetTrigger(TriggerName);
        }

        public void ResetTrigger(Animator animator)
        {
            animator.ResetTrigger(TriggerName);
        }
        
        public abstract bool EvaluateCondition(ActorBlackboard blackboard, Animator animator);
    }
}