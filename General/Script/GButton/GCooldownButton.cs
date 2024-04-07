using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 带冷却的按钮，防止重复乱点造成的问题
/// </summary>
public class GCooldownButton : Button
{
    public bool isAni = true;//是否小动画
    [SerializeField]
    [Tooltip("未赋值的话，默认会以transform为参数")]
    Transform _trans_Ani;
    Transform trans_Ani
    {
        get
        {
            if (_trans_Ani == null)
            {
                _trans_Ani = transform;
            }
            return _trans_Ani;
        }
        set
        {
            _trans_Ani = value;
        }
    }

    [SerializeField]
    [Range(0, 1)]
    float aniScaleValue = 0.9f;//形变尺寸
    float inc_AniScaleValue;//形变增量
    const int aniStep = 5;

    [SerializeField]
    float cooldown = 1;

    Timer_Stopwatch stopwatch;

# if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        trans_Ani = transform;
    }
#endif
    protected override void Awake()
    {
        base.Awake();
        stopwatch = new Timer_Stopwatch(cooldown);
        stopwatch.Restart();

        inc_AniScaleValue = (1 - aniScaleValue) / aniStep; 
    }

    private void Update()
    {
        if (stopwatch != null)
        {
            stopwatch.Update();
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (stopwatch.isRun) return;

        stopwatch.Restart();
        Debug.LogWarning("成功");
        base.OnPointerClick(eventData);
    }



    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (!isAni) return;
        if (!interactable) return;
        StopAllCoroutines();
        StartCoroutine(Ani_Down());
    }


    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (!isAni) return;
        if (!interactable) return;
        StopAllCoroutines();
        StartCoroutine(Ani_Up());
    }



    /// <summary>
    /// 简单的协程动画
    /// </summary>
    IEnumerator Ani_Down()
    {
        int count = aniStep;
        trans_Ani.localScale = Vector3.one;
        while (count > 0)
        {
            trans_Ani.localScale -= Vector3.one * inc_AniScaleValue;
            count--;
            yield return 0;
        }
        yield return 0;
    }

    IEnumerator Ani_Up()
    {
        trans_Ani.localScale = Vector3.one * aniScaleValue;
        int count = aniStep;
        while (count > 0)
        {
            trans_Ani.localScale += Vector3.one * inc_AniScaleValue;
            count--;
            yield return 0;
        }
        yield return 0;
    }
}


namespace UnityEditor.UI
{

#if UNITY_EDITOR

    [CustomEditor(typeof(GCooldownButton), true)]
    public class GCooldownButtonEditor : UnityEditor.UI.ButtonEditor
    {
        GCooldownButton targetCom;

        SerializedProperty isAni;
        SerializedProperty _trans_Ani;
        SerializedProperty aniScaleValue;
        SerializedProperty cooldown;
        protected override void OnEnable()
        {
            base.OnEnable();
            targetCom =  target as GCooldownButton;

            isAni = serializedObject.FindProperty("isAni");
            _trans_Ani = serializedObject.FindProperty("_trans_Ani");
            aniScaleValue = serializedObject.FindProperty("aniScaleValue");
            cooldown = serializedObject.FindProperty("cooldown");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(isAni);
            EditorGUILayout.PropertyField(_trans_Ani);
            EditorGUILayout.PropertyField(aniScaleValue);
            EditorGUILayout.PropertyField(cooldown);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
#endif
}
