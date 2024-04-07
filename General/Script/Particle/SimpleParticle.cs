using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParticle : MonoBehaviour
{
    public Action callBack;

    ParticleSystem ps;
    ParticleSystem.MainModule mainModule;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        mainModule = ps.main;
        mainModule.stopAction = ParticleSystemStopAction.Callback;
    }


    void OnParticleSystemStopped()
    {
        callBack?.Invoke();
    }
}
