using UnityEngine;

namespace HFSM.Samples
{
    public class WorldBlackboard : MonoBehaviour
    {
        public Blackboard Blackboard { get; } = new Blackboard();
    }
}