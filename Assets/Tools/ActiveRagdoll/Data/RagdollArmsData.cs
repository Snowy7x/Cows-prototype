using System;
using UnityEngine;

namespace _Tests.Tool.ActiveRagdoll.Data
{
    [Serializable]public class ArmData
    {
        public Vector3 upperTargetVel;
        public Vector3 lowerTargetVel;
        public float rotationSpring = 100;
        public float smoothTime = 0.1f;
    }

    [Serializable]public class RagdollArmData
    {
        public ArmData idle = new ArmData();
        public ArmData pickUp = new ArmData();
        public ArmData punch = new ArmData();
        public ArmData punchThrow = new ArmData();
    }
    
    [CreateAssetMenu(fileName = "RagdollArmData", menuName = "Snowy/Ragdoll/ArmData", order = 0)]
    public class RagdollArmsData : ScriptableObject
    {
        [Header("Left Arm")]
        public RagdollArmData leftArmData;
        
        [Header("Right Arm")]
        public RagdollArmData rightArmData;
    }
}