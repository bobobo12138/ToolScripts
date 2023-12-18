using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// AnimatorOver完毕后回调管理
/// </summary>
public class AnimatorOverCallMgr: UnitySingleton<AnimatorOverCallMgr>
{
    protected override void OnInit()
    {
        
    }
    private Dictionary<string, AnimatorCallElement> callManager = new Dictionary<string, AnimatorCallElement>();

    internal void HandleEvent(string name)
    {
        if (callManager.ContainsKey(name))
        {
            callManager[name].DoOver();
        }
    }

    internal void RemoveHandleEvent(string name)
    {
        if (callManager.ContainsKey(name))
        {
            callManager.Remove(name);
        }
    }

    internal void AddListener(Animator animator, System.Action call)
    {
        string c_InstanceID = animator.gameObject.GetInstanceID() + "";
        if (!callManager.ContainsKey(c_InstanceID))
        {
            callManager.Add(c_InstanceID, new AnimatorCallElement(animator));
        }
        CheckEvent(animator, c_InstanceID);

        callManager[c_InstanceID].onOver -= call;
        callManager[c_InstanceID].onOver += call;
    }

    /// <summary>
    /// 添加一个监听
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="call">返回值是(Layer,StateNameHash)</param>
    public void AddListener(Animator animator, System.Action<int, int> call)
    {
        string c_InstanceID = animator.gameObject.GetInstanceID() + "";
        if (!callManager.ContainsKey(c_InstanceID))
        {
            callManager.Add(c_InstanceID, new AnimatorCallElement(animator));
        }
        CheckEvent(animator, c_InstanceID);

        callManager[c_InstanceID].onOverByStateNameHash -= call;
        callManager[c_InstanceID].onOverByStateNameHash += call;
    }

    /// <summary>
    /// 移除一个监听
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="call"></param>
    public void RemoveListener(Animator animator, System.Action<int, int> call)
    {
        string c_InstanceID = animator.gameObject.GetInstanceID() + "";
        if (callManager.ContainsKey(c_InstanceID))
        {
            callManager[c_InstanceID].onOverByStateNameHash -= call;
        }
    }

    AnimationClip[] c_clips;
    AnimationEvent[] c_events;
    /// <summary>
    /// 检查是否有Event
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="c_InstanceID"></param>
    private void CheckEvent(Animator animator, string c_InstanceID)
    {
        if (animator.GetComponent<AnimatorOverCall>() == null)
        {
            AnimatorOverCall animatorOverCall = animator.gameObject.AddComponent<AnimatorOverCall>();
            animatorOverCall.SetInstanceID(c_InstanceID);
            c_clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0, length = c_clips.Length; i < length; i++)
            {
                if (c_clips[i] == null)
                {
                    continue;
                }
                c_events = c_clips[i].events;
                for (int j = 0; j < c_events.Length; j++)
                {
                    if (c_events[j] == null)
                    {
                        continue;
                    }
                    if (c_events[j].functionName == "OverCall")
                    {
                        return;
                    }
                }
                c_clips[i].AddEvent(new AnimationEvent() { functionName = "OverCall", time = c_clips[i].length });
            }
        }
    }



    #region 内部类
    private class AnimatorCallElement
    {
        public AnimatorCallElement(Animator animator)
        {
            this.animator = animator;
        }

        private Animator animator;

        internal System.Action onOver;

        internal event System.Action<int, int> onOverByStateNameHash;

        internal void DoOver()
        {
            if (onOverByStateNameHash != null)
            {
                for (int i = 0, length = animator.layerCount; i < length; i++)
                {
                    onOverByStateNameHash.Invoke(i, animator.GetCurrentAnimatorStateInfo(i).shortNameHash);
                }
            }
            onOver?.Invoke();
            onOver = null;
        }
    }
    #endregion
}

/// <summary>
/// 对Animator扩展
/// </summary>
public static class AnimatorExtend
{
    public static void Play(this Animator animator, string name, System.Action action, int layout = 0)
    {
        if (animator.HasState(layout, Animator.StringToHash(name)))
        {
            animator.Play(name, layout);
            AnimatorOverCallMgr.Instance.AddListener(animator, action);
        }
        else
        {
            action?.Invoke();
            AprilDebug.LogError("没有：" + name + "State");
        }
    }

    /// <summary>
    /// 添加完毕监听
    /// </summary>
    /// <param name="animater"></param>
    /// <param name="action">返回值是(Layer,StateNameHash)</param>
    public static void AddOverCall(this Animator animater, System.Action<int, int> action)
    {
        AnimatorOverCallMgr.Instance.AddListener(animater, action);
    }

    /// <summary>
    /// 移除完毕监听
    /// </summary>
    /// <param name="animater"></param>
    /// <param name="action"></param>
    public static void RemoveOverCall(this Animator animater, System.Action<int, int> action)
    {
        AnimatorOverCallMgr.Instance.RemoveListener(animater, action);
    }
}

