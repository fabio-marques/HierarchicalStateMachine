using System.Collections;
using UnityEngine;

namespace HFSM.Samples
{
    public class Party : MonoBehaviour
    {
        [SerializeField] private WorldBlackboard worldBlackboard;
        [SerializeField] private GameObject partyIndicator;
        [SerializeField] private float duration = 3;
        [SerializeField] private float interval = 15;

        public const string PartyKey = "PartyActive";
        
        private void OnEnable()
        {
            StartCoroutine(PartyRoutine());
        }

        private void OnDisable()
        {
            SetParty(false);
        }

        private void SetParty(bool value)
        {
            partyIndicator.SetActive(value);
            worldBlackboard.Blackboard.Set(PartyKey, value);
        }

        private IEnumerator PartyRoutine()
        {
            SetParty(false);

            while (true)
            {
                yield return new WaitForSeconds(interval);
                SetParty(true);
                
                yield return new WaitForSeconds(duration);
                SetParty(false);
            }
        }
    }
}