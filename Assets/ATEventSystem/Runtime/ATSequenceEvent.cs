using System;
using UnityEngine;
using System.Collections.Generic;
using AT.UnitySubSystem.Runtime;

namespace AT.Sequence.Runtime
{ 
    [Serializable]
    public abstract class ATSequenceEvent : ScriptableObject, IUpdatable, IStartable , IDisposable
    { 
        [SerializeField]
        public string eventName = string.Empty;

        [SerializeField]
        public float startTime = 0f;

        [SerializeField]
        public float endTime = 100f;

        [SerializeField]
        private bool isStarting;

        [SerializeField]
        private bool isCompleted;

        [SerializeField]
        private List<ATSequenceEvent> sequenceEvents = new List<ATSequenceEvent> ();

        [SerializeField]
        [Trigger]
        public Trigger defaultTrigger = new Trigger ();

        public virtual string EventName {
            get {
                string nameCounter = string.IsNullOrEmpty (eventName) ? GetDefaultName : eventName;
                return nameCounter;
            }
            set {
                eventName = value;
            }
        }

        public string GetDefaultName {
            get {
                return this.GetType ().FullName;
            }
        }

        public float StartTime {
            get {
                return startTime;
            }
            set {
                startTime = value;
            }
        }

        public float EndTime {
            get {
                return endTime;
            }
            set {
                endTime = value;
            }
        }

        public bool IsStarting {
            get {
                return isStarting;
            }
        }

        public bool IsCompleted {
            get {
                return isCompleted;
            }
        }

        public List<ATSequenceEvent> SequenceEvents {
            get {
                return sequenceEvents;
            }
            set {
                sequenceEvents = value;
            }
        }

        public Trigger Triggerr {
            get {
                return defaultTrigger;
            }
        }

        public bool SequenceEventsCompleted {
            get {
                for(int i = 0 ; i < sequenceEvents.Count ; ++i ) {
                    if ( !sequenceEvents [i].isCompleted ) {
                        return false;
                    }
                }

                return true;
            }
        }

        public void SetStart(bool isStarted)
        {
            this.isStarting = isStarted;
        }

        public void SetCompeleted(bool isCompleted)
        {
            this.isCompleted = isCompleted;
        }

        public void Start ()
        {
            Init ();
        }

        public void Update ()
        {
            StartEvent ();
            EndEvent (); 
        }

        public void Dispose ()
        {
            isStarting = false;
            isCompleted = false;
            ResetEvent ();
        }

        protected virtual void StartEvent ()
        {
            if ( SequenceEventsCompleted && !isCompleted ) {
                
                if ( UpdateTimer (ref startTime, isStarting, SetStart) ) {

                    BindEvent ();
                    defaultTrigger?.Invoke ();
                    return;
                }
            }
        }

        protected virtual void Init()
        {

        }

        protected virtual void UpdateEvent()
        {

        }

        protected virtual void BindEvent()
        {

        }

        protected virtual void EndEvent()
        {
            if ( !isCompleted && isStarting ) {
                if ( UpdateTimer (ref endTime, isCompleted, SetCompeleted) ) {
                    OnEnd ();
                    return;
                }
            }
        }

        protected virtual void OnEnd()
        {

        }

        protected virtual void ResetEvent ()
        {

        }

        private bool UpdateTimer(ref float timer, bool state, System.Action<bool> action)
        {
            float t = timer;
            if(!state && (t >= 0f)) {
                
                if( !(t >= 100f) )
                    timer -= UnityEngine.Time.deltaTime;
                else if(isStarting)
                    action (!state);

                if (timer <= 0 ) {
                    action (!state);
                    return true;
                }
            }

            else if(state == true && !IsCompleted ) {
                UpdateEvent ();
            }


            return false;
        }
    }
}