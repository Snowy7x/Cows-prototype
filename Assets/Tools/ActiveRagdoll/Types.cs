using System;
using UnityEngine;

namespace _Tests.Tool.ActiveRagdoll
{
    public enum Bipod
    {
        Left,
        Right
    }
    
    public enum ArmState
    {
        Idle,
        PickUp,
        Punch,
        PunchThrow
    }
    
    public class BoneInfo
    {
        public Transform bone;
        public ConfigurableJoint joint;
        public Rigidbody rigidbody;
        public BoneCollider boneCollider;

        public float mass
        {
            get => rigidbody == null ? 0 : rigidbody.mass;
            set
            {
                if (rigidbody != null)
                {
                    rigidbody.mass = value;
                }
            }
        }

        public float strengthMultiplier = 1f;
        public float strengthWeight = 1f;
        
        public float rotationSpring
        {
            get => joint == null ? 0 : joint.slerpDrive.positionSpring;
            set
            {
                if (joint != null)
                {
                    joint.slerpDrive = new JointDrive()
                    {
                        positionSpring = Mathf.Pow(value, strengthMultiplier * strengthWeight),
                        positionDamper = joint.slerpDrive.positionDamper,
                        maximumForce = joint.slerpDrive.maximumForce
                    };
                }
            }
        }
        
        public float rotationDamper
        {
            get => joint == null ? 0 : joint.slerpDrive.positionDamper;
            set
            {
                if (joint != null)
                {
                    joint.slerpDrive = new JointDrive()
                    {
                        positionSpring = joint.slerpDrive.positionSpring,
                        positionDamper = value,
                        maximumForce = joint.slerpDrive.maximumForce
                    };
                }
            }
        }
        
        public float rotationMaxForce
        {
            get => joint == null ? 0 : joint.slerpDrive.maximumForce;
            set
            {
                if (joint != null)
                {
                    joint.slerpDrive = new JointDrive()
                    {
                        positionSpring = joint.slerpDrive.positionSpring,
                        positionDamper = joint.slerpDrive.positionDamper,
                        maximumForce = value
                    };
                }
            }
        }
        
        public bool isFolded;
    }
    
    public struct LimbInfo
    {
        public BoneInfo upper;
        public BoneInfo lower;
        public BoneInfo end;
        public bool isFolded;
    }
    
    public struct DoubleLimbInfo
    {
        public LimbInfo left;
        public LimbInfo right;
        public bool isFolded;
    }
    
    public class ArmatureInfo
    {
        public BoneInfo head = new ();
        public BoneInfo hips = new ();
        public BoneInfo chest = new ();
        public DoubleLimbInfo arms = new ();
        public DoubleLimbInfo legs = new ();
    }
    
    public enum ColliderType
    {
        Capsule,
        Sphere,
        Box,
        None
    }
    
    public struct BoneCollider
    {
        public ColliderType colliderType;
        public float radius;
        public float height;
        public float width;
        public float depth;
    }
}