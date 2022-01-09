using HFSM.Experimental.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace HFSM.Experimental.Mecanim.Conditions
{
    [CreateAssetMenu(fileName = "Condition", menuName = "FSM/Condition", order = 0)]
    public class ConditionObject : ScriptableObject, ICondition
    {
        [SelectType] [SerializeReference] private ICondition _condition;

        public string TriggerName
        {
            get => _condition.TriggerName;
            set => _condition.TriggerName = value;
        }

        public void Evaluate(ActorBlackboard blackboard, Animator animator)
        {
            Validate();
            _condition?.Evaluate(blackboard, animator);
        }
        
        public bool EvaluateCondition(ActorBlackboard blackboard, Animator animator)
        {
            Validate();
            return _condition?.EvaluateCondition(blackboard, animator) == true;
        }

        public void ResetTrigger(Animator animator) => _condition?.ResetTrigger(animator);
        
        private void Validate()
        {
            Assert.IsFalse(
                _condition is ScriptableObjectCondition soCondition && soCondition.conditionAsset == this,
                $"TriggerConditionObject can't have itself as condition parameter ({name})."
            );
        }
    }
}