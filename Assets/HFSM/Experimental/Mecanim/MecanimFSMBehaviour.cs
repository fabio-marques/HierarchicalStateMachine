using UnityEngine;

namespace HFSM.Experimental.Mecanim
{
    public abstract class MecanimFSMBehaviour : StateMachineBehaviour
    {
        public MecanimFSM Owner { get; set; }
    }
}
