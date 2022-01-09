namespace HFSM
{
    public interface IState
    {
        public string Id { get; }
        public StateMachine Parent { get; set; }
        void Tick(ActorBlackboard blackboard);
        void Enter(ActorBlackboard blackboard);
        void Exit(ActorBlackboard blackboard);
    }
}








