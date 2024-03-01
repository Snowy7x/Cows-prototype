using System.Collections;
using UnityEngine;

namespace _Tests.Tool.ActiveRagdoll
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] float breakLimit = 5000;
        [SerializeField] float handPunchForce = 100f;
        private ConfigurableJoint joint;
        private Rigidbody rb;
        [HideInInspector] public bool isPunching = false;
        bool isGrapping = false;
        FixedJoint fixedJoint;
        bool canGrap = false;

        private void Awake()
        {
            joint = GetComponent<ConfigurableJoint>();
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (isPunching)
            {
                if (other.rigidbody)
                {
                    ContactPoint contact = other.contacts[0];
                    foreach (ContactPoint contactPoint in other.contacts)
                    {
                        if (contactPoint.otherCollider == other.collider)
                        {
                            contact = contactPoint;
                            break;
                        }
                    }
                    
                    // Calculate direction from the contact point to the center of the object
                    Vector3 forward = other.transform.position - contact.point;
                    other.rigidbody.AddForceAtPosition(forward * handPunchForce, contact.point, ForceMode.Impulse);
                }
                return;
            }
            
            if (isGrapping || !canGrap || fixedJoint != null) return;
            if (other.gameObject.CompareTag("Grappable"))
            {
                // Grape the object
                if (other.rigidbody)
                {
                    FixedJoint fixedJoint = other.gameObject.AddComponent<FixedJoint>();
                    //fixedJoint.autoConfigureConnectedAnchor = true;
                    //fixedJoint.anchor = joint.anchor;
                    //fixedJoint.connectedAnchor = joint.connectedAnchor;
                    //fixedJoint.xMotion = ConfigurableJointMotion.Locked;
                    //fixedJoint.yMotion = ConfigurableJointMotion.Locked;
                    //fixedJoint.zMotion = ConfigurableJointMotion.Locked;
                    fixedJoint.connectedBody = rb;
                    fixedJoint.breakForce = breakLimit;
                    fixedJoint.breakTorque = breakLimit;
                    isGrapping = true;
                    this.fixedJoint = fixedJoint;
                }
            }
        }
        
        public void CanGrap(bool canGrap)
        {
            this.canGrap = canGrap;
        }
        
        public void Release()
        {
            if (isGrapping)
            {
                try
                {
                    Destroy(fixedJoint);
                }catch {}
                isGrapping = false;
            }
        }
        
        public bool SetIsPunching(bool isPunching)
        {
            this.isPunching = isPunching;
            if (isPunching)
            {
                StartCoroutine(ResetIsPunching());
            }
            return isPunching;
        }
        
        IEnumerator ResetIsPunching()
        {
            yield return new WaitForSeconds(0.5f);
            isPunching = false;
        }
    }
}