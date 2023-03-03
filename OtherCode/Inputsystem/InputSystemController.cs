using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystemController : MonoBehaviour
{
    public static InputSystemController instance;
    public PlayerControls playerControls;


    private void Awake()
    {
        instance = this;


        playerControls = new PlayerControls();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
