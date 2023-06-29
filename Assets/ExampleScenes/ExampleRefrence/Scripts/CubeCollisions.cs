using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Sequence;
using AT.Sequence.Runtime;

public class CubeCollisions : MonoBehaviour
{
    private void Start ()
    {
        AT.Sequence.Component.Activate (null, "Logger", typeof (AT.Sequence.Runtime.Action));
    }

    private void OnCollisionEnter (Collision collision)
    {
        AT.Sequence.Component.Activate (null, "UseGravity", typeof (Action));
    }
}
