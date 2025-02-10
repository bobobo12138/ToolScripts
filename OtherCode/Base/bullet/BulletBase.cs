using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class BulletBase : MonoBehaviour
{
    protected Action<IHurt> onTrigger;

    [Header("可检测碰撞层级")]
    [SerializeField]
    protected LayerMask layerMask;
    [Header("wall层级，碰到会自销毁")]
    [Tooltip("注意layerMask必须包含wall")]
    [SerializeField]
    protected LayerMask wall;
    [Header("碰撞判定手段")]
    [SerializeField]
    protected BulletDetectionMethod bulletEnum = BulletDetectionMethod.Ray;


    public bool isSetData { get; private set; } = false;
    public bool isRun { get; private set; } = false;

    public BulletData bulletData { get; private set; }
    public Damage damage { get; private set; }

    public int penetrationNow { get; private set; }//现已穿透数量

    protected IBulletCreater bulletCreater;

    protected IHurt nowTarget;//当前的目标，可为空；默认子弹逻辑并没有与nowTarget的互动，但可以在重写中作为可选参数

    [SerializeField]
    protected float timer_Live_Max;//存在timer
    protected float speed_Mul;//速度乘数
    protected float shootRange_MaxPow2;//射程平方，用于射程比较逻辑
    protected Vector2 dirction;

    Vector3 startPos;//开始射击的位置

    protected IHurt tempHurt;
    protected HashSet<IHurt> tempHurts;
    protected RaycastHit2D[] tempHits;

    //有刚体时会默认遵循本类的飞行逻辑，否则需要重写
    protected Rigidbody2D bullet_Rig;

    //爆炸
    RaycastHit2D[] boomRaycastHit2Ds;

    /// <summary>
    /// 对象池初始化接口
    /// </summary>
    protected void Init()
    {
        bullet_Rig = GetComponent<Rigidbody2D>();
        tempHurts = new HashSet<IHurt>();

        OnInit();
    }
    protected virtual void FixedUpdate()
    {
        //回收计时
        if (timer_Live_Max <= 0)
        {
            bulletCreater.Recycle(this);
            return;
        }
        else
        {
            timer_Live_Max -= Time.fixedDeltaTime;
        }

        if (shootRange_MaxPow2 != -1)
        {
            //射程限制
            if ((transform.position - startPos).sqrMagnitude > shootRange_MaxPow2)
            {
                bulletCreater.Recycle(this);
                return;
            }
        }

        BulletLogic_RayType();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BulletLogic_ColliderType(collision);
    }


    /// <summary>
    /// 设置参数
    /// </summary>
    /// <param name="bulletData"></param>
    public void SetData(BulletData bulletData, Damage damage, IBulletCreater bulletCreater)
    {
        isSetData = true;
        this.bulletData = bulletData;
        this.damage = damage;
        this.bulletCreater = bulletCreater;
        OnSetData(bulletData, damage, bulletCreater);
    }

    /// <summary>
    /// 开火
    /// </summary>
    public void Fire(Vector2 dir, Vector3 pos, Damage damage = null, float speedMul = 1, float shootRange_Max = -1, Action<IHurt> onTrigger = null, IHurt target = null)
    {
        isRun = true;

        transform.right = dir;
        transform.position = pos;
        if (damage != null) this.damage = damage;
        this.speed_Mul = speedMul;
        if (shootRange_Max != -1)
        {
            this.shootRange_MaxPow2 = MathF.Pow(shootRange_Max, 2);
        }
        else
        {
            this.shootRange_MaxPow2 = -1;
        }
        this.onTrigger = onTrigger;
        this.nowTarget = target;

        penetrationNow = bulletData.PenetrationMax;
        tempHurts.Clear();

        timer_Live_Max = bulletData.TimeLiveMax;

        startPos = pos;

        FlyLogic(dir, pos, speedMul);
        OnFire(dir, pos, damage, speedMul);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void SetDisEnable()
    {
        isRun = false;
        OnSetDisable();
    }

    /// <summary>
    /// 自我回收
    /// </summary>
    public void RecycleSelf()
    {
        bulletCreater.Recycle(this);
        OnRecycle();
    }


    /// <summary>
    /// 当设置值时
    /// </summary>
    /// <param name="bulletData"></param>
    protected abstract void OnSetData(BulletData bulletData, Damage damage, IBulletCreater bulletCreater);
    /// <summary>
    /// 当开火时
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="pos"></param>
    /// <param name="speed"></param>
    protected abstract void OnFire(Vector2 dir, Vector3 pos, Damage damage = null, float speedMul = 1);

    protected abstract void OnRecycle();


    /// <summary>
    /// 当关闭时
    /// </summary>
    public virtual void OnSetDisable()
    {

    }

    /// <summary>
    /// 基础的fly逻辑，可以进行重写
    /// 若模式为非rig模式则需要自行定义飞行逻辑
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="pos"></param>
    /// <param name="speedMul"></param>
    protected virtual void FlyLogic(Vector2 dir, Vector3 pos, float speedMul = 1)
    {
        if (bullet_Rig != null)
        {
            bullet_Rig.velocity = Vector3.zero;
            bullet_Rig.angularVelocity = 0;
            bullet_Rig.AddForce(transform.right * bulletData.SpeedBasic * speedMul);
        }
        else
        {
            Debug.LogWarning("未检测到子弹刚体，你是否需要刚体，添加刚体或者重写飞行逻辑");
        }
    }


    void BulletLogic_RayType()
    {
        if (!isRun) return;
        if (bulletEnum != BulletDetectionMethod.Ray) return;

        if (bullet_Rig != null)
        {
            tempHits = Physics2D.RaycastAll(transform.position, bullet_Rig.velocity.normalized, 0.2f, layerMask);
        }
        else
        {
            tempHits = Physics2D.RaycastAll(transform.position, transform.right, 0.2f, layerMask);
        }
        if (tempHits.Length > 0)
        {
            foreach (var v in tempHits)
            {
                if (Utils.IsInLayerMask(v.transform.gameObject, wall))
                {
                    //检测是否是爆炸子弹
                    if (bulletData.IsBoom)
                    {
                        BulletBoom();
                    }
                    RecycleSelf();
                    return;
                }
                tempHurt = v.transform.GetComponent<IHurt>();
                if (tempHurt == null) continue;
                if (tempHurts.Contains(tempHurt)) continue;

                //碰到了可伤害的物体，执行子弹的逻辑
                if (bulletData.IsBoom && !bulletData.SelfATKwhenBoom)
                {
                    //爆炸子弹且自身不造成伤害
                }
                else
                {
                    tempHurt.Hurt(damage);
                }

                //可穿透数量减少
                penetrationNow--;

                onTrigger?.Invoke(tempHurt);

                if (penetrationNow == 0)
                {
                    //检测是否是爆炸子弹
                    if (bulletData.IsBoom)
                    {
                        BulletBoom();
                    }
                    //逻辑完毕，回收
                    RecycleSelf();
                    return;
                }
                else
                {
                    tempHurts.Add(tempHurt);
                }

                Debug.DrawLine(transform.position, v.point, Color.yellow);
            }
        }
    }

    void BulletLogic_ColliderType(Collider2D collision)
    {
        if (!isRun) return;
        if (bulletEnum != BulletDetectionMethod.Collider) return;

        if (Utils.IsInLayerMask(collision.gameObject, wall))
        {
            //检测是否是爆炸子弹
            if (bulletData.IsBoom)
            {
                BulletBoom();
            }
            RecycleSelf();
            return;
        }
        if (!Utils.IsInLayerMask(collision.gameObject, layerMask)) return;//碰撞体的层级限制

        tempHurt = collision.transform.GetComponent<IHurt>();
        if (tempHurt == null) return;
        if (tempHurts.Contains(tempHurt)) return;

        //碰到了可伤害的物体，执行子弹的逻辑
        if (bulletData.IsBoom && !bulletData.SelfATKwhenBoom)
        {
            //爆炸子弹且自身不造成伤害
        }
        else
        {
            tempHurt.Hurt(damage);
        }
        //可穿透数量减少
        penetrationNow--;

        onTrigger?.Invoke(tempHurt);

        if (penetrationNow == 0)
        {
            //检测是否是爆炸子弹
            if (bulletData.IsBoom)
            {
                BulletBoom();
            }
            //逻辑完毕，回收
            RecycleSelf();
        }
        else
        {
            tempHurts.Add(tempHurt);
        }

        Debug.DrawLine(transform.position, collision.transform.position, Color.red);
    }


    void BulletBoom()
    {
        //造成范围伤害
        boomRaycastHit2Ds = Physics2D.CircleCastAll(transform.position, bulletData.BoomR, Vector2.zero, 0, layerMask);

        foreach (var v in Utils.Deduplication_RaycastHit2D(boomRaycastHit2Ds))
        {
            tempHurt = v.transform.GetComponent<IHurt>();
            if (tempHurt == null) continue;

            var tempDamage = Damage.GetTempDamage();
            tempDamage.SetValue((int)(damage.damage * bulletData.BoomATKPer));

            tempHurt.Hurt(tempDamage);

            onTrigger?.Invoke(tempHurt);
        }
    }

    protected abstract void OnInit();
}
