using System;
using HFSM.Experimental.Utils;
using UnityEngine;

namespace HFSM.Experimental.Mecanim.Conditions
{
    //By Distance
    //==================================================================================================================
    [Serializable]
    public class DistanceCondition : BaseCondition
    {
        [Header("Distance to current target is...")] 
        public FloatComparer.Mode mode;
        public float value;
        
        public override bool EvaluateCondition(ActorBlackboard blackboard, Animator animator)
        { 
            if (!blackboard.Target) return false;
            
            var distance = Vector3.Distance(blackboard.Target.position, blackboard.Owner.transform.position);
            return FloatComparer.Compare(distance, value, mode);
        }
    }
    
    //By Blackboard Key
    //==================================================================================================================
    [Serializable]
    public class BlackboardCondition : BaseCondition
    {
        [Header("Is this key set?")]
        public string key;
        
        public override bool EvaluateCondition(ActorBlackboard blackboard, Animator animator)
        {
            return blackboard.GetBool(key);
        }
    }

    //From ScriptableObject
    //==================================================================================================================
    [Serializable]
    public class ScriptableObjectCondition : BaseCondition
    {
        public ConditionObject conditionAsset;
        
        public override bool EvaluateCondition(ActorBlackboard blackboard, Animator animator)
        {
            return conditionAsset.EvaluateCondition(blackboard, animator);
        }
    }
}