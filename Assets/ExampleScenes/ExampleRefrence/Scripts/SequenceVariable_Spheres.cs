using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AT.Sequence;
using AT.Sequence.Runtime;

[ATSequence(path: "Variable/Spheres")]
public class SequenceVariable_Spheres : Variable
{
    [AT.Sequence.Property]
    public List<GameObject> items = new List<GameObject> ();

    public override object Value {
        get {
            return items.ToArray ();
        }

    }
}
