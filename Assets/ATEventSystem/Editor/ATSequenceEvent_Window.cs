using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using AT.Utility;
using AT.Sequence.Runtime;
using AT.Serialization;

namespace AT.Sequence.Editor
{
    public class ATSequenceEvent_Window : EditorWindow
    { 
        private SerializedObject serializedObject;

        private SerializedObject serializedObject2;

        private SerializedProperty [] sequenceEventFieldsSerializedProperty;

        private SerializedProperty [] sequenceEventTriggerSerializedProperty;

        private SerializedProperty [] sequenceEventVariablesSerializedProperty;

        private SerializedProperty actionMethodSerializedProperty;

        private SerializedProperty methodNameSerializedProperty;

        private ATSequenceEventHelper.EditorEventInfo eventInfos;

        private bool isEditButtonPressed;

        private string titleButtonName;

        private int currentId;

        private Vector2 scrollPosition;

        private ATEvent targetEvent;

        private ReorderableList sequenceDelayEventList;

        private MethodInfo [] actionMethodInfos;

        private string [] actionMethodNames;

        private int actionMethodsIndex = 0;

        private FieldInfo [] variableField;

        private Dictionary<string, Variable> variableNamesDictionary = new Dictionary<string, Variable> ();

        private List<string> variableNames = new List<string> ();

        private List<KeyValuePair<string, Variable>> variableKeyNames = new List<KeyValuePair<string, Variable>> ();

        private static Dictionary<object, object> variableKeyHash = new Dictionary<object, object> ();

        private static ATSequenceEvent_Window window;

        private static List<ATEvent> eventListeners = new List<ATEvent> ();

        private static int eventsCount;

        private static List<bool> foldOut = new List<bool> ();

        private static List<ReorderableList> sequencesList = new List<ReorderableList> ();

        private static Dictionary<ReorderableList, ATEvent> listEventListenerDictionary = new Dictionary<ReorderableList, ATEvent> ();

        private const string dataPathLocation = "EditorData/SequenceEvents/";

        [MenuItem ("AT/AT Sequence Event Window", false, -98)]
        public static void OpenWindow ()
        {
            Initialize ();
            
            window = ATSequenceEvent_Window.GetWindow<ATSequenceEvent_Window> ("AT Sequence");
            window.minSize = new Vector2 (600f, 300f);
            window.autoRepaintOnSceneChange = true;
            
            window.Show ();
        }

        private static void Initialize ()
        {
            eventListeners.Clear ();
            foldOut.Clear ();
            sequencesList.Clear ();

            ATEvent [] aTEvents = UnityEngine.Object.FindObjectsOfType<ATEvent> ();
            foreach(ATEvent events in aTEvents ) {
                if ( !eventListeners.Contains (events) )
                    eventListeners.Add (events);
            }

            eventsCount = eventListeners.Count;
            foldOut = new List<bool> (new bool [eventsCount]);
            sequencesList = new List<ReorderableList> (new ReorderableList [eventsCount]);
            listEventListenerDictionary.Clear ();
        }

        private void OnFocus ()
        {
            for ( int i = 0 ; i < eventsCount ; ++i ) {
                sequencesList [i] = new ReorderableList (eventListeners [i].events, typeof (ATSequenceEvent), true, true, true, true);

                if ( !listEventListenerDictionary.ContainsKey (sequencesList [i]) ) {
                    listEventListenerDictionary.Add (sequencesList [i], eventListeners [i]);
                }
            }
        }

        private void OnDisable ()
        {
            Selection.activeGameObject = null;
        }

        private void Update ()
        {
            if ( eventListeners.Count <= 0 ) {
                Initialize ();
            }
        }

