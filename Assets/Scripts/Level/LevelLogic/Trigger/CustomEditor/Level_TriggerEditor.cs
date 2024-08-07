using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Level_Trigger))]
public class Level_TriggerEditor : Editor
{
    SerializedProperty Type;
    SerializedProperty Room;

    void GetSerializedProperty()
    {
        Type = serializedObject.FindProperty("type");
        Room = serializedObject.FindProperty("room");
    }

    public override void OnInspectorGUI()
    {
        


        GetSerializedProperty();
        serializedObject.Update();
        Level_Trigger tar = (Level_Trigger)target;
        tar.enabled = EditorGUILayout.Toggle("Enable", tar.enabled);

        EditorGUI.BeginDisabledGroup(!tar.enabled);
        EditorGUILayout.PropertyField(Type);

        EditorGUILayout.Space(5f);
        switch ((Level_Trigger.Type)Type.enumValueIndex)
        {
            case Level_Trigger.Type.ROOM_SWITCH:
                DisplayRoomSwitch();
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUI.EndDisabledGroup();
    }

    void DisplayRoomSwitch()
    {
        EditorGUILayout.LabelField("ROOMS",new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold
        });

        EditorGUILayout.PropertyField(Room);
    }
}
#endif