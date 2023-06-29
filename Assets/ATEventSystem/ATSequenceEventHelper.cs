using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using AT.Sequence.Runtime;

namespace AT.Sequence
{
    public class ATSequenceEventHelper
    {
        public class EditorEventInfo
        {
            public string listType;

            public int listenerIndex;

            public int sequenceIndex;

            public ATEvent eventListener;

            public ATSequenceEvent atSequence;

            public object infoObject;

            public EditorEventInfo (string listType, int listenerIndex, int sequenceIndex, ATEvent eventListener = null, ATSequenceEvent atSequence = null, object infoObject = null)
            {
                this.listType = listType;
                this.listenerIndex = listenerIndex;
                this.sequenceIndex = sequenceIndex;
                this.eventListener = eventListener;
                this.atSequence = atSequence;
                this.infoObject = infoObject;
            }
        }

        public struct SequenceEventInfo
        {
            public string name;

            public ATSequenceEvent sequenceEvent;

            public SequenceEventInfo (string name, ATSequenceEvent sequenceEvent)
            {
                this.name = name;
                this.sequenceEvent = sequenceEvent;
            }
        }

        public static Dictionary<string, ATSequenceEvent> eventDictionary = new Dictionary<string, ATSequenceEvent> ();
        
        public static Dictionary<string, ATSequenceEvent> FindEventClasses()
        {
            List<ATSequenceEvent> types = GetAllEvent ();
            eventDictionary.Clear ();
            List<ATSequenceEvent> array = types;

            foreach ( ATSequenceEvent type in array ) {

                object[] customAttributes = type.GetType().GetCustomAttributes (typeof (ATSequenceAttribute), true);
                object[] array2 = customAttributes;

                for ( int j = 0 ; j < array2.Length ; ++j ) {
                    
                    ATSequenceAttribute eventAttribute = (ATSequenceAttribute) array2 [j];
                    eventDictionary.Add (eventAttribute.MenuPath, type);
                }
            }
            return eventDictionary;
        }

        public static List<ATSequenceEvent> GetAllEvent()
        {
            Assembly [] assemblys = AppDomain.CurrentDomain.GetAssemblies ();
            Assembly [] array = assemblys;
            List<ATSequenceEvent> list = new List<ATSequenceEvent> ();

            foreach ( Assembly assembly in array ) {
                try {
                    Type [] types = assembly.GetTypes ();
                    Type [] array2 = types;


                    foreach ( Type type in types ) {
                        if ( !CheckSubTypes<ATSequenceEvent> (type) || type.IsAbstract || !type.IsClass )
                            continue;

                        ATSequenceEvent subEvent = ScriptableObject.CreateInstance (type.FullName) as ATSequenceEvent;
                        list.Add (subEvent);
                    }
                }

                catch ( ReflectionTypeLoadException ) {

                }
            }
            return list;
        }

        public static FieldInfo [] GetSequenceFields<T> (Type type)
        {
            BindingFlags flag =  BindingFlags.Public | BindingFlags.Instance;
            MemberInfo [] fieldsObject = type.GetFields (flag);
            List<object> filedObjectList = new List<object> ();

            for ( int i = 0 ; i < fieldsObject.Length ; ++i ) {

                object attri = fieldsObject [i].GetCustomAttribute (typeof (T), true);


                if ( attri != null ) {
                    filedObjectList.Add (fieldsObject [i]);
                }
            }

            FieldInfo [] ddfd = new FieldInfo [filedObjectList.Count];

            for ( int i = 0 ; i < filedObjectList.Count ; ++i )
                ddfd [i] = (FieldInfo) filedObjectList [i];

            return ddfd;
        }

        public static MethodInfo[] GetSequenceActionMethods(Type type)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            MethodInfo[] methods = type.GetMethods (flags);
            List<object> methodObjectList = new List<object> ();

            for(int i = 0 ; i < methods.Length ; ++i ) {
                object attri = methods [i].GetCustomAttribute (typeof (AT.Sequence.ActionAttribute), true);

                if ( attri != null )
                    methodObjectList.Add (methods [i]);
            }

            MethodInfo [] methods2 = new MethodInfo [methodObjectList.Count];

            for(int i = 0 ; i < methodObjectList.Count ; ++i ) {
                methods2 [i] = (MethodInfo)methodObjectList [i];
            }

            return methods2;
        }

        public static bool CheckSubTypes<T> (Type type)
        {
            return type.IsSubclassOf (typeof (T));
        }
    }
}