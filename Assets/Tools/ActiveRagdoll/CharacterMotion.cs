using System;
using System.Collections;
using _Tests.Tool.ActiveRagdoll;
using _Tests.Tool.ActiveRagdoll.Data;
using UnityEngine;

public class CharacterMotion : MonoBehaviour
{
    [SerializeField] RagdollArmature ragdollArmature;
    [SerializeField] RagdollStepData stepInfo;
    [SerializeField] RagdollArmsData armsInfo;
    [SerializeField] float hipsForwardForce = 10f;
    
    bool isStepping;
    ArmState leftArmState;
    ArmState rightArmState;
    IEnumerator leftArmCoroutine;
    IEnumerator rightArmCoroutine;
    Hand leftHand;
    Hand rightHand;
    
    public RagdollArmature RagdollArmature => ragdollArmature;
    Rigidbody hips;
    
    bool isPunching = false;
    
    private void Start()
    {
        ragdollArmature.SetUp_Animator(GetComponent<Animator>());
        SetArmTo(Bipod.Left, ArmState.Idle);
        SetArmTo(Bipod.Right, ArmState.Idle);
        rightHand = ragdollArmature.armatureInfo.arms.right.end.bone.GetComponent<Hand>() ?? ragdollArmature.armatureInfo.arms.right.end.bone.gameObject.AddComponent<Hand>();
        leftHand = ragdollArmature.armatureInfo.arms.left.end.bone.GetComponent<Hand>() ?? ragdollArmature.armatureInfo.arms.left.end.bone.gameObject.AddComponent<Hand>();
        
        hips = ragdollArmature.armatureInfo.hips.rigidbody;
    }

    private void FixedUpdate()
    {
        // Counter movement
        Vector3 hipsForward = ragdollArmature.armatureInfo.hips.bone.forward;
        Vector3 hipsVelocity = hips.velocity;
        float forwardVelocity = Vector3.Dot(hipsForward, hipsVelocity);
        if (forwardVelocity > 0)
        {
            hips.AddForce(-hipsForward * forwardVelocity * 0.5f, ForceMode.Acceleration);
        }
    }

    public void LookIntoDirection(Vector3 direction, float speed = 10f)
    {
        RotateTowardsDirection(direction, ragdollArmature.armatureInfo.hips, speed);
    }
    
    public void RotateTowardsTarget(Transform target, BoneInfo bone, float speed)
    {
        Vector3 vecToTarget = target.position - bone.bone.position;
        Vector3 vecToTargetFlat = Vector3.Scale(vecToTarget, new Vector3(1, 0, 1));
        RotateTowardsDirection(vecToTargetFlat, bone, speed);
    }

    public void RotateTowardsDirection(Vector3 dir, BoneInfo bone, float speed)
    {
        //Vector3 vecToTargetFlat = Vector3.Scale(dir, new Vector3(1, 0, 1));
        Quaternion lookRotation = Quaternion.Inverse(Quaternion.LookRotation(dir, Vector3.up));
            
        // remove y rotation
        lookRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
            
        Quaternion lerpValue = Quaternion.Lerp(bone.joint.targetRotation, lookRotation, speed * Time.fixedDeltaTime);
        bone.joint.targetRotation = lerpValue;
    }

    public void Step()
    {
        LimbInfo leftLeg = ragdollArmature.armatureInfo.legs.left;
        LimbInfo rightLeg = ragdollArmature.armatureInfo.legs.right;
        StepInfo step = stepInfo.forwardStepInfo;
        if (!isStepping)
        {
            LimbInfo leg = GetBackLeg(leftLeg, rightLeg);
            StartCoroutine(StepCoroutine(leg, step));
        }
    }

    private LimbInfo GetBackLeg(LimbInfo leftLeg, LimbInfo rightLeg)
    {
        // Check which leg is in front
        Vector3 hipsPos = ragdollArmature.armatureInfo.hips.bone.position;
        Vector3 leftLegPos = leftLeg.end.bone.position;
        Vector3 rightLegPos = rightLeg.end.bone.position;
        Vector3 leftLegToHips = leftLegPos - hipsPos;
        Vector3 rightLegToHips = rightLegPos - hipsPos;
        Vector3 forward = ragdollArmature.armatureInfo.hips.bone.forward;
        float leftAngle = Vector3.Angle(forward, leftLegToHips);
        float rightAngle = Vector3.Angle(forward, rightLegToHips);
        
        if (leftAngle < rightAngle)
        {
            return rightLeg;
        }
        else
        {
            return leftLeg;
        }
    }

    private IEnumerator StepCoroutine(LimbInfo leg, StepInfo step)
    {
        isStepping = true;
        float time = 0;
        while (time < step.stepDuration)
        {
            time += Time.deltaTime;
            float upperLegValue = step.upperLegCurve.Evaluate(time / step.stepDuration) * step.stepMultiplier;
            float lowerLegValue = step.lowerLegCurve.Evaluate(time / step.stepDuration) * step.stepMultiplier;
            leg.upper.joint.targetRotation = Quaternion.Euler(upperLegValue, 0, 0);
            leg.lower.joint.targetRotation = Quaternion.Euler(lowerLegValue, 0, 0);
            yield return null;
        }
        isStepping = false;
    }

