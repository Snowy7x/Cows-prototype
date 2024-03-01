using Snowy.Input;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform pivot;
    [SerializeField] float sensitivity = 10f;
    [SerializeField] Vector3 offset;
    
    float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (InputManager.Instance == null) return;
        float mouseX = InputManager.Instance.Look.x;
        float mouseY = InputManager.Instance.Look.y;
        
        xRotation -= mouseY * sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        float yRotation = mouseX * sensitivity * Time.deltaTime;
        
        Vector3 euler = pivot.rotation.eulerAngles;
        euler.x = xRotation + offset.x;
        euler.y += yRotation + offset.y;
        euler.z = offset.z;
        pivot.rotation = Quaternion.Euler(euler);
    }
}
