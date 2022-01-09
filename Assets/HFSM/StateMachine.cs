using System;
using System.Collections.Generic;

namespace HFSM
{
    public class StateMachine : IState
    {
        public IState CurrentState { get; private set; }
        public IState TopmostState { get; private set; }

        private readonly Dictionary<string, List<Transition>> _transitions = new Dictionary<string, List<Transition>>();
        private readonly List<Transition> _anyTransitions = new List<Transition>();
        private List<Transition> _currentTransitions = new List<Transition>();

        private static readonly List<Transition> EmptyTransitions = new List<Transition>(0);

        private readonly IState _entryState;

        public string Id { get; }
        public StateMachine Parent { get; set; }

        private bool _popRequested;

        public StateMachine(string name, IState entryState)
        {
            Id = name;
            _entryState = entryState;
        }

        public string InspectCurrentStatePath()
        {
            var path = new System.Text.StringBuilder($"({Id})");
            var stateMachine = this;
            while (stateMachine != null)
            {
                var subState = stateMachine.CurrentState;
                path.Append($" > ({subState.Id})");
                stateMachine = subState as StateMachine;
            }

            return path.ToString();
        }

        public void Enter(ActorBlackboard blackboard)
        {
            SetState(_entryState, blackboard);
        }

        public void Exit(ActorBlackboard blackboard)
        {
            SetState(null, blackboard);
        }

        public void Tick(ActorBlackboard blackboard)
        {
            if (_popRequested)
            {
                _popRequested = false;
                SetState(_entryState, blackboard);
            }
            else if (ShouldTransition(out var transition))
            {
                SetState(transition.To, blackboard);
            }

            CurrentState?.Tick(blackboard);
        }

        public void PopCurrent()
        {
            _popRequested = true;
        } 

        private void SetState(IState state, ActorBlackboard blackboard)
        {
            if (state == CurrentState) return;

            if (CurrentState != null)
            {
                CurrentState.Exit(blackboard);
                CurrentState.Parent = null;
            }
            
            CurrentState = state;
            
            if (state == null) return;
            CurrentState.Parent = this;
            UpdateTopmostState();

            if (!_transitions.TryGetValue(CurrentState.Id, out _currentTransitions))
            {
                _currentTransitions = EmptyTransitions;
            }

            CurrentState.Enter(blackboard);
        }

        private void UpdateTopmostState()
        {
            if (CurrentState is StateMachine) return;
            
            var stateMachine = this;
            while (stateMachine != null)
            {
                stateMachine.TopmostState = CurrentState;
                stateMachine = stateMachine.Parent;
            }
        }

        public StateMachine AddTransition(IState from, IState to, Func<bool> predicate, Action debugDrawGizmos = null)
        {
            if (!_transitions.TryGetValue(from.Id, out var fromTransitions))
            {
                fromTransitions = new List<Transition>();
                _transitions.Add(from.Id, fromTransitions);
            }

            fromTransitions.Add(new Transition(to, predicate, debugDrawGizmos));
            return this;
        }

        /// <summary>
        /// Add a transition to State from any other state.
        /// </summary>
        public StateMachine AddTransitionFromAny(IState state, Func<bool> predicate, Action debugDrawGizmos = null)
        {
            _anyTransitions.Add(new Transition(state, predicate, debugDrawGizmos));
            return this;
        }
        
        /// <summary>
        /// Add a transition from State to this State Machine's entry state. Essentially restarts the Machine.
        /// </summary>
        public StateMachine AddTransitionToExit(IState state, Func<bool> predicate, Action debugDrawGizmos = null)
        {
            return AddTransition(state, _entryState, predicate, debugDrawGizmos);
        }

        private class Transition
        {
            public Func<bool> Condition { get; }
            public IState To { get; }
#if UNITY_EDITOR
            public Action DebugDrawGizmos { get; }
#endif

            public Transition(IState to, Func<bool> condition, Action debugDrawGizmos = null)
            {
                To = to;
                Condition = condition;
#if UNITY_EDITOR
                DebugDrawGizmos = debugDrawGizmos;
#endif
            }
        }

        private bool ShouldTransition(out Transition transitionInfo)
        {
            foreach (var transition in _anyTransitions)
            {
                if (!transition.Condition()) continue;
                transitionInfo = transition;
                return true;
            }

            foreach (var transition in _currentTransitions)
            {
                if (!transition.Condition()) continue;
                transitionInfo = transition;
                return true;
            }

            transitionInfo = null;
            return false;
        }

        public void DrawTransitionGizmos()
        {
#if UNITY_EDITOR
            foreach (var transition in _anyTransitions)
            {
                transition.DebugDrawGizmos?.Invoke();
            }

            foreach (var transition in _currentTransitions)
            {
                transition.DebugDrawGizmos?.Invoke();
            }

            if (CurrentState is StateMachine subMachine)
            {
                subMachine.DrawTransitionGizmos();
            }
#endif
        }
    }
}