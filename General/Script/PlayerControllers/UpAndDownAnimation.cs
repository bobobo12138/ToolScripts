using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDownAnimation : MonoBehaviour
{
    bool _isPlaying = false;
    public bool isPlaying { get { return _isPlaying; } private set { _isPlaying = value; } }
    public float speed = 1;
    public float heightMul = 1;
    public float stopDeadzone = 1;//ֹͣ������

    float _time = 0;
    float y = 0;
    Transform startTrans;
    Vector3 end;

    private void Awake()
    {
        y = transform.position.y;
    }

    private void Update()
    {
        if (isPlaying)
        {
            _time += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, y + 1 * heightMul + Mathf.Cos((_time) * speed - Mathf.PI) * heightMul, transform.position.z);

            IsArrive();
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="startTrans"></param>
    /// <param name="end"></param>
    public void Play(Transform startTrans, Vector3 end)
    {
        if (!isPlaying)
        {
            //����playing״̬��������_time
            _time = 0;
            isPlaying = true;
        }
        this.startTrans = startTrans;
        this.end = end;
    }

    /// <summary>
    /// �Ƿ�ﵽ
    /// </summary>
    void IsArrive()
    {
        if (startTrans == null) return;
        if (transform.position.y - y > 0.1f) return;//ֻ������һ�β��ж�ֹͣ

        //������С��1��ֹͣ
        if (Vector3.Magnitude(startTrans.position - end) < stopDeadzone)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
            isPlaying = false;
        }

    }

}
