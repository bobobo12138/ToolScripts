using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class BulletBase : MonoBehaviour
{
    protected Action<IHurt> onTrigger;

    [Header("�ɼ����ײ�㼶")]
    [SerializeField]
    protected LayerMask layerMask;
    [Header("wall�㼶��������������")]
    [Tooltip("ע��layerMask�������wall")]
    [SerializeField]
    protected LayerMask wall;
    [Header("��ײ�ж��ֶ�")]
    [SerializeField]
    protected BulletDetectionMethod bulletEnum = BulletDetectionMethod.Ray;


    public bool isSetData { get; private set; } = false;
    public bool isRun { get; private set; } = false;

    public BulletData bulletData { get; private set; }
    public Damage damage { get; private set; }

    public int penetrationNow { get; private set; }//���Ѵ�͸����

    protected IBulletCreater bulletCreater;

    protected IHurt nowTarget;//��ǰ��Ŀ�꣬��Ϊ�գ�Ĭ���ӵ��߼���û����nowTarget�Ļ���������������д����Ϊ��ѡ����

    [SerializeField]
    protected float timer_Live_Max;//����timer
    protected float speed_Mul;//�ٶȳ���
    protected float shootRange_MaxPow2;//���ƽ����������̱Ƚ��߼�
    protected Vector2 dirction;

    Vector3 startPos;//��ʼ�����λ��

    protected IHurt tempHurt;
    protected HashSet<IHurt> tempHurts;
    protected RaycastHit2D[] tempHits;

    //�и���ʱ��Ĭ����ѭ����ķ����߼���������Ҫ��д
    protected Rigidbody2D bullet_Rig;

    //��ը
    RaycastHit2D[] boomRaycastHit2Ds;

    /// <summary>
    /// ����س�ʼ���ӿ�
    /// </summary>
    protected void Init()
    {
        bullet_Rig = GetComponent<Rigidbody2D>();
        tempHurts = new HashSet<IHurt>();

        OnInit();
    }
    protected virtual void FixedUpdate()
    {
        //���ռ�ʱ
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
            //�������
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
    /// ���ò���
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
    /// ����
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
    /// �ر�
    /// </summary>
    public void SetDisEnable()
    {
        isRun = false;
        OnSetDisable();
    }

    /// <summary>
    /// ���һ���
    /// </summary>
    public void RecycleSelf()
    {
        bulletCreater.Recycle(this);
        OnRecycle();
    }


    /// <summary>
    /// ������ֵʱ
    /// </summary>
    /// <param name="bulletData"></param>
    protected abstract void OnSetData(BulletData bulletData, Damage damage, IBulletCreater bulletCreater);
    /// <summary>
    /// ������ʱ
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="pos"></param>
    /// <param name="speed"></param>
    protected abstract void OnFire(Vector2 dir, Vector3 pos, Damage damage = null, float speedMul = 1);

    protected abstract void OnRecycle();


    /// <summary>
    /// ���ر�ʱ
    /// </summary>
    public virtual void OnSetDisable()
    {

    }

    /// <summary>
    /// ������fly�߼������Խ�����д
    /// ��ģʽΪ��rigģʽ����Ҫ���ж�������߼�
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
            Debug.LogWarning("δ��⵽�ӵ����壬���Ƿ���Ҫ���壬��Ӹ��������д�����߼�");
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
                    //����Ƿ��Ǳ�ը�ӵ�
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

                //�����˿��˺������壬ִ���ӵ����߼�
                if (bulletData.IsBoom && !bulletData.SelfATKwhenBoom)
                {
                    //��ը�ӵ�����������˺�
                }
                else
                {
                    tempHurt.Hurt(damage);
                }

                //�ɴ�͸��������
                penetrationNow--;

                onTrigger?.Invoke(tempHurt);

                if (penetrationNow == 0)
                {
                    //����Ƿ��Ǳ�ը�ӵ�
                    if (bulletData.IsBoom)
                    {
                        BulletBoom();
                    }
                    //�߼���ϣ�����
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
            //����Ƿ��Ǳ�ը�ӵ�
            if (bulletData.IsBoom)
            {
                BulletBoom();
            }
            RecycleSelf();
            return;
        }
        if (!Utils.IsInLayerMask(collision.gameObject, layerMask)) return;//��ײ��Ĳ㼶����

        tempHurt = collision.transform.GetComponent<IHurt>();
        if (tempHurt == null) return;
        if (tempHurts.Contains(tempHurt)) return;

        //�����˿��˺������壬ִ���ӵ����߼�
        if (bulletData.IsBoom && !bulletData.SelfATKwhenBoom)
        {
            //��ը�ӵ�����������˺�
        }
        else
        {
            tempHurt.Hurt(damage);
        }
        //�ɴ�͸��������
        penetrationNow--;

        onTrigger?.Invoke(tempHurt);

        if (penetrationNow == 0)
        {
            //����Ƿ��Ǳ�ը�ӵ�
            if (bulletData.IsBoom)
            {
                BulletBoom();
            }
            //�߼���ϣ�����
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
        //��ɷ�Χ�˺�
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
