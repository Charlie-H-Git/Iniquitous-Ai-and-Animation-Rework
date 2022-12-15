using UnityEditor;
using UnityEngine;


    [CustomEditor(typeof(AiAgent))]
    public class AiAgentCustomEditor : UnityEditor.Editor
    {
        public enum VariableCategory
        {
            Default,
            StateInfo,
            Global,
            Idle,
            Movement,
            Attack,
            CombatStance
        }

        public VariableCategory variableCategory;

        public override void OnInspectorGUI()
        {
            variableCategory = (VariableCategory) EditorGUILayout.EnumPopup("Display", variableCategory);
            EditorGUILayout.Space();
            switch (variableCategory)
            {
                case VariableCategory.Default:
                    DrawDefaultInspector();
                    break;
                case VariableCategory.StateInfo:
                    DisplayStateInfo();
                    break;
                case VariableCategory.Global:
                    DisplayGlobalInfo();
                    break;
                case VariableCategory.Idle:
                    DisplayIdleInfo();
                    break;
                case VariableCategory.Movement:
                    DisplayMovementInfo();
                    break;
                case VariableCategory.Attack:
                    DisplayAttackInfo();
                    break;
                case VariableCategory.CombatStance:
                    DisplayCombatStanceInfo();
                    break;
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        void DisplayStateInfo()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentState"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialState"));
        }

        void DisplayGlobalInfo()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("navMeshAgent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stateText"));
        }

        void DisplayIdleInfo()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("angleFromPlayer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceFromPlayer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("los"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("inCone"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("canSee"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("coneAngle"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("detectionDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("combatMask"));
        }

        void DisplayMovementInfo()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_enemyRigidbody"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSpeed"));
        }

        void DisplayAttackInfo()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumAttackRange"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyAttacks"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isAttacking"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentRecoveryTime"));
        }

        void DisplayCombatStanceInfo()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("circleRadius"));
        }
    }

