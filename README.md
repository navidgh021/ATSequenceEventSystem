# AT Sequence Event System
The advanced Event Framework for unity game engine with different Features.

### Features
* Events with scriptableObject
* with editor window
* The ability to launch different events in a period of time
* with 3 different event type
* Ability to call events without dependency reference

### How To Use
1.First clone or download this repository and add this to your project.2.create new empty gameobject or select an exists gameobject.

4.Add 
`ATEvent`
component to selected gameobject.

<a href="https://imgbb.com/"><img src="https://i.ibb.co/QN49GxD/Untitled.png" alt="Untitled" border="0"></a>

4.Now you have added the event listener to the target object.

5.Now you can define your own events and add them to the event listeners.Create a new C# Script and use this librarys on top on script
`using AT.Sequence;`
`using AT.Sequence.Runtime;`

6.You can determine the own menu for the events by tagging the event classes.Now inherit the default class with one of the existing event classes.

```c#
using UnityEngine;
using AT.Sequence;
using AT.Sequence.Runtime;

[ATSequence (path: "Action/Logger")]
public class SequenceAction_Logger : Action
{
    [AT.Sequence.Property]
    public string LogText = "Logger Is Called!";

    [Action]
    public void Log()
    {
        Debug.Log (LogText);
    }

    [Action]
    public void LogWarning()
    {
        Debug.LogWarning (LogText);
    }

    [Action]
    public void LogError()
    {
        Debug.LogError (LogText);
    }
}

```
```c#
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

```

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT.Sequence;
using AT.Sequence.Runtime;

[ATSequence(path: "Event/TestEvent")]
public class EventTest : AT.Sequence.Runtime.Event
{
    [AT.Sequence.Property]
    public float tested = 1000000f;

    protected override void UpdateEvent ()
    {
        Debug.Log ("UpdateEvent");
    }

    protected override void BindEvent ()
    {
        Debug.Log ("BindEvent");
    }

    protected override void OnEnd ()
    {
        Debug.Log ("EndEvent");
    }

    protected override bool CheckActivate (GameObject gameObject, Object target)
    {
        return true;
    }
}

```

7.Now go to `AT/AT Sequence Event Window` menu bar and open Sequence event editor window.

8.You can create your events from the created menus.

<a href="https://ibb.co/2Z8VMZW"><img src="https://i.ibb.co/GHsYMHn/Untitled2.png" alt="Untitled2" border="0"></a>

9.You can tagging fields and change them in the editor window.for action events, mark method to select target method for call.

<a href="https://ibb.co/6gyQqf8"><img src="https://i.ibb.co/PwrkVfM/Untitled3.png" alt="Untitled3" border="0"></a>

10.To call an event, just remember its name and call it by writing a single line code.

```c#
private void Start ()
    {
        AT.Sequence.Component.Activate (null, "Event Name", typeof (AT.Sequence.Runtime.ATSequenceEvent));
    }
```
