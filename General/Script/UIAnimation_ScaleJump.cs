using DG.Tweening;
using Game.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RectTransform;

/// <summary>
/// �����������Ծ����
/// </summary>
[ExecuteInEditMode]
public class UIAnimation_ScaleJump : MonoBehaviour
{
    public Action OnFinish;

    [Header("����")]
    [SerializeField]
    RectTransform circleMask;
    [SerializeField]
    RawImage rawImage;
    [SerializeField]
    float time_Scale;
    [SerializeField]
    float scaleMinSize;
    [SerializeField]
    GameObject showAsync;
    [SerializeField]
    CanvasGroup canvasGroup;
    Vector2 rectSize;
    Camera camera;


    [Header("������")]
    [SerializeField]
    List<Transform> pointTrans = new List<Transform>();
    [SerializeField]
    float time_Beziers;
    [SerializeField]
    Transform moveTrans;
    List<Vector3> pointVectors = new List<Vector3>();


    /// ����
    Sequence sequence;
    RenderTexture renderTexture;
    RectTransform rectTransform;
    //Vector3 pos;

    /// <summary>
    /// ����start�г�ʼ��
    /// </summary>
    public void StartInit()
    {
        rectTransform = GetComponent<RectTransform>();
        //��ʼ������
        camera = UIManager.Instance.uiCamera.currentCamera;
        rectSize = new Vector2(circleMask.rect.width, circleMask.rect.height);
        //��ʼ���������ڵ�
        foreach (var v in pointTrans)
        {
            pointVectors.Add(v.position);
        }
    }

    public void ResetState()
    {
        sequence.Kill();
        circleMask.SetSizeWithCurrentAnchors(Axis.Horizontal, rectSize.x);
        circleMask.SetSizeWithCurrentAnchors(Axis.Vertical, rectSize.y);

        rawImage.rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, rectTransform.rect.width);//�������Ǹ�Ϊshader
        rawImage.rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, rectTransform.rect.height);
        circleMask.anchoredPosition = Vector2.zero;
        showAsync.gameObject.SetActive(false);
        canvasGroup.alpha = 1;
    }

    public void Play()
    {
        ResetState();
        if (renderTexture != null)
        {
            RenderTexture.ReleaseTemporary(renderTexture);
        }
        ///��ȡ��ǰ���������Ⱦ
        rawImage.gameObject.SetActive(false);
        renderTexture = RenderTexture.GetTemporary((int)(rectTransform.rect.width * 0.9f), (int)(rectTransform.rect.height * 0.9f));
        camera.targetTexture = renderTexture;
        camera.Render();
        rawImage.texture = renderTexture;
        camera.targetTexture = null;
        rawImage.gameObject.SetActive(true);
        ///ִ�ж���

        sequence = DOTween.Sequence();
        sequence.Insert(0, DOTween.To(() => rectSize.x, (x) =>
        {
            circleMask.SetSizeWithCurrentAnchors(Axis.Horizontal, x);
            circleMask.SetSizeWithCurrentAnchors(Axis.Vertical, x);

        }, scaleMinSize, time_Scale).OnComplete(() =>
        {
            showAsync.gameObject.SetActive(true);
        }));
        sequence.Insert(time_Scale / 2, DOTween.To(() => 0.0f, (t) =>
          {
              moveTrans.position = Beziers(pointVectors, t);
          }, 1, time_Beziers));
        sequence.Insert(time_Scale / 1.25f, DOTween.To(() => 1.0f, (alpha) =>
        {
            canvasGroup.alpha = alpha;
        }, 0, time_Beziers * 0.95f));
        sequence.Play();
        sequence.OnComplete(() =>
        {
            OnFinish?.Invoke();
            RenderTexture.ReleaseTemporary(renderTexture);
        });
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        pointVectors = new List<Vector3>();
        foreach (var v in pointTrans)
        {
            pointVectors.Add(v.position);
        }
        List<Vector3> points = new List<Vector3>();
        for (float i = 0; i < 50; i++)
        {
            points.Add(Beziers(pointVectors, i / 50));
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }

    Vector3 Beziers(List<Vector3> ps, float t)
    {
        if (ps.Count < 2) return ps[0];
        List<Vector3> temps = new List<Vector3>();
        for (int i = 0; i < ps.Count - 1; i++)
        {
            temps.Add((1 - t) * ps[i] + t * ps[i + 1]);
            //temps.Add((ps[i + 1] - ps[i]) * (t / 1));
        }
        return Beziers(temps, t);
    }



    //public void Play_CreateTexture2D()
    //{
    //    //tempRenderTexture = RenderTexture.GetTemporary(camera.pixelWidth, camera.pixelHeight);
    //    //Graphics.Blit(camera.activeTexture, tempRenderTexture);
    //    //rawImage.texture = tempRenderTexture;
    //    //RenderTexture.ReleaseTemporary(tempRenderTexture);

    //    RenderTexture renderTexture = RenderTexture.GetTemporary(camera.pixelWidth, camera.pixelHeight);
    //    camera.targetTexture = renderTexture;
    //    camera.Render();
    //    // ʹ�� Graphics API �� RenderTexture ���Ƶ���һ�� Texture2D ������
    //    Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
    //    texture.name = "cameraPrint";
    //    RenderTexture.active = renderTexture;
    //    texture.ReadPixels(new Rect(0, 0, camera.pixelWidth, camera.pixelHeight), 0, 0);
    //    texture.Apply();
    //    RenderTexture.active = null;

    //    // �� Texture2D ����ֵ�� RawImage ����� texture ���ԣ����� RawImage ����ʾ���ͼ��ĸ���Ʒ
    //    rawImage.texture = texture;

    //    // ������� targetTexture ��������Ϊ null���Իָ���Ĭ����Ϊ
    //    camera.targetTexture = null;
    //}
}
