using HFSM.Experimental.Utils;
using UnityEngine;

namespace HFSM.Experimental.Mecanim.Conditions
{
    public class SetCondition : MecanimFSMBehaviour
    {
        [SelectType] [SerializeReference] private ICondition[] triggerCondition;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Owner.OverrideStateTransitions(triggerCondition);
            // for (int i = 0; i < triggerCondition.Length; i++)
            // {
            //     triggerCondition[i]?.Perform(Owner.Blackboard, animator);
            // }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Owner.OverrideStateTransitions(null);
            // for (int i = 0; i < triggerCondition.Length; i++)
            // {
            //     triggerCondition[i]?.ResetTrigger(animator);
            // }
        }

        // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     for (int i = 0; i < triggerCondition.Length; i++)
        //     {
        //         triggerCondition[i]?.Perform(Owner.Blackboard, animator);
        //     }
        // }
    }
}