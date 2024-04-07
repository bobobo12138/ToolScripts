using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 通用图片装载器
/// </summary>
public class GImgLoader : MonoBehaviour
{
    public Action onDelete;//当删除时触发

    [SerializeField]
    RawImage rawImage;

    [SerializeField]
    [Header("是否自动回收(若赋值时rawImage有texture则回收上一个texture)")]
    bool isAutoRecycle = true;

    [SerializeField][BoxGroup("isBtn_Cancel")] bool isBtn_Cancel = true;
    [ShowIf("isBtn_Cancel")]
    [SerializeField][BoxGroup("isBtn_Cancel")] Button btn_Cancel;


    [SerializeField][BoxGroup("isCustomObject")] bool isCustomObject = true;
    [Header("拥有/未拥有图片显示的obj，没有则保持为空")]
    [EnableIf("isCustomObject", true)]
    [SerializeField][BoxGroup("isCustomObject")] GameObject gameobject_Textured;
    [EnableIf("isCustomObject", true)]
    [SerializeField][BoxGroup("isCustomObject")] GameObject gameobject_Textureless;


    [Header("是否自适应")]
    [Tooltip("rawImage的texture是否自适应，开启后会比例缩放texture，使其铺满rawImage")]
    [SerializeField][BoxGroup("自适应")] bool isTextureAdaptive = false;
    [ShowIf("isTextureAdaptive", true)]
    [SerializeField][BoxGroup("自适应")] RectTransform textureParent;

    private void Awake()
    {
        if (isBtn_Cancel)
            btn_Cancel.onClick.AddListener(ClickBtn_Cancel);
    }

    /// <summary>
    /// 是否拥有图片
    /// </summary>
    /// <returns></returns>
    public bool IsHaveImg()
    {
        if (rawImage.texture != null) return true;
        return false;
    }

    /// <summary>
    /// 设置图片
    /// 会根据texture2D是否为null来决定是否显示
    /// </summary>
    /// <param name="texture2D"></param>
    public void SetImg(Texture2D texture2D)
    {
        if (texture2D != null)
        {
            //防止删除相同的图片
            if (rawImage.texture == texture2D) return;
        }

        if (isAutoRecycle)
        {
            if (rawImage.texture != null) Destroy(rawImage.texture);
        }
        var isNull = texture2D == null;

        rawImage.texture = texture2D;
        if (gameobject_Textured != null) gameobject_Textured.SetActive(!isNull);
        if (gameobject_Textureless != null) gameobject_Textureless.SetActive(isNull);

        if (isTextureAdaptive)
        {
            if (rawImage.texture != null)
                Utils.MaxTiled(rawImage.rectTransform, new Vector2(rawImage.texture.width, rawImage.texture.height), textureParent.rect.size);
        }
    }

    public void Recycle()
    {
        Destroy(rawImage.texture);
        SetImg(null);
    }

    public Texture2D GetImg()
    {
        return rawImage.texture as Texture2D;
    }

    void ClickBtn_Cancel()
    {
        Recycle();
        onDelete?.Invoke();
    }
}
