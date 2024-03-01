using Snowy.Input;
using UnityEngine;

namespace Utilities
{
    public class OnlineClient : OnlineClass
    {
        [SerializeField] InputManager inputManager;

        protected override void Awake()
        {
            if (!inputManager) inputManager = GetComponent<InputManager>();
            OnInitialized += () => inputManager.Initialize();
            base.Awake();
        }
    }
}