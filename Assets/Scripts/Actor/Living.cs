using UnityEngine;

namespace Systems.Actor
{
    public class Living : Actor
    {
        protected Vector3 velcoity;
        protected float gravityMultiplier = 1f;
        private Vector3 lastPos;

        protected virtual void Update()
        {
            // Calculate the velocity.
            velcoity = (transform.position - lastPos) / Time.deltaTime;
        }

        public Vector3 GetVelocity() => velcoity;
        public float GetSpeed() => velcoity.magnitude;
        public float GetGravityMultiplier() => gravityMultiplier;
    }
}