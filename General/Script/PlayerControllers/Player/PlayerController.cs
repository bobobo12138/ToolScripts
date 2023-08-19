using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
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
