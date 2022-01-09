using System;
using System.Collections.Generic;
using System.Linq;
using HFSM.Experimental.Mecanim.Conditions;
using HFSM.Experimental.Utils;
using HFSM.Samples;
using HFSM.Samples.Utils;
using UnityEngine;

namespace HFSM.Experimental.Mecanim
{
    public class MecanimFSM : MonoBehaviour
    {
        public ActorBlackboard Blackboard { get; private set; }
        
        [SerializeField] private Animator behaviourController;
        [SerializeField] private string hasTargetBoolParam = "_hasTarget";
        [Tooltip("Ignore parameters with this prefix when syncing. Useful for internal parameters that are modified directly.")]
        [SerializeField] private string ignoreParamsWithPrefix = "_";
        [SerializeField] private Animator animationController;
        [SerializeField] private Rigidbody body;
        [SerializeField] private Transform target;
        [SerializeField] private WorldBlackboard worldBlackboard;
        [Space]
        [SerializeField] private List<ParamConditions> conditions;
        
        private readonly Dictionary<string, ICondition> _overrideConditions = new Dictionary<string, ICondition>();


        [System.Serializable]
        public class ParamConditions
        {
            public ParamConditions(string paramName) => this.paramName = paramName;

            [InspectorReadOnly] public string paramName;
            [SelectType] [SerializeReference] public ICondition condition;
        }

        private void Awake()
        {
            Blackboard = new ActorBlackboard(this, body, behaviourController)
            {
                Target = target
            };

            SetTarget(target);

            foreach (var state in behaviourController.GetBehaviours<MecanimFSMBehaviour>())
            {
                state.Owner = this;
            }
        }

        private void OnEnable()
        {
            if (worldBlackboard) worldBlackboard.Blackboard.OnUpdate += OnWorldBlackboardUpdate;
        }
        private void OnDisable()
        {
            if (worldBlackboard) worldBlackboard.Blackboard.OnUpdate -= OnWorldBlackboardUpdate;
        }

        private void OnWorldBlackboardUpdate()
        {
            //Placeholder
            Blackboard.Clear();
            Blackboard.CopyKeyValuesFrom(worldBlackboard.Blackboard);
        }

        private void Update()
        {
            for (int i = 0, len = conditions.Count; i < len; i++)
            {
                _overrideConditions.TryGetValue(conditions[i].paramName, out var condition);
                condition ??= conditions[i].condition;
                condition?.Evaluate(Blackboard, behaviourController);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            Blackboard.Target = target;
            behaviourController.SetBool(hasTargetBoolParam, target != null);
        }

        public void OnReachedTarget()
        {
            //
        }

        public void PlayAnimation(string animStateName)
        {
            if (string.IsNullOrWhiteSpace(animStateName))
            {
                ResetAnimations();
            }
            else
            {
                animationController.Play(animStateName);
            }
        }

        public void ResetAnimations()
        {
            animationController.Rebind();
            animationController.Update(0f);
        }
        
        public void OverrideStateTransitions(ICondition[] triggerConditions)
        {
            _overrideConditions.Clear();
            
            if (triggerConditions == null) return;
            
            for (int i = 0; i < triggerConditions.Length; i++)
            {
                var condition = triggerConditions[i];
                _overrideConditions.Add(condition.TriggerName, condition);
            }
        }

        #region Inspector Buttons

        private const string MissingPrefix = "(Missing) ";
        
        [ContextMenu("Sync Animator Parameters")]
        private void SyncAnimatorParameters()
        {
            //Unity Bug: animator.parameters return 0 parameters if AnimatorController was just modified
            //Sometimes, the warning "Animator is not playing an AnimatorController" appears, which the internet said happens when the animator is not on an active object in the scene
            //(i.e. either disabled or in a prefab, none of which was true in my case)
            //However, disabling and enabling it again seems to fix the issue
            behaviourController.enabled = false;
            behaviourController.enabled = true;
            
            var animParams = behaviourController.parameters;
            
            foreach (var condition in conditions)
            {
                condition.paramName = condition.paramName.Replace(MissingPrefix, "");
                bool existsInAnimator = animParams.Any(parameter => parameter.name == condition.paramName);
                if (!existsInAnimator)
                {
                    condition.paramName = MissingPrefix + condition.paramName;
                }
            }
            
            foreach (var parameter in animParams)
            {
                if(parameter.name[0] == ignoreParamsWithPrefix.ToCharArray(0, 1)[0]) continue;
                
                bool existsInList = conditions.Any(condition => condition.paramName == parameter.name);
                if (!existsInList)
                {
                    conditions.Add(new ParamConditions(parameter.name));
                }
            }
        }

        [ContextMenu("Clear Missing Parameters")]
        private void ClearMissingParameters()
        {
            UnityEditor.Undo.RecordObject(this, "Clear missing parameters");
            var clearedList = conditions.Where(condition => !condition.paramName.Contains(MissingPrefix)).ToList();
            conditions = clearedList;
            UnityEditor.Undo.FlushUndoRecordObjects();
        }

        #endregion
        
        private void OnValidate()
        {
            if (Application.isPlaying && Blackboard != null) SetTarget(target);
            
            if (conditions == null) return;
            
            foreach (var condition in conditions)
            {
                if (condition.condition != null)
                {
                    condition.condition.TriggerName = condition.paramName;
                }
            }
        }
    }
}
