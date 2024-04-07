using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingleRewardsData
{
    public Sprite cover;
    public int num;
    public string name;
    public object data;

    public SingleRewardsData(Sprite cover, int num, string name, object data)
    {
        this.cover = cover;
        this.num = num;
        this.name = name;
        this.data = data;
    }

    public SingleRewardsData()
    {
        this.cover = null;
        this.num = 0;
        this.name = null;
        this.data = null;
    }
}


/// <summary>
/// 奖励的结构体
/// 可以通过getrewardspanel_helper进行显示
/// </summary>
public class RewardsData
{
    public Dictionary<string,SingleRewardsData> singleRewardsDatas;//key是SingleRewardsData.name
    public bool isRewardTransformed;//提示

    public RewardsData(Dictionary<string, SingleRewardsData> singleRewardsDatas, bool isSkin2Gold)
    {
        this.singleRewardsDatas = singleRewardsDatas;
        this.isRewardTransformed = isSkin2Gold;

    }

    public RewardsData(SingleRewardsData singleRewardsData, bool isSkin2Gold)
    {
        singleRewardsDatas = new Dictionary<string, SingleRewardsData>();
        singleRewardsDatas.Add(singleRewardsData.name, singleRewardsData);
        isRewardTransformed = isSkin2Gold;
    }

    public RewardsData()
    {
        this.singleRewardsDatas = new Dictionary<string, SingleRewardsData>();
        this.isRewardTransformed = false;
    }

    public static RewardsData operator +(RewardsData a, RewardsData b)
    {
        if (a == null || b == null)
        {
            if (a != null) return a;
            if (b != null) return b;
            return null;
        }

        foreach (var v in a.singleRewardsDatas)
        {
            if (b.singleRewardsDatas.ContainsKey(v.Key))
            {
                b.singleRewardsDatas[v.Key].num += v.Value.num;
            }
            else
            { 
                b.singleRewardsDatas.Add(v.Key, v.Value);
            }
        }
        bool sumIsSkin2Gold = a.isRewardTransformed || b.isRewardTransformed;

        return new RewardsData(b.singleRewardsDatas, sumIsSkin2Gold);
    }

}
