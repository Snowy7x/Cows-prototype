using UnityEngine;

namespace Utilities
{
    public class OnlineClass : MonoBehaviour
    {
        public delegate void Initialized();
        public static event Initialized OnInitialized;
        [SerializeField] protected bool isOnline;
        
        protected virtual void Awake()
        {
            if (!isOnline)
                OnInitialized?.Invoke();
            else
            {
                // TODO: Check if owner; if not, destroy.
            }
        }
    }
}