using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ͨ�÷���״̬��_����ջ�嵥
/// ��ʵ�⣬�����Ǽ̳���дԭʼ״̬��
/// </summary>
public class GObjPool_WithOutlist<T> where T : MonoBehaviour
{
    Stack<T> pool = new Stack<T>();
    List<T> outlist = new List<T>();//��pool�嵥
    Transform parent;
    T prototype;

    /// <summary>
    /// �����������Ԥ��������
    /// </summary>
    public GObjPool_WithOutlist(Transform _parent, T _spawnObj, int num = 0)
    {
        parent = _parent;
        prototype = GameObject.Instantiate(_spawnObj, parent);//Ԥ�ȿ�¡һ����Ϊԭ�ͣ������ⲿ�����޸�
        prototype.name = prototype.name + "prototype";
        for (int i = 0; i < num; i++)
        {
            var v = GameObject.Instantiate(prototype, parent);
            pool.Push(v);
            v.gameObject.SetActive(false);
        }
        prototype.gameObject.SetActive(false);//���Ž�ԭ��SetActive(false)��������awake��������
    }
    /// <summary>
    /// ��ȡһ������
    /// </summary>
    /// <returns></returns>
    public T GetObj()
    {
        if (pool.Count == 0)
        {
            var v = GameObject.Instantiate(prototype, parent);

            //���г�ʼ�������̳���IObjInit�ӿڵĻ�
            if (v is IObjInit)
            {
                var temp = v as IObjInit;
                temp.Init();
            }

            v.gameObject.SetActive(true);
            Expand_Get(v);
            outlist.Add(v);
            return v;
        }

        var obj = pool.Pop();
        obj.gameObject.SetActive(true);
        Expand_Get(obj);
        outlist.Add(obj);
        return obj;
    }

    /// <summary>
    /// ����һ������
    /// </summary>
    /// <param name="obj"></param>
    public void RecycleObj(T obj)
    {
        if (obj == prototype) return;//���ܻ���ԭ��
        if (pool.Contains(obj)) return;
        if (obj.transform.parent != parent)
            obj.transform.SetParent(parent);
        obj.gameObject.SetActive(false);
        Expand_RecycleObj(obj);
        outlist.Remove(obj);
        pool.Push(obj);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="num"></param>
    public void Produce(int num = 10)
    {
        for (int i = 0; i < num; i++)
        {
            var v = GameObject.Instantiate(prototype, parent);
            v.gameObject.SetActive(false);
            pool.Push(v);
        }
    }
    /// <summary>
    /// ��������
    /// </summary>
    public void CleanPool()
    {
        if (parent.childCount <= 1) return;

        pool.Clear();
        outlist.Clear();
        for (int i = 1; i < parent.childCount; i++)//������ɾԭ��
        {
            GameObject.Destroy(parent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// ��ÿ�¡��ԭ�ͣ�������ԭ�ͽ����޸�
    /// </summary>
    /// <returns></returns>
    public T GetPrototype()
    {
        if (prototype != null)
        {
            return prototype;
        }
        else
        {
            //Debug.LogWarning("cant get the prototype,is objpool init?(constructor)");
            Debug.LogWarning("�޷����ԭ�ͣ�����س�ʼ����");
            return null;
        }

    }

    public List<T> GetOutlist()
    {
        return outlist;
    }

    /// <summary>
    /// GetObje������չ�����޷��������󣬼̳б��࣬��д�����Getʱ�Զ�����
    /// </summary>
    /// <param name="v"></param>
    protected virtual void Expand_Get(T v)
    {

    }


    /// <summary>
    /// RecycleObj������չ�����޷��������󣬼̳б��࣬��д�����RecycleObjʱ�Զ�����
    /// </summary>
    /// <param name="v"></param>
    protected virtual void Expand_RecycleObj(T v)
    {

    }

}
