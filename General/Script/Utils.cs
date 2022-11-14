using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Utils
{
    public struct Coroutines
    {
        public static WaitForSeconds waiteSecond1 = new WaitForSeconds(1);
        public static WaitForSeconds waiteSecond2 = new WaitForSeconds(2);
        public static WaitForSeconds waiteSecond3 = new WaitForSeconds(3);
    }


    /// <summary>
    /// 随机整数 取不到max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static int GetRandom(int max)
    {
        return Random.Range(0, max);
    }
    /// <summary>
    /// 随机整数 取不到max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static float GetRandom(float max)
    {
        return Random.Range(0, max);
    }

    /// <summary>
    /// 随机整数 取不到max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static int GetRandom(int min, int max)
    {
        return Random.Range(min, max);
    }
    /// <summary>
    /// 随机小数 可以取到max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static float GetRandom(float min, float max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// 返回二维空间中的随机坐标
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static Vector2 GetRandom(Vector2 min, Vector2 max)
    {
        return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }




    /// <summary>
    /// 屏幕坐标转3d
    /// </summary>
    /// <param name="vec2"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 PointVec2ToVec3(Vector3 vec2, float z)
    {
        Vector3 world = new Vector3(vec2.x / Screen.width, vec2.y / Screen.height, z);
        Vector3 world1 = Camera.main.ViewportToWorldPoint(new Vector3(world.x, world.y, world.z)); // 屏幕坐标转换成场景坐标
        return world1;
    }

    /// <summary>
    /// 得到【屏幕外物体位置到屏幕中心的连线】与屏幕边界的交点，无死角。
    /// </summary>
    /// <param name="x">物体X坐标</param>
    /// <param name="y">物体Y坐标</param>
    /// <param name="width">屏幕宽度</param>
    /// <param name="height">屏幕高度</param>
    /// <returns></returns>
    public static Vector2 CalculateIntersectionBetter(float x, float y, float width, float height)
    {
        Vector2 position = new Vector2();
        if (CheckInView(x, y, width, height))
        {
            position.x = x;
            position.y = y;

            return position;
        }

        float aspectRatio = height / width;
        float relativeY = y - height / 2;
        float relativeX = x - width / 2;

        relativeX = relativeX == 0 ? 0.01f : relativeX;//GetSafeFloatDivisor ： return value = value == 0 ? 0.01f : value;

        float k = relativeY / relativeX;

        /*
         * 
         *                    |
         *           2        |        3
         *                    |
         *                    |
         *                    |
         *    1               |               4
         *                    |
         *                    |
         *————————————————————|————————————————————h/2
         *                    |
         *                    |
         *    8               |               5
         *                    |
         *                    |
         *                    |
         *           7        |        6
         *                    |
         *                   w/2
         * 
         *
         * 8=1  2=3  4=5  6=7
         */

        if (y > height / 2)
        {
            if (x < width / 2)
            {
                if (-aspectRatio < k)   //1
                {
                    position.x = 0;
                    position.y = height / 2 + (y - (height / 2)) * (width / 2) / (width / 2 - x);
                }
                else                    //2
                {
                    position.x = width / 2 + (x - (width / 2)) * (height / 2) / (y - height / 2);
                    position.y = height;
                }
            }
            else
            {
                if (aspectRatio < k)    //3
                {
                    position.x = width / 2 + (x - (width / 2)) * (height / 2) / (y - height / 2);
                    position.y = height;
                }
                else                    //4
                {
                    position.x = width;
                    position.y = height / 2 + (y - (height / 2)) * (width / 2) / (x - width / 2);
                }
            }
        }
        else
        {
            if (x > width / 2)
            {
                if (-aspectRatio < k)   //5
                {
                    position.x = width;
                    position.y = height / 2 + (y - (height / 2)) * (width / 2) / (x - width / 2);
                }
                else                    //6
                {
                    position.y = 0;
                    position.x = width / 2 + (x - (width / 2)) * (height / 2) / (height / 2 - y);
                }
            }
            else
            {
                if (aspectRatio < k)    //7
                {
                    position.y = 0;
                    position.x = width / 2 + (x - (width / 2)) * (height / 2) / (height / 2 - y);
                }
                else                    //8
                {
                    position.x = 0;
                    position.y = height / 2 + (y - (height / 2)) * (width / 2) / (width / 2 - x);
                }
            }
        }

        return position;
    }

    /// <summary>
    /// 确认目标是否在视野内
    /// </summary>
    /// <param name="x">物体X坐标</param>
    /// <param name="y">物体Y坐标</param>
    /// <param name="width">屏幕宽度</param>
    /// <param name="height">屏幕高度</param>
    public static bool CheckInView(float x, float y, float width, float height)
    {
        return x > 0 && x < width && y > 0 && y < height;
    }

    /// <summary>
    /// 删除子节点
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="ReserveNum">需要保留的数量 默认全部清空</param>
    public static void DestroyAllChildren(Transform tr, int ReserveNum = 0)
    {
        for (int i = tr.childCount - 1; i >= ReserveNum; i--)
        {
            Object.Destroy(tr.GetChild(i).gameObject);
        }

    }

    /// <summary>
    /// 隐藏所有子节点
    /// </summary>
    /// <param name="tr"></param>
    public static void HideAllChildren(Transform tr)
    {
        for (int i = tr.childCount - 1; i >= 0; i--)
        {
            tr.GetChild(i).gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 获取两点之间距离一定百分比的一个点
    /// </summary>
    /// <param name="start">起始点</param>
    /// <param name="end">结束点</param>
    /// <param name="distance">起始点到目标点距离百分比</param>
    /// <returns></returns>
    public static Vector3 GetBetweenPointInPercentage(Vector3 start, Vector3 end, float percent)
    {
        Vector3 normal = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        return normal * (distance * percent) + start;
    }

    /// <summary>
    /// 获取两点之间一定距离的点
    /// </summary>
    /// <param name="start">起始点</param>
    /// <param name="end">结束点</param>
    /// <param name="distance">距离</param>
    /// <returns></returns>
    public static Vector3 GetBetweenPointInLength(Vector3 start, Vector3 end, float distance)
    {
        Vector3 normal = (end - start).normalized;
        return normal * distance + start;
    }

    /// <summary>
    /// 得到point是否在多边形内
    /// </summary>
    /// <param name="point">判断的点</param>
    /// <param name="polygon">多边形数组</param>
    /// <returns></returns>
    public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        int polygonLength = polygon.Length, i = 0;
        bool inside = false;

        float pointX = point.x, pointY = point.y;

        float startX, startY, endX, endY;
        Vector2 endPoint = polygon[polygonLength - 1];
        endX = endPoint.x;
        endY = endPoint.y;
        while (i < polygonLength)
        {
            startX = endX;
            startY = endY;
            endPoint = polygon[i++];
            endX = endPoint.x;
            endY = endPoint.y;
            inside ^= (endY > pointY ^ startY > pointY) && ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
        }
        return inside;
    }

    /// <summary>
    /// 读取json
    /// </summary>
    /// <param name="jsonPath">json路径</param>
    public static JsonReader ReadJsonData(string jsonPath)
    {
        StreamReader streamreader = new StreamReader(jsonPath);
        JsonReader js = new JsonReader(streamreader);
        return js;
    }

    /// <summary>
    /// 从resources加载图片
    /// </summary>
    /// <param name="img"></param>
    /// <param name="imgPath"></param>
    public static void LoadImage(Image img, string imgPath)
    {
        //把资源加载到内存中
        Object Preb = Resources.Load(imgPath, typeof(Sprite));
        try
        {
            Sprite tmpsprite = Object.Instantiate(Preb) as Sprite;
            img.sprite = tmpsprite;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("加载图片失败:" + imgPath + " / " + ex);
        }
    }

    public static void LoadMesh(SkinnedMeshRenderer skinnedMeshRenderer, string meshPath)
    {
        Object Preb = Resources.Load(meshPath, typeof(Mesh));
        try
        {
            Mesh tmpMesh = Object.Instantiate(Preb) as Mesh;
            skinnedMeshRenderer.sharedMesh = tmpMesh;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("加载Mesh失败" + ex);
        }
    }

    public static void LoadMaterial(SkinnedMeshRenderer skinnedMeshRenderer, string materialPath)
    {
        Object Preb = Resources.Load(materialPath, typeof(Material));
        try
        {
            Material tmpMaterial = Object.Instantiate(Preb) as Material;
            skinnedMeshRenderer.material = tmpMaterial;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("加载Material失败" + ex);
        }
    }

    /// <summary>
    /// 递归查找某个transform下的所有物体
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    /// <returns></returns>
    public static Transform FindChildInTransform(Transform parent, string child)
    {
        Transform childTF = parent.Find(child);
        if (childTF != null)
        {
            return childTF;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            childTF = FindChildInTransform(parent.GetChild(i), child);
            if (childTF != null)
            {
                return childTF;
            }
        }

        return null;
    }

    //我们首先用二元一次方程求得在扇形内
    //再得到所在位置圆的半径
    //然后判断是否在圆内
    //⑴当x^2+y^2>r^2
    //时，则点P在圆外。
    //⑵当x^2+y^2=r^2
    //时，则点P在圆上。
    //⑶当x^2+y^2<r^2
    //时，则点P在圆内。
    /*public static bool IsInSight(float XView, float YView, Vector3 Position)
    {
        ///首先计算是否在视野的扇形范围内，会用到unity坐标的x,z轴做平面
        ///YView * Y = XView * X相当于方程Y=aX
        ///

        if(YView* Position.z>XView* Position.x)
    }*/


    ////求角度 及前后左右方位3D
    //public static float CheckTargetDirForMe(Transform target, Transform me)
    //{
    //    //xuqiTest：  target.position = new Vector3(3, 0, 5);
    //    Vector3 dir = target.position - me.position; //位置差，方向
    //    //方式1   点乘
    //    //点积的计算方式为: a·b =| a |·| b | cos < a,b > 其中 | a | 和 | b | 表示向量的模 。
    //    float dot = Vector3.Dot(me.forward, dir.normalized);//点乘判断前后   //dot >0在前  <0在后 
    //    float dot1 = Vector3.Dot(me.right, dir.normalized);//点乘判断左右    //dot1>0在右  <0在左                                               
    //    float angle = Mathf.Acos(Vector3.Dot(me.forward.normalized, dir.normalized)) * Mathf.Rad2Deg;//通过点乘求出夹角
    //    Debug.Log(angle);
    //    return angle;
    //}


    //求角度 及前后左右方位2D
    public static float CheckTargetDirForMe(Transform target, Transform me)
    {
        //xuqiTest：  target.position = new Vector3(3, 0, 5);
        Vector3 dir = target.position - me.position; //位置差，方向
        dir.y = 0;
        //方式1   点乘
        //点积的计算方式为: a·b =| a |·| b | cos < a,b > 其中 | a | 和 | b | 表示向量的模 。
        float dot = Vector3.Dot(me.forward, dir.normalized);//点乘判断前后   //dot >0在前  <0在后 
        float dot1 = Vector3.Dot(me.right, dir.normalized);//点乘判断左右    //dot1>0在右  <0在左                                               
        float angle = Mathf.Acos(Vector3.Dot(me.forward.normalized, dir.normalized)) * Mathf.Rad2Deg;//通过点乘求出夹角
        if (angle != angle) angle = 0;//防止NAN
        return angle;
    }

    /// <summary>
    /// 交换父级
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public static void SwapParent(Transform A, Transform B, Transform AParent = null, Transform BParent = null)
    {
        var tempAParent = A.parent;
        var tempBParent = B.parent;
        ///若设置了预定的父级，则使用预定的父级
        if (AParent != null) tempAParent = AParent;
        if (BParent != null) tempBParent = BParent;

        A.SetParent(tempBParent);
        A.position = tempBParent.position;
        B.SetParent(tempAParent);
        B.position = tempAParent.position;
    }

    /// <summary>
    /// 抽卡，百分
    /// </summary>
    public static bool LuckDraw(int probability)
    {
        if (probability >= 100) return true;//must

        if (probability <= 0) return false; //never

        if (GetRandom(1, 101) <= probability)//相当于1-100中取随机数
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 抽卡，浮点数
    /// </summary>
    /// <param name="probability"></param>
    /// <returns></returns>
    public static bool LuckDraw(float probability)
    {
        if (probability >= 1) return true;//must

        if (probability <= 0) return false; //never

        if (GetRandom(0, 1.0f) <= probability)//相当于1-100中取随机数
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// 抽卡，自定义分子分母
    /// </summary>
    /// <param name="numerator"></param>
    /// <param name="denominator"></param>
    /// <returns></returns>
    public static bool LuckDraw(int numerator,int denominator)
    {
        if (numerator >= denominator) return true;

        if (numerator <= 0) return false;

        if (GetRandom(1, denominator) <= numerator)//相当于1-100中取随机数
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 字符串转换为枚举
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumType"></param>
    /// <param name="state"></param>
    public T StringToEnum<T>(string state)
    {
        ///字符串转枚举
        return (T)System.Enum.Parse(typeof(T), state);
    }


    /// <summary>
    /// 取最大值
    /// </summary>
    /// <returns></returns>
    public static float GetMax(float v1,float v2)
    {
        if (v1 > v2)
        {
            return v1;
        }
        else
        {
            return v2;
        }
    }

    /// <summary>
    /// 抽卡，分子，分母
    /// </summary>
    /// <param name="moleculem"></param>
    /// <param name="denominator"></param>
    /// <returns></returns>
    public static bool DrawACard(float moleculem, float denominator)
    {
        var card = GetRandom(0, denominator);

        if (card < moleculem)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 二进制序列化拷贝
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepCopyByBinary<T>(T obj)
    {
        object retval;
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            retval = bf.Deserialize(ms);
            ms.Close();
        }
        return (T)retval;
    }


    /// <summary>
    /// 求向量（1,0）旋转degrees度的坐标
    /// </summary>
    /// <param name="degrees"></param>
    /// <returns></returns>
    public static Vector2 RotationOfVectors(float degrees)
    {
        Vector2 Vector;
        Vector.x = 1;
        Vector.y = 0;

        Vector.x = 1 * Mathf.Cos(Mathf.PI/180 * degrees);
        Vector.y = 1 * Mathf.Sin(Mathf.PI / 180 * degrees);

        return Vector;
    }

}

