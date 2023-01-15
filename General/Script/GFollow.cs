using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���׸���ű�
/// ��ʱֻ֧��UI
/// </summary>
public class GFollow : MonoBehaviour
{
    [SerializeField]
    RectTransform followTrans;
    RectTransform rectTransform;

    Vector2 temp;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        temp = rectTransform.anchoredPosition - followTrans.pivot;
    }


    private void LateUpdate()
    {
        rectTransform.anchoredPosition = followTrans.anchoredPosition + temp;
    }

}
