using System.Collections.Generic;

public sealed class BulletData
{

    public BulletData(int id, string name, string Language, string detial, int CollisionRayDis, int speed_Basic, bool isBoom, bool selfATKwhenBoom, float boomR, float boomATKPer, float time_Live_Max, int penetrationMax, System.Collections.Generic.Dictionary<string, string> paramsDic, System.Collections.Generic.List<string> paramsDetial)
    {
        this.Id = id;
        this.Name = name;
        this.Language = Language;
        this.Detial = detial;
        this.CollisionRayDis = CollisionRayDis;
        this.SpeedBasic = speed_Basic;
        this.IsBoom = isBoom;
        this.SelfATKwhenBoom = selfATKwhenBoom;
        this.BoomR = boomR;
        this.BoomATKPer = boomATKPer;
        this.TimeLiveMax = time_Live_Max;
        this.PenetrationMax = penetrationMax;
        this.ParamsDic = paramsDic;
        this.ParamsDetial = paramsDetial;
    }

    /// <summary>
    /// 可能的自定义...
    /// </summary>
    public BulletData()
    { 
    
    }

    /// <summary>
    /// 这是id<br/>子弹的前三位是100
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 这是名字，会用于寻址
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 多语言
    /// </summary>
    public string Language { get; private set; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Detial { get; private set; }
    /// <summary>
    /// 容错修正射线的长度
    /// </summary>
    public int CollisionRayDis { get; private set; }
    /// <summary>
    /// 基础速度m/s<br/>556M193普通弹初速997
    /// </summary>
    public int SpeedBasic { get; private set; }
    /// <summary>
    /// 子弹击中是否爆炸
    /// </summary>
    public bool IsBoom { get; private set; }
    /// <summary>
    /// 爆炸子弹自身是否造成伤害(自身+爆炸会造成两段伤害)
    /// </summary>
    public bool SelfATKwhenBoom { get; private set; }
    /// <summary>
    /// 爆炸半径
    /// </summary>
    public float BoomR { get; private set; }
    /// <summary>
    /// 伤害占基础伤害的多少百分比
    /// </summary>
    public float BoomATKPer { get; private set; }
    /// <summary>
    /// 存在时间s
    /// </summary>
    public float TimeLiveMax { get; private set; }
    /// <summary>
    /// 子弹可穿透的数量
    /// </summary>
    public int PenetrationMax { get; private set; }
    /// <summary>
    /// 参数字典dic&lt;string,string&gt;
    /// </summary>
    public System.Collections.Generic.Dictionary<string, string> ParamsDic { get; private set; }
    /// <summary>
    /// 参数描述
    /// </summary>
    public System.Collections.Generic.List<string> ParamsDetial { get; private set; }
}

