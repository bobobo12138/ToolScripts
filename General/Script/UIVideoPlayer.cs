using DG.Tweening;
using Game.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;//记得启用Video包
using static UnityEngine.RectTransform;

public class UIVideoPlayer : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rectTransform;
    #region 可选
    //可选参数
    public bool isAutoStop = false;
    public bool isInit = false;
    public bool isPlaying;
    [Tooltip("是否使用video的尺寸，注意只有开始播放视频后（视频准备好）才知道视频尺寸")]
    public bool isUseVideoSize = false;
    #endregion

    [Header("是否需要进度条")]
    [SerializeField]
    bool isSlider = false;//是否有进度条
    [SerializeField]
    [ShowIf("isSlider", true)]
    Slider slider;//可选的进度条

    #region 必选
    /// <summary>
    /// 内部获取
    /// </summary>
    RawImage rawImage;
    RenderTexture renderTexture;

    /// <summary>
    /// 设置值
    /// </summary>
    VideoPlayer videoPlayer;
    enum CoverType
    {
        Image,
        RawImage,
        none,//无封面
    }
    [Header("封面类型")]
    [SerializeField]
    CoverType coverType = CoverType.Image;

    [SerializeField]
    [ShowIf("coverType", CoverType.Image)]
    Image coverImg;
    [SerializeField]
    [ShowIf("coverType", CoverType.RawImage)]
    RawImage coverRawImage;//rawimg封面，后续需要拓展选择枚举，选择显示某一个
    #endregion

    Rect oriRect = new Rect(0, 0, 0, 0);//原始rect
    Action afterOnPlay;//开始播放后的回调

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
    /// 设置参数
    /// </summary>
    /// <param name="url">mp4路径</param>
    /// <param name="cover">封面</param>
    public void SetData(string url, Sprite cover)
    {
        if (coverType != CoverType.Image)
        {
            Debug.LogError("传输过来的值时sprite，但您选择封面的gameobject是" + coverType);
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
            Debug.LogError("传输过来的值时texture，但您选择封面的gameobject是" + coverType);
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
                //注意oriRect设置时机，若在oriRect赋值时active==false，会导致rectTransform.rect.size为0
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

    #region 时间
    /// <summary>
    /// 获得当前时间点比例0-1
    /// </summary>
    /// <returns></returns>
    public float GetNowFrame()
    {
        if (!videoPlayer.isPrepared) return -1;

        return videoPlayer.frame / (float)videoPlayer.frameCount;
    }
    /// <summary>
    /// 获得总时间长度
    /// </summary>
    /// <returns></returns>
    public float GetMaxFrame()
    {
        if (!videoPlayer.isPrepared) return -1;

        return videoPlayer.frameCount;
    }

    /// <summary>
    /// 设置时间点比例0-1
    /// </summary>
    /// <param name="t"></param>
    public void SetFrame(float t)
    {
        if (!videoPlayer.isPrepared)
        {
            Debug.LogWarning("视频未准备好");
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
