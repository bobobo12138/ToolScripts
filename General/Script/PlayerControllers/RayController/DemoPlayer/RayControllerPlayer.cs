using UnityEngine;
using UnityEngine.AI;
namespace General
{
    public class RayControllerPlayer : MonoBehaviour
    {
        [SerializeField]
        RayController rayController;
        [SerializeField]
        NavMeshAgent navMeshAgent;
        [SerializeField]
        UpAndDownAnimation upAndDownAnimation;

        private void Awake()
        {
            rayController.Init();
            rayController.onClick += SetPosition;
        }

        private void Update()
        {

        }

        void SetPosition(Vector3 pos)
        {
            navMeshAgent.SetDestination(pos);
            upAndDownAnimation.Play(transform, pos);
        }
    }
}