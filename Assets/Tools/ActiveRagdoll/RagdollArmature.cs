using UnityEngine;

namespace _Tests.Tool.ActiveRagdoll
{
    public class RagdollArmature : MonoBehaviour
    {
        public ArmatureInfo armatureInfo = new ArmatureInfo();
        

        public void SetUp_Animator(Animator anim)
        {
            BoneInfo head = GetBoneInfo_Animator(anim, HumanBodyBones.Head);
            BoneInfo hips = GetBoneInfo_Animator(anim, HumanBodyBones.Hips);
            BoneInfo chest = GetBoneInfo_Animator(anim, HumanBodyBones.Spine);
            DoubleLimbInfo arms = new ();
            arms.left = GetLimbInfo_Animator(anim, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);
            arms.right = GetLimbInfo_Animator(anim, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);
            
            DoubleLimbInfo legs = new ();
            legs.left = GetLimbInfo_Animator(anim, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot);
            legs.right = GetLimbInfo_Animator(anim, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot);
            
            armatureInfo.head = head;
            armatureInfo.hips = hips;
            armatureInfo.chest = chest;
            armatureInfo.arms = arms;
            armatureInfo.legs = legs;
        }
        
        public LimbInfo GetLimbInfo_Animator(Animator anim, HumanBodyBones upperBone, HumanBodyBones lowerBone, HumanBodyBones endBone)
        {
            LimbInfo limbInfo = new LimbInfo();
            limbInfo.upper = GetBoneInfo_Animator(anim, upperBone);
            limbInfo.lower = GetBoneInfo_Animator(anim, lowerBone);
            limbInfo.end = GetBoneInfo_Animator(anim, endBone);
            return limbInfo;
        }

        public BoneInfo GetBoneInfo_Animator(Animator anim, HumanBodyBones bone)
        {
            Transform boneTransform = anim.GetBoneTransform(bone);
            ConfigurableJoint joint = boneTransform.GetComponent<ConfigurableJoint>();
            if (joint) joint.rotationDriveMode = RotationDriveMode.Slerp;
            Rigidbody rigidbody = boneTransform.GetComponent<Rigidbody>();
            BoneCollider boneCollider = new BoneCollider();
            boneCollider.colliderType = GetColliderType(rigidbody);
            BoneInfo boneInfo = new BoneInfo()
            {
                bone = boneTransform,
                joint = joint,
                rigidbody = rigidbody,
                boneCollider = boneCollider
            };
            return boneInfo;
        }

        private ColliderType GetColliderType(Rigidbody rb)
        {
            if (rb == null || rb.GetComponent<Collider>() == null)
            {
                return ColliderType.None;
            }
            
            Collider col = rb.GetComponent<Collider>();
            if (col.enabled == false)
            {
                return ColliderType.None;
            }
            
            if (col is CapsuleCollider)
            {
                return ColliderType.Capsule;
            }
            
            if (col is SphereCollider)
            {
                return ColliderType.Sphere;
            }
            
            if (col is BoxCollider)
            {
                return ColliderType.Box;
            }
            
            return ColliderType.None;
        }
            
        
        [ContextMenu("Test")]
        public void Test()
        {
            Debug.Log(armatureInfo.head.mass);
        }
    }
}