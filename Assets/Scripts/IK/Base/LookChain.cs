using System;
using UnityEngine;

namespace Systems.IK.Base
{
    [Serializable] public class LookChain
    {
        public Transform transform;
        /// <summary>
        /// Look at target
        /// </summary>
        public Transform Target;
        public bool useLookAt = false;
        public bool x = true;
        public bool y = true;
        public bool z = true;
        
        public void Init()
        {
            if (Target == null)
                return;
        }

        public void Resolve()
        {
            if (Target == null)
                return;
            
            Quaternion rotation = Quaternion.LookRotation(Target.position - transform.position);
            
            if (useLookAt)
            {
                transform.LookAt(Target);
            } 
            else
            {
                Vector3 euler = rotation.eulerAngles;
                if (!x) euler.x = 0f;
                if (!y) euler.y = 0f;
                if (!z) euler.z = 0f;
                transform.rotation = Quaternion.Euler(euler);
            }
        }
    }
}