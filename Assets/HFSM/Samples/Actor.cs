using HFSM.Samples.Utils;
using UnityEngine;

namespace HFSM.Samples
{
    public abstract class Actor : MonoBehaviour
    {
        protected StateMachine RootStateMachine { get; set; }
        protected ActorBlackboard Blackboard { get; set; }

#if UNITY_EDITOR
        [InspectorReadOnly] [SerializeField] private string statePath;
    
        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying || RootStateMachine?.CurrentState == null) return;

            statePath = RootStateMachine.InspectCurrentStatePath();

            RootStateMachine.DrawTransitionGizmos();
        }
#endif
    }
}