        private void OnGUI ()
        {
            GUILayout.Label ("AT Sequence Event System", Style.baseStyle);
            GUILayout.Space (5);
            if ( eventListeners.Count <= 0 ) {
                GUILayout.Label ("There is no sequence event available in the scene!", Style.baseStyle);
                return;
            }
            scrollPosition = GUILayout.BeginScrollView (scrollPosition);

            GUILayout.BeginHorizontal ();


            GUILayout.BeginVertical (EditorStyles.helpBox, GUILayout.Width (Screen.width / 3f), GUILayout.ExpandHeight (true));
            GUILayout.Space (5);
            for ( int i = 0 ; i < eventsCount ; ++i ) {


                GUI.color = new Color32 (211, 211, 211, 255);
                GUILayout.BeginVertical (EditorStyles.helpBox);
                GUI.color = Color.white;
                if ( eventListeners [i] != null ) {
                    foldOut [i] = EditorGUILayout.Foldout (foldOut [i], eventListeners [i].gameObject.name, true);
                    if ( foldOut [i] ) {
                        InitializeDrawerList (i);
                        sequencesList [i].DoLayoutList ();
                    }
                }

                GUILayout.EndVertical ();
                GUILayout.Space (1f);
            }
            GUILayout.EndVertical ();


            GUILayout.BeginVertical (EditorStyles.helpBox, GUILayout.Height (Screen.height));
            DrawSequenceTitle ();
            GUILayout.Space (3f);
            DrawSequenceHeaderInformations ();
            GUILayout.Space (3f);
            DrawSequenceEventFields ();
            GUILayout.EndVertical ();



            GUILayout.EndHorizontal ();
            GUILayout.EndScrollView ();
        }

        private void AddMenuItem (object target)
        {
            if ( target == null)
                return;

            ATSequenceEventHelper.EditorEventInfo eventInfo = (ATSequenceEventHelper.EditorEventInfo) target;
            ATSequenceEvent ev = eventInfo.atSequence;
            ev.EventName = ev.GetDefaultName + eventInfo.eventListener.events.Count;

            if ( listEventListenerDictionary.TryGetValue ((ReorderableList) eventInfo.infoObject, out targetEvent) ) {
                if ( ev == null || targetEvent == null )
                    return;

                targetEvent.events.Add (ev);

            }
        }

