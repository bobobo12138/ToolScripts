using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemController : MonoBehaviour
{
    public static InputSystemController instance;

    ///�ڴ�������Ҫ�õ���InputActions 

    public PlayerControls playerControls;


    private void Awake()
    {
        instance = this;


        playerControls = new PlayerControls();
    }
    public void Loadebinding()
    {
        ///��ȡ

    }

    public void Savebinding()
    {
        var isEnabled = playerControls.asset.enabled;
        var json = playerControls.SaveBindingOverridesAsJson();

        ///����
    }

    public void Rebinding(InputAction _inputAction)
    {
        //Debug.Log(Keyboard.current.ToString()+Keyboard.current.enterKey.keyCode.ToString());

        bool isEnabled = _inputAction.enabled;
        _inputAction.Disable();
        _inputAction.PerformInteractiveRebinding()
            .WithControlsExcluding("/Keyboard/a")
            .OnComplete((callback) =>
            {
                Debug.Log(callback);
                callback.Dispose();
                _inputAction.Enable();

                ///�ظ���ԭ��������״̬
                if (isEnabled)
                {
                    _inputAction.Enable();
                }
            })
            .Start();
    }
}
