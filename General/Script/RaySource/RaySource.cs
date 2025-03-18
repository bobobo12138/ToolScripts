using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

enum RaySourceType
{
    Screen,
    Gameobject,
}

public class RaySource : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private RaySourceType raySourceType = RaySourceType.Screen;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask layerMask = ~0;
    [SerializeField] private Transform originTransform; // For Gameobject type

    [SerializeField]
    bool _isRunning;
    public bool isRunning { get => _isRunning; protected set => _isRunning = value; }

    [SerializeField]
    bool _isUpdate;
    public bool isUpdate {get => _isUpdate; protected set => _isUpdate = value; }

    private Camera mainCamera;
    private IRayTarget rayTarget;

    public Action<IRayTarget> onEnterTarget;
    public Action<IRayTarget> onExitTarget;

    void Start()
    {
        mainCamera = Camera.main;
        ValidateReferences();
    }

    void Update()
    {
        if (!isUpdate) return;

        switch (raySourceType)
        {
            case RaySourceType.Screen:
                HandleScreenRay();
                break;
            case RaySourceType.Gameobject:
                HandleGameObjectRay();
                break;
        }
    }

    public void Trigger(object data)
    {
        rayTarget?.OnRayTrigger(this, data);
    }

    public void Enable() => isRunning = true;
    public void Disable() => isRunning = false;

    private void ValidateReferences()
    {
        if (raySourceType == RaySourceType.Gameobject && originTransform == null)
        {
            Debug.LogError("Gameobject RaySource requires an Origin Transform!", this);
        }
    }

    private void HandleScreenRay()
    {
        if (mainCamera == null) return;
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        ProcessRay(ray);
    }

    private void HandleGameObjectRay()
    {
        if (originTransform == null) return;
        Ray ray = new Ray(originTransform.position, originTransform.forward);
        ProcessRay(ray);
    }

    private void ProcessRay(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            IRayTarget target = hit.collider.GetComponent<IRayTarget>();
            HandleTargetDetection(target);
        }
        else
        {
            ClearCurrentTarget();
        }
    }

    private void HandleTargetDetection(IRayTarget newTarget)
    {
        if (newTarget != null)
        {
            if (rayTarget != newTarget)
            {
                ClearCurrentTarget();
                rayTarget = newTarget;
                onEnterTarget?.Invoke(rayTarget);
            }
        }
        else
        {
            ClearCurrentTarget();
        }
    }

    private void ClearCurrentTarget()
    {
        if (rayTarget != null)
        {
            onExitTarget?.Invoke(rayTarget);
            rayTarget = null;
        }
    }


}