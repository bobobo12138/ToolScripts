using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用粒子容器，自动进行对象池管理
/// </summary>
public class GeneralParticleContainer
{
    public bool isRun = true;

    Transform parent;

    bool isUITransform = false;
    GameObject generalParticleContainerParent;


    Dictionary<string, GObjPool_WithPopList<SimpleParticle>> dic;


    public void InitSet(Transform parent, bool isUITransform = false)
    {
        this.parent = parent;
        this.isUITransform = isUITransform;
        generalParticleContainerParent = new GameObject();
        generalParticleContainerParent.name = "generalParticleContainerParent";
        generalParticleContainerParent.transform.SetParent(parent);
        if (isUITransform)
        {
            //UI坐标需要重置缩放
            generalParticleContainerParent.AddComponent<RectTransform>();
            generalParticleContainerParent.transform.localScale = Vector3.one;

        }
        generalParticleContainerParent.transform.position = Vector3.zero;
        dic = new Dictionary<string, GObjPool_WithPopList<SimpleParticle>>();
    }


    public SimpleParticle AddParticle(SimpleParticle particlePrefab, Transform particleParent)
    {
        if (!isRun) return null;
        SimpleParticle obj;
        if (dic.ContainsKey(particlePrefab.name))
        {
            obj = dic[particlePrefab.name].GetObj();
        }
        else
        {
            GameObject parent = new GameObject();

            parent.name = particlePrefab.name + "Parent";
            parent.transform.SetParent(generalParticleContainerParent.transform);
            if (isUITransform)
            {
                parent.AddComponent<RectTransform>();
                parent.transform.localScale = Vector3.one;

            }
            parent.transform.position = Vector3.zero;
            dic.Add(particlePrefab.name, new GObjPool_WithPopList<SimpleParticle>(parent.transform, particlePrefab));
            obj = dic[particlePrefab.name].GetObj();
        }
        obj.transform.SetParent(particleParent);
        obj.transform.localPosition = Vector3.zero;                                             //自动设置父级，对其坐标，完事自动回收
        obj.transform.localScale = dic[particlePrefab.name].GetPrototype().transform.localScale;//使用原型重置缩放
        obj.callBack = () => { dic[particlePrefab.name].RecycleObj(obj); };

        return obj;
    }


    public void InitSetParticle(SimpleParticle particlePrefab, int initNum = 5)
    {
        if (dic.ContainsKey(particlePrefab.name))
        {
            Debug.Log("已存在同名粒子");
        }
        else
        {
            GameObject parent = new GameObject();
            parent.name = particlePrefab.name + "Parent";
            parent.transform.SetParent(generalParticleContainerParent.transform);
            if (isUITransform)
            {
                parent.AddComponent<RectTransform>();
                parent.transform.localScale = Vector3.one;

            }
            parent.transform.position = Vector3.zero;
            dic.Add(particlePrefab.name, new GObjPool_WithPopList<SimpleParticle>(parent.transform, particlePrefab, initNum));//key名字还是使用particlePrefab.name
        }
    }
    public void RecycleParticle(string particleName, SimpleParticle simpleParticle)
    {
        if (dic.ContainsKey(particleName))
        {
            dic[particleName].RecycleObj(simpleParticle);
        }
    }
}
