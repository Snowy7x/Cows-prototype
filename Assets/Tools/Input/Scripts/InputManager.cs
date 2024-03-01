using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Snowy.Input
{
    public enum InputVersion { OldInput, NewInput }
    public enum ButtonState {None, Pressed, Held, Released }

    public class InputManager : MonoBehaviour
    {
        
        public delegate void OnInitialized(InputManager inputManager);
        public static event OnInitialized onInitialized;
        
        public static InputManager Instance { get; private set; }
        [SerializeField] private InputVersion inputVersion;
        [SerializeField] private PlayerInput playerInput;
    
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        
        public Vector2 Look { get; private set; }
        
        public ButtonState Jump { get; private set; }
        public ButtonState Sprint { get; private set; }
        public ButtonState Crouch { get; private set; }
        public ButtonState Interact { get; private set; }
        public ButtonState Attack { get; private set; }
        public ButtonState Aim { get; private set; }
        
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;
        public event Action<ButtonState> OnJump;
        public event Action<ButtonState> OnSprint;
        public event Action<ButtonState> OnCrouch;
        public event Action<ButtonState> OnInteract;
        public event Action<ButtonState> OnAttack;
        public event Action<ButtonState> OnAim;
        public event Action<float> OnMouseWheel; 
        
        private bool _jump;
        private bool _sprint;
        private bool _crouch;
        private bool _interact;
        private bool _attack;
        private bool _aim;

        private void Start()
        {
            switch (inputVersion)
            {
                case InputVersion.OldInput:
                    break;
                case InputVersion.NewInput:
                    if (playerInput == null)
                        playerInput = GetComponent<PlayerInput>();
                    if (playerInput == null)
                        Debug.LogError("PlayerInput component not found on " + gameObject.name);
                    else
                    {
                        playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
                        playerInput.onActionTriggered += OnActionTriggered;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Initialize()
        {
            Instance = this;
            onInitialized?.Invoke(this);
        }

        private void OnActionTriggered(InputAction.CallbackContext obj)
        {
            string actionName = obj.action.name;
            switch (actionName)
            {
                case "Move":
                    Horizontal = obj.ReadValue<Vector2>().x;
                    Vertical = obj.ReadValue<Vector2>().y;
                    break;
                case "Look":
                    Look = obj.ReadValue<Vector2>();
                    break;
                case "Jump":
                    _jump = obj.ReadValue<float>() > 0;
                    break;
                case "Sprint":
                    _sprint = obj.ReadValue<float>() > 0;
                    break;
                case "Crouch":
                    _crouch = obj.ReadValue<float>() > 0;
                    break;
                case "Interact":
                    _interact = obj.ReadValue<float>() > 0;
                    break;
                case "Attack":
                    _attack = obj.ReadValue<float>() > 0;
                    break;
                case "Aim":
                    _aim = obj.ReadValue<float>() > 0;
                    break;
                case "MouseWheel":
                    if (obj.ReadValue<float>() > 0)
                        OnMouseWheel?.Invoke(1);
                    else if (obj.ReadValue<float>() < 0)
                        OnMouseWheel?.Invoke(-1);
                    break;
                    
            }
        }

        private void Update()
        {
            if (inputVersion == InputVersion.OldInput)
            {
                OldInput();
            }
            
            UpdateEvents();
        }

        private void OldInput()
        {
            Horizontal = UnityEngine.Input.GetAxis("Horizontal");
            Vertical = UnityEngine.Input.GetAxis("Vertical");
            
            Look = new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
            
            _jump = UnityEngine.Input.GetButton("Jump");
            _sprint = UnityEngine.Input.GetButton("Sprint");
            _crouch = UnityEngine.Input.GetButton("Crouch");
            _interact = UnityEngine.Input.GetButton("Interact");
            _attack = UnityEngine.Input.GetButton("Attack");
            _aim = UnityEngine.Input.GetButton("Aim");
            
            if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0)
                OnMouseWheel?.Invoke(1);
            else if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0)
                OnMouseWheel?.Invoke(-1);
        }

        void UpdateEvents()
        {
            Vector2 move = new Vector2(Horizontal, Vertical);
            OnMove?.Invoke(move);
            
            Vector2 look = Look;
            OnLook?.Invoke(look);

            if (_jump && Jump == ButtonState.None)
            {
                Jump = ButtonState.Pressed;
                OnJump?.Invoke(Jump);
            }else if (_jump && Jump == ButtonState.Pressed) {
                Jump = ButtonState.Held;
                OnJump?.Invoke(Jump);
            }else if (!_jump && Jump == ButtonState.Held)
            {
                Jump = ButtonState.Released;
                OnJump?.Invoke(Jump);
            }
            else if (_jump && Jump == ButtonState.Held) {
                Jump = ButtonState.Held;
                OnJump?.Invoke(Jump);
            }else if (!_jump && Jump == ButtonState.Released) {
                Jump = ButtonState.None;
                OnJump?.Invoke(Jump);
            }
            
            if (_sprint && Sprint == ButtonState.None)
            {
                Sprint = ButtonState.Pressed;
                OnSprint?.Invoke(Sprint);
            }else if (_sprint && Sprint == ButtonState.Pressed) {
                Sprint = ButtonState.Held;
                OnSprint?.Invoke(Sprint);
            }else if (!_sprint && Sprint == ButtonState.Held) {
                Sprint = ButtonState.Released;
                OnSprint?.Invoke(Sprint);
            }else if (_sprint && Sprint == ButtonState.Held) {
                Sprint = ButtonState.Held;
                OnSprint?.Invoke(Sprint);
            }
            else if (!_sprint && Sprint == ButtonState.Released) {
                Sprint = ButtonState.None;
                OnSprint?.Invoke(Sprint);
            }
            
            if (_crouch && Crouch == ButtonState.None)
            {
                Crouch = ButtonState.Pressed;
                OnCrouch?.Invoke(Crouch);
            }else if (_crouch && Crouch == ButtonState.Pressed) {
                Crouch = ButtonState.Held;
                OnCrouch?.Invoke(Crouch);
            }else if (!_crouch && Crouch == ButtonState.Held)
            {
                Crouch = ButtonState.Released;
                OnCrouch?.Invoke(Crouch);
            } else if (_crouch && Crouch == ButtonState.Held)
            {
                Crouch = ButtonState.Held;
                OnCrouch?.Invoke(Crouch);
            }else if (!_crouch && Crouch == ButtonState.Released) {
                Crouch = ButtonState.None;
                OnCrouch?.Invoke(Crouch);
            }
            
            if (_interact && Interact == ButtonState.None)
            {
                Interact = ButtonState.Pressed;
                OnInteract?.Invoke(Interact);
            }else if (_interact && Interact == ButtonState.Pressed) {
                Interact = ButtonState.Held;
                OnInteract?.Invoke(Interact);
            }else if (!_interact && Interact == ButtonState.Held)
            {
                Interact = ButtonState.Released;
                OnInteract?.Invoke(Interact);
            }else if (_interact && Interact == ButtonState.Held) {
                Interact = ButtonState.Held;
                OnInteract?.Invoke(Interact);
            }else if (!_interact && Interact == ButtonState.Released) {
                Interact = ButtonState.None;
                OnInteract?.Invoke(Interact);
            }
            
            if (_attack && Attack == ButtonState.None)
            {
                Attack = ButtonState.Pressed;
                OnAttack?.Invoke(Attack);
            }else if (_attack && Attack == ButtonState.Pressed) {
                Attack = ButtonState.Held;
                OnAttack?.Invoke(Attack);
            }else if (!_attack && Attack == ButtonState.Held)
            {
                Attack = ButtonState.Released;
                OnAttack?.Invoke(Attack);
            } else if (_attack && Attack == ButtonState.Held)
            {
                Attack = ButtonState.Held;
                OnAttack?.Invoke(Attack);
            }else if (!_attack && Attack == ButtonState.Released) {
                Attack = ButtonState.None;
                OnAttack?.Invoke(Attack);
            }
            
            if (_aim && Aim == ButtonState.None)
            {
                Aim = ButtonState.Pressed;
                OnAim?.Invoke(Aim);
            }else if (_aim && Aim == ButtonState.Pressed) {
                Aim = ButtonState.Held;
                OnAim?.Invoke(Aim);
            }else if (!_aim && Aim == ButtonState.Held)
            {
                Aim = ButtonState.Released;
                OnAim?.Invoke(Aim);
            } else if (_aim && Aim == ButtonState.Held)
            {
                Aim = ButtonState.Held;
                OnAim?.Invoke(Aim);
            }else if (!_aim && Aim == ButtonState.Released) {
                Aim = ButtonState.None;
                OnAim?.Invoke(Aim);
            }
        }
    }
}