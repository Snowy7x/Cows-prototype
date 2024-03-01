using System;
using UnityEngine;

namespace _Tests.Tool.ActiveRagdoll.Data
{
    [Serializable]public class StepInfo
    {
        public AnimationCurve upperLegCurve;
        public AnimationCurve lowerLegCurve;
        public float stepDuration;
        public float stepMultiplier;
    }
    
    [CreateAssetMenu(fileName = "RagdollStepData", menuName = "Snowy/Ragdoll/StepData", order = 0)]
    public class RagdollStepData : ScriptableObject
    {
        [Header("Forward")]
        public StepInfo forwardStepInfo;
        
        [Header("Backward")]
        public StepInfo backwardStepInfo;
    }
}