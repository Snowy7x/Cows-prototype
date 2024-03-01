using System;
using UnityEngine;

namespace Systems.IK.Base
{
    [Serializable] public class FollowTarget
    {
        public bool rotation;
        public bool position;
        
        public Transform transform;
        public Transform target;
        
        public void Init()
        {
        }
        
        public void Resolve()
        {
            if (target == null)
                return;
            
            if (rotation)
            {
                transform.rotation = target.rotation;
            }
            
            if (position)
            {
                transform.position = target.position;
            }
        }
    }
}