        private void InitializeDrawerList (int ListenerIndex)
        {
            Selection.activeGameObject = eventListeners [ListenerIndex].gameObject;
            sequencesList [ListenerIndex].onAddDropdownCallback = (buttonRect, list) => {
                GenericMenu menu = new GenericMenu ();
                Dictionary<string, ATSequenceEvent> pairs = ATSequenceEventHelper.FindEventClasses ();

                for ( int j = 0 ; j < pairs.Count ; ++j ) {
                    
                    KeyValuePair<string, ATSequenceEvent> kpv = pairs.ElementAt (j);
                    menu.AddItem (new GUIContent (kpv.Key), false, AddMenuItem, new ATSequenceEventHelper.EditorEventInfo ("event" + eventListeners[ListenerIndex].gameObject.name.ToString(), ListenerIndex, j, eventListeners[ListenerIndex], kpv.Value, list));
                }

                menu.ShowAsContext ();
            };

            sequencesList [ListenerIndex].onRemoveCallback = (recorderableList) => {
                eventInfos  = new ATSequenceEventHelper.EditorEventInfo ("event" + eventListeners [ListenerIndex].gameObject.name.ToString (), ListenerIndex, 0, eventListeners [ListenerIndex]);

                recorderableList.list.RemoveAt (recorderableList.index);
            };

            sequencesList [ListenerIndex].drawHeaderCallback = (rect) => {
                EditorGUI.LabelField (rect, "Events");
            };

            sequencesList [ListenerIndex].drawElementCallback = (rect, elementIndex, isActive, isFocused) => {
                string element = eventListeners[ListenerIndex].events [elementIndex].EventName;
                Rect rect2 = new Rect (rect.x, rect.y + 5, 267f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField (rect2, element);

                if ( isFocused ) {
                    eventInfos = new ATSequenceEventHelper.EditorEventInfo ("event" + eventListeners [ListenerIndex].gameObject.name.ToString (), ListenerIndex, elementIndex, eventListeners [ListenerIndex]);
                    InitializeSequenceEventPropertys (eventInfos);
                }
            };
        }

        private void DrawSequenceTitle()
        {
            ATSequenceEventHelper.EditorEventInfo eventInfo = eventInfos;

            if ( eventInfo == null || eventInfo.eventListener.events.Count <= 0 )
                return;

            GUILayout.Label ("Sequence Name : ", EditorStyles.whiteLargeLabel);

            GUI.color = new Color32 (211, 211, 211, 255);
            GUILayout.BeginHorizontal (EditorStyles.helpBox);
            GUI.color = Color.white;

            if ( eventInfo.listType == "event" + eventInfo.eventListener.gameObject.name ) {
                if( eventInfo.eventListener.events.Count > 0 ) {

                    if ( !isEditButtonPressed ) {
                        GUILayout.Label ("Name : " + eventInfo.eventListener.events[eventInfo.sequenceIndex].EventName);
                    }
                    
                    else {
                        GUILayout.BeginHorizontal ();
                        GUILayout.Label ("Enter new name : ");
                        GUILayout.Space (7f);

                        eventInfo.eventListener.events [eventInfo.sequenceIndex].EventName = EditorGUILayout.TextField (eventInfo.eventListener.events [eventInfo.sequenceIndex].EventName, EditorStyles.toolbarTextField);
                        GUILayout.EndHorizontal ();
                    }

                    GUILayout.FlexibleSpace ();

                    titleButtonName = isEditButtonPressed ? "Apply" : "Edit";

                    if ( GUILayout.Button (titleButtonName, EditorStyles.toolbarButton) ) 
                        isEditButtonPressed = !isEditButtonPressed;

                    currentId = UnityEditor.EditorGUIUtility.GetControlID (FocusType.Passive) + 100;

                    if(GUILayout.Button("Import", EditorStyles.toolbarButton)) 
                        EditorGUIUtility.ShowObjectPicker<ScriptableObject> (null, false, string.Empty, currentId);

                    if(UnityEngine.Event.current.commandName == "ObjectSelectorClosed" && (EditorGUIUtility.GetObjectPickerControlID() == currentId) ) {
                        currentId = -1;

                        if(EditorGUIUtility.GetObjectPickerObject() != null ) {
                            if ( EditorGUIUtility.GetObjectPickerObject () as ATSequenceEvent )
                                eventInfo.eventListener.events [eventInfo.sequenceIndex] = (ATSequenceEvent) EditorGUIUtility.GetObjectPickerObject ();
                            else
                                Debug.Log ("Selected element is not sequence event");
                        }
                    }

                    if ( GUILayout.Button ("Export", EditorStyles.toolbarButton) )
                        AssetUtils.CreateScriptableObjectAsset (eventInfo.eventListener.events [eventInfo.sequenceIndex].GetDefaultName, eventInfo.eventListener.events [eventInfo.sequenceIndex].EventName);
                }
            }

            GUILayout.EndHorizontal ();
        }

        private void DrawSequenceHeaderInformations()
        {
            ATSequenceEventHelper.EditorEventInfo eventInfo = eventInfos;

            if ( eventInfo == null || eventInfo.eventListener.events.Count <= 0 )
                return;

            GUI.color = new Color32 (211, 211, 211, 255);
            GUILayout.BeginVertical (EditorStyles.helpBox);
            GUI.color = Color.white;
            if ( eventInfo.listType == "event" + eventInfo.eventListener.gameObject.name ) {
                if ( eventInfo.eventListener.events.Count > 0 ) {
                    string startTimeTextContainer = eventInfo.eventListener.events [eventInfo.sequenceIndex].StartTime > 0f ? eventInfo.eventListener.events [eventInfo.sequenceIndex].StartTime.ToString () + " s " : "On Call";
                    string endTimeTextContainer = eventInfo.eventListener.events [eventInfo.sequenceIndex].EndTime < 100f ? eventInfo.eventListener.events [eventInfo.sequenceIndex].EndTime.ToString () + " s " : "Unlimited";


                    GUILayout.Label ("Status : ", EditorStyles.whiteLargeLabel);
                    GUILayout.Label ("Sequence Start : " + eventInfo.eventListener.events [eventInfo.sequenceIndex].IsStarting);
                    GUILayout.Label ("Sequence Completed : " + eventInfo.eventListener.events [eventInfo.sequenceIndex].IsCompleted);
                    GUILayout.Label ("Start Time : " + startTimeTextContainer);
                    GUILayout.Label ("End Time : " + endTimeTextContainer);

                    GUILayout.Space (10f);

                    GUILayout.Label ("Edit Sequence Event Time : ", EditorStyles.whiteLargeLabel);

                    GUILayout.Space (10f);

                    GUILayout.BeginHorizontal ();
                    eventInfo.eventListener.events [eventInfo.sequenceIndex].StartTime = EditorGUILayout.FloatField (eventInfo.eventListener.events [eventInfo.sequenceIndex].StartTime, GUILayout.Width (37f));
                    EditorGUILayout.MinMaxSlider (ref eventInfo.eventListener.events [eventInfo.sequenceIndex].startTime, ref eventInfo.eventListener.events [eventInfo.sequenceIndex].endTime, 0f, 100f);
                    if ( eventInfo.eventListener.events [eventInfo.sequenceIndex].EndTime < 100f )
                        eventInfo.eventListener.events [eventInfo.sequenceIndex].EndTime = EditorGUILayout.FloatField (eventInfo.eventListener.events [eventInfo.sequenceIndex].EndTime, GUILayout.Width (37f));
                    else
                        GUILayout.Label ("∞", EditorStyles.textField, GUILayout.Width (37f));
                    GUILayout.EndHorizontal ();

                    GUILayout.Space (10f);

                    sequenceDelayEventList.DoLayoutList ();
                }
            }
            GUILayout.EndVertical ();
        }

        private void InitializeSequenceEventPropertys (ATSequenceEventHelper.EditorEventInfo infos)
        {
            ATSequenceEventHelper.EditorEventInfo eventInfo = infos;
            variableNamesDictionary.Clear ();

            if ( eventInfo == null || eventInfo.eventListener.events.Count <= 0 )
                return;

            if ( eventInfo.listType == "event" + eventInfo.eventListener.gameObject.name ) {
                if ( eventInfo.eventListener.events.Count > 0 ) {

                    serializedObject = new SerializedObject (eventInfo.eventListener.events [eventInfo.sequenceIndex]);
                    serializedObject2 = new SerializedObject (eventInfo.eventListener.events [eventInfo.sequenceIndex]);

                    FieldInfo [] fields = ATSequenceEventHelper.GetSequenceFields<PropertyAttribute> (eventInfo.eventListener.events [eventInfo.sequenceIndex].GetType ());
                    FieldInfo [] triggerField = ATSequenceEventHelper.GetSequenceFields<TriggerAttribute> (eventInfo.eventListener.events [eventInfo.sequenceIndex].GetType ());
                    variableField = ATSequenceEventHelper.GetSequenceFields<VariableAttribute> (eventInfo.eventListener.events [eventInfo.sequenceIndex].GetType ());

                    sequenceEventFieldsSerializedProperty = new SerializedProperty [fields.Length];
                    sequenceEventTriggerSerializedProperty = new SerializedProperty [triggerField.Length];
                    sequenceEventVariablesSerializedProperty = new SerializedProperty [variableField.Length];
                    

                    if ( fields.Length > 0 ) {
                        for ( int i = 0 ; i < sequenceEventFieldsSerializedProperty.Length ; ++i )
                            sequenceEventFieldsSerializedProperty [i] = serializedObject.FindProperty (fields [i].Name);

                    }

                    if ( triggerField.Length > 0 ) {
                        for ( int j = 0 ; j < sequenceEventTriggerSerializedProperty.Length ; ++j )
                            sequenceEventTriggerSerializedProperty [j] = serializedObject.FindProperty (triggerField [j].Name);
                    }

                    if ( eventInfo.eventListener.events [eventInfo.sequenceIndex].GetType ().IsSubclassOf (typeof (AT.Sequence.Runtime.Action)) ) {
                        actionMethodInfos = ATSequenceEventHelper.GetSequenceActionMethods (eventInfo.eventListener.events [eventInfo.sequenceIndex].GetType ());
                        actionMethodNames = new string [actionMethodInfos.Length];

                        if ( actionMethodInfos.Length > 0 ) {
                            for ( int i = 0 ; i < actionMethodInfos.Length ; ++i )
                                actionMethodNames [i] = (actionMethodInfos [i].Name.ToString ());
                        }

                        actionMethodSerializedProperty = serializedObject2.FindProperty ("actionMethod");
                        methodNameSerializedProperty = actionMethodSerializedProperty.FindPropertyRelative ("methodName");
                    }

                    if ( variableField.Length > 0 ) {
                        Dictionary<object, object> hashDatas = new Dictionary<object, object>();
                        Scene currentScene = SceneManager.GetActiveScene ();
                        string currentSceneName = currentScene.name;
                        string sceneName = string.IsNullOrEmpty (currentSceneName) ? ("UnNamedScene" + eventInfo.eventListener.gameObject.name) : currentSceneName;

                        try {
                            hashDatas = Serializer.FromFile<Dictionary<object, object>> (dataPathLocation + sceneName + ".xml");

                        }
                        catch ( ObjectNotFoundException ) { }
                        catch( CorruptFileException ) { }

                        if ( hashDatas != null )
                            variableKeyHash = new Dictionary<object, object> (hashDatas);
                        

                        for ( int i = 0 ; i < sequenceEventVariablesSerializedProperty.Length ; ++i )
                            sequenceEventVariablesSerializedProperty [i] = serializedObject.FindProperty (variableField [i].Name);

                        for ( int i = 0 ; i < eventListeners.Count ; ++i ) {
                            for ( int j = 0 ; j < eventListeners [i].events.Count ; ++j ) {
                                ATSequenceEvent events = eventListeners [i].events [j];

                                if ( ATSequenceEventHelper.CheckSubTypes<Variable> (events.GetType ()) ) {
                                    string variableSequenceName = $"{eventListeners [i].gameObject.name} / {events.EventName}";
                                    variableNames.Add (variableSequenceName);
                                    variableNamesDictionary.Add (variableSequenceName, (Variable) events);
                                }
                            }
                        }
                    }


                    sequenceDelayEventList = new ReorderableList (eventInfos.eventListener.events [eventInfos.sequenceIndex].SequenceEvents, typeof (ATSequenceEvent), true, true, true, true) {
                        onAddDropdownCallback = (rect, list) => {
                            GenericMenu menu = new GenericMenu ();
                            List<ATSequenceEventHelper.SequenceEventInfo> pairs = GetAvalibleEvents ();

                            for ( int i = 0 ; i < pairs.Count ; ++i ) {
                                menu.AddItem (new GUIContent (pairs [i].name), false, (objects) => eventInfo.eventListener.events [eventInfo.sequenceIndex].SequenceEvents.Add ((ATSequenceEvent) objects), pairs [i].sequenceEvent);
                            }

                            menu.ShowAsContext ();
                        },

                        drawHeaderCallback = (rect) => {
                            EditorGUI.LabelField (rect, "Sequence Events");
                        },

                        drawElementCallback = (rect, index, isActive, isFocused) => {
                            Rect rect2 = new Rect (rect.x, rect.y, 267, EditorGUIUtility.singleLineHeight);
                            EditorGUI.LabelField (rect2, eventInfos.eventListener.events [eventInfos.sequenceIndex].SequenceEvents [index].EventName);
                        }
                    };
                }
            }
        }

        private void DrawSequenceEventFields ()
        {
            ATSequenceEventHelper.EditorEventInfo eventInfo = eventInfos;

            if ( eventInfo == null || eventInfo.eventListener.events.Count <= 0 )
                return;
             
            serializedObject.Update ();
            serializedObject2.Update ();

            GUI.color = new Color32 (211, 211, 211, 255);
            GUILayout.BeginVertical (EditorStyles.helpBox);
            GUI.color = Color.white;
            if ( eventInfo.listType == "event" + eventInfo.eventListener.gameObject.name && eventInfo.eventListener.events.Count > 0 ) {
                if ( eventInfo.eventListener.events.Count > 0 ) {

                    GUILayout.Label ("Edit Properties : ", EditorStyles.whiteLargeLabel);
                     

                    for ( int i = 0 ; i < sequenceEventFieldsSerializedProperty.Length ; ++i ) {
                        EditorGUILayout.PropertyField( (sequenceEventFieldsSerializedProperty [i]), true);
                        GUILayout.Space (3f);
                    }

                    if ( variableNames.Count > 0 ) {
                        if ( variableField.Length > 0 ) {
                            for ( int i = 0 ; i < variableNamesDictionary.Count ; ++i ) {
                                variableKeyNames.Add (variableNamesDictionary.ElementAt (i));
                            }


                            Scene currentScene = SceneManager.GetActiveScene ();
                            string currentSceneName = currentScene.name;
                            string sceneName = string.IsNullOrEmpty (currentSceneName) ? ("UnNamedScene" + eventInfo.eventListener.gameObject.name) : currentSceneName;

                            for ( int j = 0 ; j < sequenceEventVariablesSerializedProperty.Length ; ++j ) {

                                string labelName = variableField [j].Name.ToString ();
                                object key = currentSceneName + eventInfo.eventListener.gameObject.name + eventInfo.eventListener.events [eventInfo.sequenceIndex].EventName + labelName;

                                if ( !variableKeyHash.ContainsKey (key) )
                                    variableKeyHash.Add (key, 0);

                                variableKeyHash [key] = (int) GetVariableIndex (variableNames [(int) variableKeyHash [key]]);

                                EditorGUI.BeginChangeCheck ();

                                variableKeyHash [key] = EditorGUILayout.Popup (labelName, (int) variableKeyHash [key], variableNames.ToArray ());

                                if ( EditorGUI.EndChangeCheck () ) {
                                    Serializer.ToFile (variableKeyHash, dataPathLocation + sceneName + ".xml");
                                }
                                
                                if ( variableNamesDictionary.TryGetValue (variableNames [(int) variableKeyHash [key]], out Variable variable) ) {
                                    sequenceEventVariablesSerializedProperty [j].objectReferenceValue = variable;
                                }
                            }
                        }
                    }

                    GUILayout.Space (5f);

                    if ( ATSequenceEventHelper.CheckSubTypes<AT.Sequence.Runtime.Action> (eventInfo.eventListener.events [eventInfo.sequenceIndex].GetType ()) ) {

                        actionMethodsIndex = GetActionMethodsIndex (methodNameSerializedProperty.stringValue);
                        actionMethodsIndex = EditorGUILayout.Popup ("Call Method", actionMethodsIndex, actionMethodNames.ToArray ());
                        methodNameSerializedProperty.stringValue = actionMethodInfos [actionMethodsIndex].Name;

                    }

                    GUILayout.Space (5f);

                    GUILayout.Label ("Events : ", EditorStyles.whiteLargeLabel);

                    for(int j = 0 ; j < sequenceEventTriggerSerializedProperty.Length ; ++j ) {
                        EditorGUILayout.PropertyField (sequenceEventTriggerSerializedProperty [j], true);
                        GUILayout.Space (5f);
                    }
                }
            }
            serializedObject.ApplyModifiedProperties ();
            serializedObject2.ApplyModifiedProperties ();
            GUILayout.EndVertical ();
        }

        private List<ATSequenceEventHelper.SequenceEventInfo> GetAvalibleEvents()
        {
            List<ATSequenceEventHelper.SequenceEventInfo> list = new List<ATSequenceEventHelper.SequenceEventInfo> ();

            for(int i = 0 ; i < eventInfos.eventListener.events.Count ; ++i ) {
                if( eventInfos.eventListener.events[eventInfos.sequenceIndex] != eventInfos.eventListener.events [i] ) {
                    list.Add (new ATSequenceEventHelper.SequenceEventInfo ("Sequence Event/" + eventInfos.eventListener.events [i].EventName, eventInfos.eventListener.events [i]));
                }

            }
            return list;
        }

        private int GetActionMethodsIndex(string value)
        {
            string name = value;
            int index = 0;

            foreach(MethodInfo method in actionMethodInfos ) {
                if ( method.Name == name )
                    return index;

                index++;
            }

            return 0; 
        }

        private int GetVariableIndex(string value)
        {
            string key = value;
            int index = 0;

            foreach(var variable in variableKeyNames ) {
                if ( variable.Key == key )
                    return index;

                index++;
            }
            return 0;
        }

        internal static class Style
        {
            internal static readonly GUIStyle baseStyle;

            static Style ()
            {
                baseStyle = new GUIStyle () {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState () {
                        textColor = Color.white
                    }
                };
            }
        }
    }
}
