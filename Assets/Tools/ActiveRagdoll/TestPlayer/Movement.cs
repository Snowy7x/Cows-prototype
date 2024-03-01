using _Tests.Tool.ActiveRagdoll;
using Snowy.Input;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private CharacterMotion motion;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] bool spineToCamera = true;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float xOffset = 0;
    
    void Start()
    {
        InputManager.Instance.OnAttack += OnAttack;
        InputManager.Instance.OnAim += OnAim;
    }
    
    private void OnAttack(ButtonState state)
    {
        if (state == ButtonState.Held)
        {
            motion.SetArmTo(Bipod.Left, ArmState.Punch);
        }else if (state == ButtonState.Released)
        {
            motion.SetArmTo(Bipod.Left, ArmState.Idle);
            motion.ReleaseLeftHand();
        }
    }
    
    private void OnAim(ButtonState state)
    {
        if (state == ButtonState.Held)
        {
            motion.SetArmTo(Bipod.Right, ArmState.Punch);
        }else if (state == ButtonState.Released)
        {
            motion.SetArmTo(Bipod.Right, ArmState.Idle);
            motion.ReleaseRightHand();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance == null) return;
        float h = InputManager.Instance.Horizontal;
        float v = InputManager.Instance.Vertical;
        if (h != 0 || v != 0)
        {
            Vector3 direction = cameraPivot.forward * v + cameraPivot.right * h;
            motion.LookIntoDirection(direction, rotationSpeed);
            motion.Move();
        }
        
        if (spineToCamera)
        {
            Vector3 targetRotation = Vector3.zero;
            targetRotation.x = cameraPivot.rotation.eulerAngles.x;
            Quaternion tR = Quaternion.Euler(targetRotation) * Quaternion.Euler(xOffset, 0, 0);
            motion.RagdollArmature.armatureInfo.chest.joint.targetRotation = Quaternion.Inverse(tR);
        }
    }
}
