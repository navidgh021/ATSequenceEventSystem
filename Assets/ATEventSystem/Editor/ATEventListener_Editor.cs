using UnityEngine;
using UnityEditor;
using AT.Sequence.Runtime;

namespace AT.Sequence.Editor
{ 
    [CustomEditor(typeof(ATEvent))]
    public class ATEventListener_Editor : UnityEditor.Editor
    {
        private ATEvent atEventListener;

        private SerializedProperty serializedProperty;

        private void OnEnable ()
        {
            atEventListener = (ATEvent) target;
            serializedProperty = serializedObject.FindProperty ("events");
        }

        public override void OnInspectorGUI ()
        {
            EditorGUILayout.Space ();
            EditorGUILayout.LabelField ("AT Event", Style.style);
            EditorGUILayout.Space ();

            GUI.color = new Color32 (150, 150, 150, 255);
            GUILayout.BeginVertical ("Window", GUILayout.Height (1f));


            GUI.color = new Color32 (255, 255, 255, 255);
            GUILayout.BeginVertical ("Button");
            



            GUILayout.BeginVertical ("HelpBox");

            EditorGUI.indentLevel++;

            if ( atEventListener.events.Count <= 0 ) {
                EditorGUILayout.LabelField ("Unavalible sequence event", Style.style2);
            }
            else {
                EditorGUILayout.LabelField ("sequence events", Style.style2);
                EditorGUILayout.Space ();
                for ( int i = 0 ; i < serializedProperty.arraySize ; ++i ) {
                    ATSequenceEvent sequenceEvent = serializedProperty.GetArrayElementAtIndex (i).objectReferenceValue as ATSequenceEvent;
                    GUILayout.BeginHorizontal ();
                    GUILayout.Label ("Event " + (i + 1) + " : ", GUILayout.Width (65f));
                    GUILayout.Label (sequenceEvent.EventName);
                    GUILayout.EndHorizontal ();
                }
            }

            EditorGUI.indentLevel--;


            GUILayout.EndVertical ();

            GUILayout.Space (10f);
            if(GUILayout.Button("Open Editor") ) {
                ATSequenceEvent_Window.OpenWindow ();
            }
            GUILayout.Space (10f);

            GUILayout.EndVertical ();
            GUILayout.EndVertical ();
            EditorUtility.SetDirty (atEventListener);
        }

        internal static class Style
        {
            internal static readonly GUIStyle style;

            internal static readonly GUIStyle style2;

            static Style()
            {
                style = new GUIStyle () {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = new GUIStyleState () {
                        textColor = Color.white
                    },
                    fontSize = 24,
                };

                style2 = new GUIStyle () {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = new GUIStyleState () {
                        textColor = Color.white
                    },
                    fontSize = 19,

                };
            }
        }
    }
}