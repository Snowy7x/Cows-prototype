using UnityEngine;

namespace Snowy.Editor.Inspectors
{
    [UnityEditor.CustomEditor(typeof(Systems.Actor.Actor), true)]
    public class ActorEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            
            // Draw a line in the inspector
            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.LabelField("Damageable", UnityEditor.EditorStyles.boldLabel);
            UnityEditor.EditorGUILayout.LabelField("Health", ((Systems.Actor.Actor)target).Health.ToString());
            UnityEditor.EditorGUILayout.LabelField("Is Alive", ((Systems.Actor.Actor)target).IsAlive.ToString());

            if (GUILayout.Button("Take Damage"))
            {
                ((Systems.Actor.Actor)target).TakeDamage(10f);
            }

            if (GUILayout.Button("Heal"))
            {
                ((Systems.Actor.Actor)target).Heal(10f);
            }

            if (GUILayout.Button("Die"))
            {
                ((Systems.Actor.Actor)target).Die();
            }
        }
    }
}