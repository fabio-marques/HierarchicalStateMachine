using UnityEngine;

namespace HFSM
{
    public class ActorBlackboard : Blackboard
    {
        public MonoBehaviour Owner { get; }
        public Rigidbody RigidBody { get; }
        public Animator Animator { get; }
        
        public Transform Target { get; set; }

        public ActorBlackboard(MonoBehaviour owner, Rigidbody rigidbody, Animator animator)
        {
            Owner = owner;
            RigidBody = rigidbody;
            Animator = animator;
        }
    }
}