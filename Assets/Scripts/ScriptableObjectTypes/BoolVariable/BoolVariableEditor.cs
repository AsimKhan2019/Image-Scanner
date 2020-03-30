using UnityEditor;
using NaughtyAttributes;

#if UnityEditor
[CustomEditor(typeof(BoolVariable))]
[CanEditMultipleObjects]
public class BoolVariableCustomEditor : Editor 
{
    SerializedProperty v;
    SerializedProperty initV;
    
    void OnEnable()
    {
        initV = serializedObject.FindProperty("InitialValue");
        v = serializedObject.FindProperty("Value");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //EditorGUILayout.LabelField("Value");
      ///  EditorGUILayout.PropertyField(v);
        EditorGUILayout.PropertyField(initV);
                EditorGUILayout.PropertyField(v);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif