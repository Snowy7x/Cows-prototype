using System;
using UnityEditor;
using UnityEngine;

namespace _Tests.Tool.ActiveRagdoll
{
    
    
    public class RagdollValuesEditor : EditorWindow
    {
        private enum Shown
        {
            Group,
            Individual
        }
        
        // Button Design
        private const int ButtonHeight = 30;
        private GUILayoutOption[] ButtonOptions = {GUILayout.ExpandWidth(true), GUILayout.Height(ButtonHeight)};

        private ArmatureInfo armatureInfo
        {
            get => ragdollArmature.armatureInfo;
            set => ragdollArmature.armatureInfo = value;
        }
        
        // Individual Values
        private bool loadedValues = false;
        
        private Shown shown = Shown.Group;
        private RagdollArmature ragdollArmature;
        private Vector2 scrollPosition = Vector2.zero;
        
        [MenuItem("Custom Tools/Values Editor")]
        public static void ShowWindow()
        {
            GetWindow<RagdollValuesEditor>("Ragdoll Editor Values");
        }

        private void OnGUI()
        {
            // Scroll View
            // Check if the ragdoll armature is selected
            if (Selection.activeGameObject == null ||
                (ragdollArmature = Selection.activeGameObject.GetComponentInParent<RagdollArmature>()) == null)
            {
                GUILayout.Label("Select a Ragdoll Armature");
                return;
            }
            
            ragdollArmature = Selection.activeGameObject.GetComponentInParent<RagdollArmature>();
            if (ragdollArmature.armatureInfo.hips.rigidbody == null)
            {
                ragdollArmature.SetUp_Animator(ragdollArmature.GetComponent<Animator>());
            }
            // horizontal Buttons to switch between Group and Individual
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Group", ButtonOptions)) shown = Shown.Group;
            if (GUILayout.Button("Individual", ButtonOptions)) shown = Shown.Individual;
            
            GUILayout.EndHorizontal();
            GUILayout.BeginScrollView(scrollPosition, false, false);

            // Render the values
            switch (shown)
            {
                case Shown.Group:
                    RenderGroupValues();
                    break;
                case Shown.Individual:
                    RenderIndividualValues();
                    break;
            }
            
            GUILayout.EndScrollView();
        }


        private void RenderGroupValues()
        {
            armatureInfo.hips = RenderBoneValues(armatureInfo.hips, "Root");
            armatureInfo.chest = RenderBoneValues(armatureInfo.chest, "Core");
            armatureInfo.arms = RenderBoneValues(armatureInfo.arms, "Arms");
            armatureInfo.legs = RenderBoneValues(armatureInfo.legs, "Legs");
        }
        
        
        private void RenderIndividualValues()
        {
            armatureInfo.head = RenderBoneValues(armatureInfo.head, "Head");
            armatureInfo.chest = RenderBoneValues(armatureInfo.chest, "Chest");
            armatureInfo.hips = RenderBoneValues(armatureInfo.hips, "Hips");
            armatureInfo.arms.left = RenderBoneValues(armatureInfo.arms.left, "Left Arm");
            armatureInfo.arms.right = RenderBoneValues(armatureInfo.arms.right, "Right Arm");
            armatureInfo.legs.left = RenderBoneValues(armatureInfo.legs.left, "Left leg");
            armatureInfo.legs.right = RenderBoneValues(armatureInfo.legs.right, "Right leg");
            
            // Apply the values to all bones
            // TODO: Apply
        }
        
        private BoneInfo RenderBoneValues(BoneInfo values, string boneName)
        {
            // But them in box
            GUILayout.BeginVertical("Box");
            values.isFolded = EditorGUILayout.Foldout(values.isFolded, boneName);
            GUILayout.EndVertical();
            if (values.isFolded)
            {
                EditorGUI.indentLevel++;
                values.mass = EditorGUILayout.FloatField("Mass", values.mass);
                values.rotationSpring = EditorGUILayout.FloatField("Rotation Spring", values.rotationSpring);
                values.rotationDamper = EditorGUILayout.FloatField("Rotation Damper", values.rotationDamper);
                values.rotationMaxForce = EditorGUILayout.FloatField("Rotation Max Force", values.rotationMaxForce);
                values.strengthMultiplier = EditorGUILayout.FloatField("Strength Multiplier", values.strengthMultiplier);
                // Slider
                values.strengthWeight = EditorGUILayout.Slider("Strength Weight", values.strengthWeight, 0, 1);
                EditorGUI.indentLevel--;
            }
            
            return values;
        }

        private DoubleLimbInfo RenderBoneValues(DoubleLimbInfo values, string boneName)
        {
            // But them in box
            GUILayout.BeginVertical("Box");
            values.isFolded = EditorGUILayout.Foldout(values.isFolded, boneName);
            GUILayout.EndVertical();
            if (values.isFolded)
            {
                EditorGUI.indentLevel++;
                float mass = EditorGUILayout.FloatField("Mass", values.left.upper.mass);
                float rotationSpring = EditorGUILayout.FloatField("Rotation Spring", values.left.upper.rotationSpring);
                float rotationDamper = EditorGUILayout.FloatField("Rotation Damper", values.left.upper.rotationDamper);
                float rotationMaxForce = EditorGUILayout.FloatField("Rotation Max Force", values.left.upper.rotationMaxForce);
                
                // Apply the values to all bones
                values.left = ApplyLimbValues(values.left, mass, rotationSpring, rotationDamper, rotationMaxForce);
                values.right = ApplyLimbValues(values.right, mass, rotationSpring, rotationDamper, rotationMaxForce);
                
                //values.strengthMultiplier = EditorGUILayout.FloatField("Strength Multiplier", values.strengthMultiplier);
                // Slider
                //values.strengthWeight = EditorGUILayout.Slider("Strength Weight", values.strengthWeight, 0, 1);
                EditorGUI.indentLevel--;
            }

            return values;
        }

        private LimbInfo RenderBoneValues(LimbInfo values, string boneName)
        {
            GUILayout.BeginVertical("Box");
            values.isFolded = EditorGUILayout.Foldout(values.isFolded, boneName);
            GUILayout.EndVertical();
            if (values.isFolded)
            {
                EditorGUI.indentLevel++;
                float mass = EditorGUILayout.FloatField("Mass", values.upper.mass);
                float rotationSpring = EditorGUILayout.FloatField("Rotation Spring", values.upper.rotationSpring);
                float rotationDamper = EditorGUILayout.FloatField("Rotation Damper", values.upper.rotationDamper);
                float rotationMaxForce = EditorGUILayout.FloatField("Rotation Max Force", values.upper.rotationMaxForce);
                
                // Apply the values to all bones
                values = ApplyLimbValues(values, mass, rotationSpring, rotationDamper, rotationMaxForce);
                
                //values.strengthMultiplier = EditorGUILayout.FloatField("Strength Multiplier", values.strengthMultiplier);
                // Slider
                //values.strengthWeight = EditorGUILayout.Slider("Strength Weight", values.strengthWeight, 0, 1);
                EditorGUI.indentLevel--;
            }
            
            return values;
        }

        private LimbInfo ApplyLimbValues(LimbInfo info, float mass, float rotationSpring, float rotationDamper, float rotationMaxForce)
        {
            // Apply the values to all bones
            info.upper.mass = info.lower.mass = info.end.mass = mass;
            info.upper.rotationSpring = info.lower.rotationSpring = info.end.rotationSpring = rotationSpring;
            info.upper.rotationDamper = info.lower.rotationDamper = info.end.rotationDamper = rotationDamper;
            info.upper.rotationMaxForce = info.lower.rotationMaxForce = info.end.rotationMaxForce = rotationMaxForce;
            return info;
        }
    }
}