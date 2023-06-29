using System;
using System.Collections.Generic;
using UnityEngine; 
using AT.Sequence.Runtime;
using AT.UnitySubSystem;
using AT.UnitySubSystem.Runtime;

namespace AT.Sequence
{
    public class Component
    {
        private static Dictionary<GameObject, List<ATSequenceEvent>> eventSequencesDictionary = new Dictionary<GameObject, List<ATSequenceEvent>> ();

        private static Dictionary<string, ATSequenceEvent> sequenceEventNameDictionary = new Dictionary<string, ATSequenceEvent> ();

        static Component ()
        {
            eventSequencesDictionary = new Dictionary<GameObject, List<ATSequenceEvent>> ();
            sequenceEventNameDictionary = new Dictionary<string, ATSequenceEvent> ();
        }

        public static void AddSequenceEvent (ATEvent eventListener)
        {
            ATEvent aTEventListener = eventListener;
            GameObject gameObject = eventListener.gameObject;
            int index = aTEventListener.events.Count;

            if ( index <= 0 )
                return;

            if ( !eventSequencesDictionary.ContainsKey (gameObject) ) {
                eventSequencesDictionary.Add (gameObject, aTEventListener.events);
                for ( int i = 0 ; i < index ; ++i ) {
                    ATSequenceEvent sequenceEvent = aTEventListener.events [i];
                    string sequenceEventName = sequenceEvent.EventName;

                    if ( sequenceEventNameDictionary.ContainsKey (sequenceEventName) ) {
                        Debug.LogError ("There are two sequence event name available. please change this sequence name : " + sequenceEventName);
                        return;
                    }
                    if ( !sequenceEventNameDictionary.ContainsValue (sequenceEvent) ) {
                        
                        sequenceEventNameDictionary.Add (sequenceEventName, sequenceEvent);
                    }
                }
            }
        }

        public static void RemoveSequenceEvent (ATEvent eventListener)
        {
            GameObject gameObject = eventListener.gameObject;
            int index = eventListener.events.Count;

            if ( eventSequencesDictionary.ContainsKey (gameObject) ) {
                eventSequencesDictionary.Remove (gameObject);

                for(int i = 0 ; i < index ; ++i ) {
                    ATSequenceEvent sequenceEvent = eventListener.events [i];
                    string sequenceName = sequenceEvent.EventName;

                    if( sequenceEventNameDictionary.ContainsKey (sequenceName) ) {
                        ATSubSystems.Unregister (sequenceEvent);
                        sequenceEventNameDictionary.Remove (sequenceName);
                    }
                }
            }
        }

        public static ATSequenceEvent Activate(GameObject target, string eventName, Type sequenceType)
        {
            if ( !string.IsNullOrEmpty (eventName) ) {
                if ( target != null ) {
                    if ( eventSequencesDictionary.TryGetValue (target, out List<ATSequenceEvent> sequenceList) ) {
                        ATEvent eventListener = target.GetComponent<ATEvent> ();

                        for ( int i = 0 ; i < sequenceList.Count ; ++i ) {
                            ATSequenceEvent sequenceEvent = sequenceList [i];
                            if ( sequenceEvent.EventName == eventName ) {


                                List<ATSequenceEvent> sequenceEventsSubList = sequenceEvent.SequenceEvents;
                                int sequenceEventsSubListIndex = sequenceEventsSubList.Count;

                                if ( sequenceEventsSubListIndex > 0 ) {
                                    for ( int j = 0 ; j < sequenceEventsSubListIndex ; ++j ) {
                                        ATSequenceEvent subSequenceEvents = sequenceEventsSubList [j];
                                        eventListener.InvokeEvent (subSequenceEvents);
                                    }
                                }
                                eventListener.InvokeEvent (sequenceEvent);
                                return sequenceType.ReflectedType == sequenceEvent.GetType ().ReflectedType ? sequenceEvent : default;
                            }
                        }
                    }
                }

                else {
                    if( sequenceEventNameDictionary.TryGetValue(eventName, out ATSequenceEvent sequenceEvent )){
                        List<ATSequenceEvent> sequenceEventsSubList = sequenceEvent.SequenceEvents;
                        int sequenceEventsSubListIndex = sequenceEventsSubList.Count;

                        if ( sequenceEventsSubListIndex > 0 ) {
                            for ( int j = 0 ; j < sequenceEventsSubListIndex ; ++j ) {
                                ATSequenceEvent subSequenceEvents = sequenceEventsSubList [j];
                                ATSubSystems.Register (subSequenceEvents);
                            }
                        }
                        ATSubSystems.Register (sequenceEvent);
                        return sequenceType.ReflectedType == sequenceEvent.GetType ().ReflectedType ? sequenceEvent : default;
                    }
                }
            }

            return default;
        }
    }
}