using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyHealth))]
public class EnemyHealthCustomEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("health"));

        EnemyHealth enemyHealth = (EnemyHealth) target;
        if (GUILayout.Button("Damage"))
        {
            enemyHealth.enemyHealth.Damage(70);
        }
        if (GUILayout.Button("Heel/Heal"))
        {
            enemyHealth.enemyHealth.Heal(70);
        }
    }
    
}
