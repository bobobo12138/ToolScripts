using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemController : MonoBehaviour
{
    public static InputSystemController instance;

    ///在此生命出要用到的InputActions 

    public PlayerControls playerControls;


    private void Awake()
    {
        instance = this;


        playerControls = new PlayerControls();
    }
    public void Loadebinding()
    {
        ///读取

    }

    public void Savebinding()
    {
        var isEnabled = playerControls.asset.enabled;
        var json = playerControls.SaveBindingOverridesAsJson();

        ///保存
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

                ///回复到原来的启用状态
                if (isEnabled)
                {
                    _inputAction.Enable();
                }
            })
            .Start();
    }
}
