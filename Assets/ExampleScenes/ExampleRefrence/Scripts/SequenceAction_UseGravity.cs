using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Sequence;
using AT.Sequence.Runtime;

[ATSequence(path: "Action/UseGravity")]
public class SequenceAction_UseGravity : Action
{
    [Variable]
    public Variable items = null;

    [Action]
    public void EnableGravity()
    {
        if ( items != null ) { 
            if ( items.Value.GetType ().IsArray ) {
                IEnumerable enumerable = (IEnumerable) items.Value;
                foreach ( GameObject g in enumerable ) {
                    Rigidbody rigidbody = g.GetComponent<Rigidbody> ();

                    if ( rigidbody != null ) {
                        rigidbody.useGravity = true;
                    }
                }
            }
        }
    }

    [Action]
    public void DisableGravity()
    {
        if ( items != null ) {
            if ( items.Value.GetType ().IsArray ) {
                IEnumerable enumerable = (IEnumerable) items.Value;
                foreach ( GameObject g in enumerable ) {
                    Rigidbody rigidbody = g.GetComponent<Rigidbody> ();

                    if ( rigidbody != null ) {
                        rigidbody.useGravity = false;
                    }
                }
            }
        }
    }
}