    public void SetArmTo(Bipod arm, ArmState state)
    {
        ArmState pState = arm == Bipod.Left ? leftArmState : rightArmState;
        if (pState == ArmState.Punch && state != ArmState.Punch)
        {
            state = ArmState.PunchThrow;
        }
        
        if (arm == Bipod.Left)
        {
            if (leftArmCoroutine != null) StopCoroutine(leftArmCoroutine);
            leftArmState = state;
        }
        else
        {
            if (rightArmCoroutine != null) StopCoroutine(rightArmCoroutine);
            rightArmState = state;
        }
        
        LimbInfo joint = arm == Bipod.Left ? ragdollArmature.armatureInfo.arms.left : ragdollArmature.armatureInfo.arms.right;
        RagdollArmData armData = arm == Bipod.Left ? armsInfo.leftArmData : armsInfo.rightArmData;
        Hand hand = arm == Bipod.Left ? leftHand : rightHand;

        if (hand && hand.isPunching)
        {
            return;
        }
        
        switch (state)
        {
            case ArmState.Idle:
                SetArmsRot(joint, armData.idle);
                break;
            case ArmState.PickUp:
                SetArmsRot(joint, armData.pickUp);
                hand.CanGrap(true);
                break;
            case ArmState.Punch:
                SetArmsRot(joint, armData.punch);
                break;
            case ArmState.PunchThrow:
                StartCoroutine(PunchThrow(joint, armData.punchThrow, hand));
                break;
        }
    }
    
    private void SetArmsRot(LimbInfo limb, ArmData armData)
    {
        
        limb.upper.joint.targetRotation = Quaternion.Euler(armData.upperTargetVel);
        limb.lower.joint.targetRotation = Quaternion.Euler(armData.lowerTargetVel);
        
        limb.upper.joint.slerpDrive = new JointDrive()
        {
            positionSpring = armData.rotationSpring,
            positionDamper = limb.upper.joint.slerpDrive.positionDamper,
            maximumForce = limb.upper.joint.slerpDrive.maximumForce
        };
        
        limb.lower.joint.slerpDrive = new JointDrive()
        {
            positionSpring = armData.rotationSpring,
            positionDamper = limb.lower.joint.slerpDrive.positionDamper,
            maximumForce = limb.lower.joint.slerpDrive.maximumForce
        };
        
        
        
        /*float time = 0;
        float upperSpring = limb.upper.joint.slerpDrive.positionSpring;
        float lowerSpring = limb.lower.joint.slerpDrive.positionSpring;
        
        while (time < armData.smoothTime)
        {
            time += Time.deltaTime;
            float t = time / armData.smoothTime;
            limb.upper.joint.slerpDrive = new JointDrive()
            {
                positionSpring = Mathf.Lerp(upperSpring, armData.rotationSpring, t),
                positionDamper = limb.upper.joint.slerpDrive.positionDamper,
                maximumForce = limb.upper.joint.slerpDrive.maximumForce
            };
            
            limb.lower.joint.slerpDrive = new JointDrive()
            {
                positionSpring = Mathf.Lerp(lowerSpring, armData.rotationSpring, t),
                positionDamper = limb.lower.joint.slerpDrive.positionDamper,
                maximumForce = limb.lower.joint.slerpDrive.maximumForce
            };
            limb.upper.joint.targetRotation = Quaternion.Lerp(limb.upper.joint.targetRotation, Quaternion.Euler(armData.upperTargetVel), t);
            limb.lower.joint.targetRotation = Quaternion.Lerp(limb.lower.joint.targetRotation, Quaternion.Euler(armData.lowerTargetVel), t);
            yield return null;
        }*/
    }

    IEnumerator PunchThrow(LimbInfo limb, ArmData armData, Hand hand)
    {
        hand.SetIsPunching(true);
        SetArmsRot(limb, armData);
        yield return new WaitForSeconds(armData.smoothTime);
        SetArmTo(hand == leftHand ? Bipod.Left : Bipod.Right, ArmState.Idle);
    }

    public void ReleaseRightHand()
    {
        if (rightHand != null)
        {
            rightHand.Release();
            rightHand.CanGrap(false);
        }
    }
    
    public void ReleaseLeftHand()
    {
        if (leftHand != null)
        {
            leftHand.Release();
            leftHand.CanGrap(false);
        }
    }

    public void Move()
    {
        Rigidbody hips = ragdollArmature.armatureInfo.hips.rigidbody;
        Vector3 hipsForward = ragdollArmature.armatureInfo.hips.bone.forward;
        hips.AddForce(hipsForward * hipsForwardForce, ForceMode.Acceleration);
        Step();
    }
}
