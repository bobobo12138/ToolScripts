using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
namespace General
{
    public class RayController : MonoBehaviour
    {
        /// 初始化参数
        [HideInInspector]
        public bool awakeInit = true;

        bool _isInit = false;
        public bool isInit { get { return _isInit; } private set { _isInit = value; } }

        [SerializeField]
        [Header("主要参数")]
        public int maxDistance = 100;
        public LayerMask layerMask;
        public Action<Vector3> onClick;

        RayInputSystem rayInputSystem;

        private void Awake()
        {
            if (awakeInit) Init();
        }
        public void Init()
        {
            if (isInit) return;
            rayInputSystem = new RayInputSystem();
            rayInputSystem.Enable();
            rayInputSystem.Player.RayButton.performed += OnPerformedRay;
            isInit = true;
        }



        public void Enable()
        {
            rayInputSystem.Enable();
        }


        public void Disable()
        {
            rayInputSystem.Disable();
        }


        /// <summary>
        /// rayInputSystem.raybutton回调坐标时
        /// </summary>
        /// <param name="data"></param>
        void OnPerformedRay(CallbackContext data)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                Debug.Log(hit.collider.name);
                onClick(hit.point);
            }
        }
    }

}
