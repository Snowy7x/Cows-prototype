using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Systems.IK.Base
{
    public class BaseIK : MonoBehaviour
    {
        public List<ChainIK> chains;
        public LookChain[] lookChains;
        public FollowTarget[] followTargets;
        
        [Header("Debug")] public bool debug = false;
        public event Action OnIKResolved;
        
        void Awake()
        {
            foreach (var chain in chains)
            {
                chain.Init();
            }
            
            
            foreach (var chain in followTargets)
            {
                chain.Init();
            }
            
            foreach (var chain in lookChains)
            {
                chain.Init();
            }
        }

        private void LateUpdate()
        {
            foreach (var chain in lookChains)
            {
                chain.Resolve();
            }

            foreach (var chain in followTargets)
            {
                chain.Resolve();
            }
            
            OnIKResolved?.Invoke();

            foreach (var chain in chains)
            {
                chain.ResolveIK();
            }
            
        }
    }
}