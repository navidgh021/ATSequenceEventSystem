using UnityEngine;

namespace AT.Sequence.Runtime
{
    public abstract class Event : ATSequenceEvent
    {
        [Property]
        public GameObject gameObject;

        [Property]
        public Object target;

        protected override void StartEvent ()
        {
            if(CheckActivate(gameObject, target)) {
                base.StartEvent ();
            }
        }

        protected abstract bool CheckActivate (GameObject gameObject, Object target);
    }
}