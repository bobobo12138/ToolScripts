using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GAnimationContainer
{
    public abstract class StraightLineMember : MonoBehaviour
    {
        public abstract void IDoScaleAnime();
    }

    public class StraightLineContainer
    {
        Transform parent;

        GObjPool_WithPopList<StraightLineMember> goldPool;
        GameObject parent_GoldPool;




        AnimationCurConfig animationCurConfig;

        public void InitSet(Transform parent, StraightLineMember member)
        {
            this.parent = parent;

            animationCurConfig = ConfigController.Instance.GetAnimationCurConfig();

            parent_GoldPool = new GameObject("parent_GoldPool");
            parent_GoldPool.transform.SetParent(parent);
            parent_GoldPool.transform.localPosition = Vector3.zero;
            parent_GoldPool.transform.localScale = Vector3.one;
            goldPool = new GObjPool_WithPopList<StraightLineMember>(parent_GoldPool.transform, member, 10);
            //ItemResTool.Instance.GetItem_UI_Gold()
        }

        /// <summary>
        /// 播放金币飞动画
        /// </summary>
        /// <param name="num"></param>
        /// <param name="time">时间，单位秒</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="onEnd_Single">单个金币动画完毕</param>
        /// <param name="onEnd_All">所有金币动画完毕</param>
        public void DoGoldFlightAni(int num, float time, Vector3 start, Vector3 end, Action onEnd_Single = null, Action onEnd_All = null)
        {
            ////简单限制下数量
            //int tempNum = num;
            //if (num > 10)
            //{
            //    tempNum = 10 + num / 10;
            //}

            //时间限制，转换为毫秒
            int time_Spawn = (int)(time * 1000) / 2;
            int time_Spawn_Single = time_Spawn / num;//生成间隔
            float time_Move = time / 2;//移动时间


            UniTask.Void(async () =>
            {
                for (int i = 1; i <= num; i++)
                {
                    if (i == num)
                    {
                        _DoGoldFlightAni(time_Move, start, end, () => { onEnd_Single?.Invoke(); onEnd_All?.Invoke(); });
                    }
                    else
                    {
                        _DoGoldFlightAni(time_Move, start, end, onEnd_Single);
                    }
                    await UniTask.Delay(time_Spawn_Single);
                }
            });


            void _DoGoldFlightAni(float time, Vector3 start, Vector3 end, Action onEnd = null)
            {
                StraightLineMember gold = goldPool.GetObj();
                gold.IDoScaleAnime();

                float tempTime = 0;
                start += new Vector3(Utils.GetRandom(-0.2f, 0.2f), Utils.GetRandom(-0.2f, 0.2f), 0);//临时的随机坐标

                UniTask.Void(async () =>
                {
                    while (tempTime < time)
                    {
                        if (gold != null) gold.transform.position = Vector3.Lerp(start, end, animationCurConfig.acceleration.Evaluate(tempTime / time));
                        tempTime += Time.deltaTime;
                        await UniTask.DelayFrame(1);
                    }

                    goldPool.RecycleObj(gold);
                    onEnd?.Invoke();
                });

            }
        }

        public bool isAnyFly()
        {
            if (goldPool.GetOutlist().Count != 0) return true;
            return false;
        }

    }



}

