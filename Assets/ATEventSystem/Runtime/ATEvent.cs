using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.UnitySubSystem.Runtime;

namespace AT.Sequence.Runtime
{
    public class ATEvent : MonoBehaviour
    {
        public List<ATSequenceEvent> events = new List<ATSequenceEvent> ();

        private void OnEnable () => Component.AddSequenceEvent (this);

        private void OnDisable () => Component.RemoveSequenceEvent (this);

        public void InvokeEvent( ATSequenceEvent targetEvents)
        {
            ATSequenceEvent target = targetEvents;

            if ( !events.Contains (target) ) 
                return;

            ATSubSystems.Register (target);
        }
    }
}