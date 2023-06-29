using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT.Sequence.Runtime
{
    public abstract class Variable : ATSequenceEvent
    {
        public virtual object Value {
            get {
                return new object ();
            }

            set {
                Value = value;
            }
        }
    }
}