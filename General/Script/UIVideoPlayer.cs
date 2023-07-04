using DG.Tweening;
using Game.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;//�ǵ�����Video��
using static UnityEngine.RectTransform;

public class UIVideoPlayer : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rectTransform;
    #region ��ѡ
    //��ѡ����
    public bool isAutoStop = false;
    public bool isInit = false;
    public bool isPlaying;
    [Tooltip("�Ƿ�ʹ��video�ĳߴ磬ע��ֻ�п�ʼ������Ƶ����Ƶ׼���ã���֪����Ƶ�ߴ�")]
    public bool isUseVideoSize = false;
    #endregion

    [Header("�Ƿ���Ҫ������")]
    [SerializeField]
    bool isSlider = false;//�Ƿ��н�����
    [SerializeField]
    [ShowIf("isSlider", true)]
    Slider slider;//��ѡ�Ľ�����

    #region ��ѡ
    /// <summary>
    /// �ڲ���ȡ
    /// </summary>
    RawImage rawImage;
    RenderTexture renderTexture;

    /// <summary>
    /// ����ֵ
    /// </summary>
    VideoPlayer videoPlayer;
    enum CoverType
    {
        Image,
        RawImage,
        none,//�޷���
    }
    [Header("��������")]
    [SerializeField]
    CoverType coverType = CoverType.Image;

    [SerializeField]
    [ShowIf("coverType", CoverType.Image)]
    Image coverImg;
    [SerializeField]
    [ShowIf("coverType", CoverType.RawImage)]
    RawImage coverRawImage;//rawimg���棬������Ҫ��չѡ��ö�٣�ѡ����ʾĳһ��
    #endregion

    Rect oriRect = new Rect(0, 0, 0, 0);//ԭʼrect
    Action afterOnPlay;//��ʼ���ź�Ļص�

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        rawImage = GetComponent<RawImage>();
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.prepareCompleted += OnVideoPrepareCompleted;
        isInit = true;
    }
    private void Update()
    {
        UpdateSlider();
    }

    /// <summary>
    /// ���ò���
    /// </summary>
    /// <param name="url">mp4·��</param>
    /// <param name="cover">����</param>
    public void SetData(string url, Sprite cover)
    {
        if (coverType != CoverType.Image)
        {
            Debug.LogError("���������ֵʱsprite������ѡ������gameobject��" + coverType);
            return;
        }

        videoPlayer.url = url;
        if (cover != null)
            coverImg.sprite = cover;
    }
    public void SetData(string url, Texture cover)
    {
        if (coverType != CoverType.RawImage)
        {
            Debug.LogError("���������ֵʱtexture������ѡ������gameobject��" + coverType);
            return;
        }
        videoPlayer.url = url;
        if (cover != null)
            coverRawImage.texture = cover;
    }
    public void SetData(string url)
    {
        videoPlayer.url = url;
    }


    void OnVideoPrepareCompleted(VideoPlayer source)
    {
        if (!isPlaying) return;
        if (renderTexture != null) RenderTexture.ReleaseTemporary(renderTexture);
        renderTexture = RenderTexture.GetTemporary((int)source.width, (int)source.height, 0);
        videoPlayer.targetTexture = renderTexture;
        rawImage.texture = renderTexture;

        if (isUseVideoSize)
        {
            if (oriRect.size == Vector2.zero)
            {
                //ע��oriRect����ʱ��������oriRect��ֵʱactive==false���ᵼ��rectTransform.rect.sizeΪ0
                Canvas.ForceUpdateCanvases();
                oriRect = rectTransform.rect;
            }
            rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, (int)source.width);
            rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, (int)source.height);
            Utils.MaxTiled(rectTransform, rectTransform.rect.size, oriRect.size);
        }

        coverImg.DOKill();
        coverImg.gameObject.SetActive(false);
        videoPlayer.Play();
        afterOnPlay?.Invoke();

    }


    public void Play(Action afterPlay = null)
    {
        if (videoPlayer == null) return;
        if (isPlaying) return;

        isPlaying = true;
        videoPlayer.Prepare();
        this.afterOnPlay = afterPlay;
    }

    public void Stop()
    {
        if (videoPlayer == null) return;
        if (!isPlaying) return;

        isPlaying = false;
        videoPlayer.Stop();

        coverImg.color = new Color(1, 1, 1, 0);
        coverImg.gameObject.SetActive(true);
        coverImg.DOFade(1, 1f).OnComplete(() => { });
    }

    public bool isPrepared()
    {
        return videoPlayer.isPrepared;
    }

    public string GetUrl()
    {
        return videoPlayer.url;
    }

    #region ʱ��
    /// <summary>
    /// ��õ�ǰʱ������0-1
    /// </summary>
    /// <returns></returns>
    public float GetNowFrame()
    {
        if (!videoPlayer.isPrepared) return -1;

        return videoPlayer.frame / (float)videoPlayer.frameCount;
    }
    /// <summary>
    /// �����ʱ�䳤��
    /// </summary>
    /// <returns></returns>
    public float GetMaxFrame()
    {
        if (!videoPlayer.isPrepared) return -1;

        return videoPlayer.frameCount;
    }

    /// <summary>
    /// ����ʱ������0-1
    /// </summary>
    /// <param name="t"></param>
    public void SetFrame(float t)
    {
        if (!videoPlayer.isPrepared)
        {
            Debug.LogWarning("��Ƶδ׼����");
            return;
        }

        videoPlayer.frame = (long)(t * videoPlayer.frameCount);
    }


    void UpdateSlider()
    {
        if (slider == null) return;
        if (!videoPlayer.isPrepared) return;

        slider.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
    }

    #endregion
}
