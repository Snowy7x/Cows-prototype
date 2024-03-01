using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Systems.Actor
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Creature : Living
    {
        [SerializeField] private NavMeshAgent agent;
        protected bool canBeTammed;
        protected bool isAngry;
        protected bool isTammed;

        private Actor angryTarget;
        private Living tameOwner;
        
        protected override void Awake()
        {
            base.Awake();
            if (!agent) agent = GetComponent<NavMeshAgent>();
        }

        public void GoTo(Vector3 pos)
        {
            agent.isStopped = true;
            StopCoroutine(nameof(GoToRoutine));
            StartCoroutine(GoToRoutine(pos));
        }
        
        protected IEnumerator GoToRoutine(Vector3 pos, float threshold = 0.1f)
        {
            agent.SetDestination(pos);
            agent.isStopped = false;
            agent.stoppingDistance = threshold;
            while (agent.remainingDistance > threshold)
            {
                yield return null;
            }

            agent.isStopped = true;
        }
        
        public void Tame(Living owner)
        {
            isTammed = true;
            tameOwner = owner;
        }
        
        public bool IsTammed => isTammed;
    }
}