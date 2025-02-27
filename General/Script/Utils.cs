﻿using LitJson;
//using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.RectTransform;

public class Utils
{
    public struct Coroutines
    {
        public static WaitForSeconds waiteSecond1 = new WaitForSeconds(1);
        public static WaitForSeconds waiteSecond2 = new WaitForSeconds(2);
        public static WaitForSeconds waiteSecond3 = new WaitForSeconds(3);
    }

    //可用于临时处理的temp数据容器
    public static int tempInt;
    public static float tempFloat;
    public static Vector2 tempVector2;
    public static Vector3 tempVector3;
    public static List<int> tempInts = new List<int>();
    public static HashSet<int> tempIntHash = new HashSet<int>();

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
    /// 从列表中找出两个不同的元素
    /// </summary>
    /// <param name="inputList"></param>
    /// <returns></returns>
    public static List<string> GetRandomDistinctElements(List<string> inputList, int count)
    {
        if (inputList == null || count <= 0 || count > inputList.Count)
        {
            UnityEngine.Debug.LogError("输入参数无效");
            return null;
        }

        List<string> result = new List<string>();
        System.Random random = new System.Random();

        while (result.Count < count)
        {
            int randomIndex = random.Next(0, inputList.Count);
            string randomElement = inputList[randomIndex];

            if (!result.Contains(randomElement))
            {
                result.Add(randomElement);
            }
        }

        return result;
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
    public static LitJson.JsonReader ReadJsonData(string jsonPath)
    {
        StreamReader streamreader = new StreamReader(jsonPath);
        LitJson.JsonReader js = new LitJson.JsonReader(streamreader);
        return js;
    }

    public static byte[] GetBytesFromPath(string path)
    {
        FileStream stream = new FileInfo(path).OpenRead();
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, System.Convert.ToInt32(stream.Length));
        stream.Close();
        stream.Dispose();
        return buffer;
    }

    public static GameObject CreateNewGameObject(Transform parent, string name)
    {
        GameObject temp = new GameObject(name);
        temp.transform.parent = parent;
        temp.transform.localPosition = Vector3.zero;
        return temp;
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
    //    AprilDebug.Log(angle);
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
    public static bool LuckDraw(int numerator, int denominator)
    {
        if (numerator >= denominator) return true;

        if (numerator <= 0) return false;

        if (GetRandom(1, denominator) <= numerator)//相当于1-100中取随机数
        {
            return true;
        }
        return false;
    }


    public static (int pos, bool isGet) LuckDraw_WithPos(int numerator, int denominator)
    {
        if (numerator >= denominator) return (-1, true);

        if (numerator <= 0) return (-1, false);

        var random = GetRandom(1, denominator);
        if (random <= numerator)//相当于1-100中取随机数
        {
            return (random, true);
        }
        return (random, false);
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
    public static float GetMax(float v1, float v2)
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
    //public static T DeepCopyByJsonSerialize<T>(T obj)
    //{
    //    JsonSerializerSettings settings = new JsonSerializerSettings
    //    {
    //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    //    };
    //    string json = JsonConvert.SerializeObject(obj, settings);

    //    return JsonConvert.DeserializeObject<T>(json, settings);
    //}

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

        Vector.x = 1 * Mathf.Cos(Mathf.PI / 180 * degrees);
        Vector.y = 1 * Mathf.Sin(Mathf.PI / 180 * degrees);

        return Vector;
    }

    /// <summary>
    /// 比例计算器
    /// 参考者，被计算者，变化量or
    /// 分子，分母，变量
    /// </summary>
    public static float ProportionCalculate(float molecule, float denominator, float v)
    {
        return (molecule / denominator) * v;
    }

    /// <summary>
    /// 宽高限定计算器
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="max">最大</param>
    /// <returns></returns>
    public static Vector2 WHSizelimit(Vector2 user, Vector2 max)
    {
        float prox = 1;
        float proy = 1;

        if (user.x >= max.x)
        {
            if (user.x > max.x)
            {
                prox = max.x / user.x;
            }
        }
        if (user.y > max.y)
        {
            if (user.y > max.y)
            {
                proy = max.y / user.y;
            }
        }

        return new Vector2(user.x * Mathf.Min(prox, proy), user.y * Mathf.Min(prox, proy));
    }

    /// <summary>
    /// 最大平铺，将user铺满max
    /// 返回乘数
    /// </summary>
    /// <param name="user"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float MaxTiled(Vector2 user, Vector2 max)
    {
        float x = max.x / user.x;
        float y = max.y / user.y;

        if (x >= y)
        {
            return x;
        }
        else
        {
            return y;
        }
    }

    /// <summary>
    /// 最大平铺，自动设置RectTransform大小
    /// </summary>
    /// <param name="userRectTransform"></param>
    /// <param name="user"></param>
    /// <param name="max"></param>
    public static void MaxTiled(RectTransform userRectTransform, Vector2 user, Vector2 max)
    {
        ///获取乘数
        float mul = 1;
        float x = max.x / user.x;
        float y = max.y / user.y;

        if (x >= y)
        {
            mul = x;
        }
        else
        {
            mul = y;
        }
        ///设置大小
        userRectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, user.x * mul);
        userRectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, user.y * mul);
    }


    /// <summary>
    /// 最小平铺，将user长轴铺满max
    /// 返回乘数
    /// </summary>
    /// <param name="user"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float MinTiled(Vector2 user, Vector2 max)
    {
        float x = max.x / user.x;
        float y = max.y / user.y;

        if (x >= y)
        {
            return y;
        }
        else
        {
            return x;
        }
    }

    public static void MinTiled(RectTransform userRectTransform, Vector2 user, Vector2 max)
    {
        ///获取乘数
        float mul = 1;
        float x = max.x / user.x;
        float y = max.y / user.y;

        if (x >= y)
        {
            mul = y;
        }
        else
        {
            mul = x;
        }
        ///设置大小
        userRectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, user.x * mul);
        userRectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, user.y * mul);
    }

    /// <summary>
    /// 字符串转枚举
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <returns></returns>
    public static T ToEnum<T>(string str)
    {
        UnityEngine.Debug.Log(str);
        return (T)System.Enum.Parse(typeof(T), str);
    }

    /// <summary>
    /// 最大公约数
    /// </summary>
    /// <param name="m"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static int Gcd(int m, int n)
    {
        if (m == 0)
            return n;

        return Gcd(n % m, m);
    }

    /// <summary>
    /// 获取Blit的renderTexture，会分配一个新的renderTexture
    /// 注意必须手动ReleaseTemporary！
    /// </summary>
    /// <param name="texture2D"></param>
    /// <param name="material"></param>
    /// <param name="accuracy">精度，全精度是1，半精度就是2</param>
    /// <returns></returns>
    public static RenderTexture GetBlitRenderTexture(Texture2D texture2D, Material material, int accuracy = 1)
    {
        var tempRenderTexture = RenderTexture.GetTemporary(texture2D.width / accuracy, texture2D.height / accuracy, 0);
        Graphics.Blit(texture2D, tempRenderTexture, material);
        return tempRenderTexture;
    }


    /// <summary>
    /// 判断rect是否重合
    /// 后续考虑封装成扩展方法，特别是RectOverlaps(RectTransform rectTransform1, RectTransform rectTransform2, Camera camera)
    /// 发现rect1.Overlaps有误差，具体查看"问题备忘"
    /// </summary>
    public static bool RectOverlaps(Rect rect1, Rect rect2)
    {
        return rect1.Overlaps(rect2);
    }

    static Rect tempRect1;
    static Rect tempRect2;
    public static bool RectOverlaps(RectTransform rectTransform1, RectTransform rectTransform2, Camera camera)
    {
        tempRect1 = rectTransform1.rect;
        tempRect1.position = camera.WorldToScreenPoint(rectTransform1.position);
        tempRect1.position -= camera.pixelRect.size / 2;
        tempRect2 = rectTransform2.rect;
        tempRect2.position = camera.WorldToScreenPoint(rectTransform2.position);
        tempRect2.position -= camera.pixelRect.size / 2;

        if (Mathf.Abs(tempRect1.position.x - tempRect2.position.x) < tempRect1.size.x / 2 + tempRect2.size.x / 2 &&
            Mathf.Abs(tempRect1.position.y - tempRect2.position.y) < tempRect1.size.y / 2 + tempRect2.size.y / 2)
        {
            return true;
        }
        return false;

    }
    public static bool RectOverlaps(Rect rect1, Rect rect2, Camera camera)
    {
        tempRect1 = rect1;
        tempRect1.position = camera.WorldToScreenPoint(rect1.position);
        tempRect1.position -= camera.pixelRect.size / 2;
        tempRect2 = rect2;
        tempRect2.position = camera.WorldToScreenPoint(rect2.position);
        tempRect2.position -= camera.pixelRect.size / 2;

        if (Mathf.Abs(tempRect1.position.x - tempRect2.position.x) < tempRect1.size.x / 2 + tempRect2.size.x / 2 &&
            Mathf.Abs(tempRect1.position.y - tempRect2.position.y) < tempRect1.size.y / 2 + tempRect2.size.y / 2)
        {
            return true;
        }
        return false;

    }

    //public static bool RectOverlaps(RectTransform rectTransform1, RectTransform rectTransform2, Camera camera)
    //{
    //    tempRect1 = rectTransform1.rect;
    //    tempRect1.position = camera.WorldToViewportPoint(rectTransform1.position);
    //    tempRect1.position = tempRect1.position * camera.pixelRect.size - (camera.pixelRect.size / 2);
    //    tempRect2 = rectTransform2.rect;
    //    tempRect2.position = camera.WorldToViewportPoint(rectTransform2.position);
    //    tempRect2.position = tempRect2.position * camera.pixelRect.size - (camera.pixelRect.size / 2);
    //    //return tempRect1.Overlaps(tempRect2);


    //    if (Mathf.Abs(tempRect1.position.x - tempRect2.position.x) < tempRect1.size.x / 2 + tempRect2.size.x / 2)
    //    {
    //        return true;
    //    }
    //    return false;
    //}


    /// <summary>
    /// 将世界转本地
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="targetRectTransform"></param>
    /// <returns></returns>
    public static Vector3 ConvertWorldToLocal(Vector3 worldPosition, Transform targetRectTransform)
    {
        return targetRectTransform.InverseTransformPoint(worldPosition);
    }

    /// <summary>
    /// 本地转世界
    /// </summary>
    /// <param name="localPosition"></param>
    /// <param name="targetRectTransform"></param>
    /// <param name="targetRectTransform">是否计算旋转，旋转也会影响局部坐标</param>
    /// <returns></returns>
    public static Vector3 ConvertLocalToWorld(Vector3 localPosition, Transform targetRectTransform, bool isCalRotate = false)
    {
        if (isCalRotate)
        {
            return targetRectTransform.TransformPoint(localPosition);
        }
        else
        {
            var tempRotate = targetRectTransform.localEulerAngles;
            targetRectTransform.localEulerAngles = Vector3.zero;
            var pos = targetRectTransform.TransformPoint(localPosition);
            targetRectTransform.localEulerAngles = tempRotate;
            return pos;
        }
    }

    /// <summary>
    /// renderTexture2texutre
    /// </summary>
    /// <param name="rTex"></param>
    /// <returns></returns>
    public static Texture2D RenderTexture2Texture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }



    /// <summary>
    /// 求等差数列和
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static int SumArithmeticSeries(int num)
    {
        int sum = 0;

        for (int i = 1; i <= num; i++)
        {
            sum += i;
        }
        return sum;
    }


    ///// <summary>
    ///// 将DateTime类型转换为long类型
    ///// </summary>
    ///// <param name="dt">时间</param>
    ///// <returns></returns>
    //public static long ConvertDataTimeLong(System.DateTime dt)
    //{
    //    //dateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000
    //    System.DateTime dtBase = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
    //    System.TimeSpan toNow = dt.ToUniversalTime().Subtract(dtBase);
    //    long timeStamp = toNow.Ticks / 10000;
    //    return timeStamp;
    //}
    /// <summary>
    /// 将long类型转换为DateTime类型
    /// </summary>
    /// <param name="timeStamp">时间戳</param>
    /// <returns></returns>
    public static System.DateTime ConvertLongtoDataTime(long timeStamp)
    {
        System.DateTime dtBase = new System.DateTime(1970, 1, 1, 0, 0, 0);
        return dtBase.AddSeconds(timeStamp);
    }


    public static int LayerMaskToLayer(LayerMask layerMask)
    {
        for (int i = 0; i < 32; i++)
        {
            if ((layerMask & (1 << i)) != 0)
            {
                return i;
            }
        }

        return -1;
    }

    public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        // 根据Layer数值进行移位获得用于运算的Mask值
        int objLayerMask = 1 << obj.layer;
        return (layerMask.value & objLayerMask) > 0;
    }

    public static bool IsInLayerMask(LayerMask testMask, LayerMask layerMask)
    {
        // 根据Layer数值进行移位获得用于运算的Mask值
        int objLayerMask = 1 << testMask;
        return (layerMask.value & objLayerMask) > 0;
    }

    /// <summary>
    /// 简单拆解十进制数字
    /// 传入12345，传出[1,2,3,4,5]（列表）
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static List<int> SplitNumber(int number)
    {
        List<int> result = new List<int>();

        while (number > 0)
        {
            int digit = number % 10;
            result.Insert(0, digit); // 插入到列表的开头，保持顺序
            number /= 10;
        }

        return result;
    }

    /// <summary>
    /// 归val化
    /// </summary>
    /// <param name="val"></param>
    /// <param name="minVal"></param>
    /// <param name="maxVal"></param>
    /// <returns></returns>
    public static float Normalize(float val, float minVal, float maxVal)
    {
        return (val - minVal) / (maxVal - minVal);
    }

    /// <summary>
    /// 获取当前本地时间戳
    /// </summary>     
    public static long GetCurrentTimestamp()
    {
        long t = GetTimestampFromDateTime(System.DateTime.Now);
        return t;
    }
    public static long GetTimestampFromDateTime(System.DateTime dateTime)
    {
        System.TimeSpan ts = (dateTime - System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
        long t = (long)ts.TotalSeconds;
        return t;
    }


    /// <summary>
    /// RaycastHit2D[]去重
    /// 会排除rig为空的
    /// 不要在异步中使用
    /// </summary>
    static HashSet<Rigidbody2D> deduplication_RaycastHit2DHash;
    public static HashSet<Rigidbody2D> Deduplication_RaycastHit2D(RaycastHit2D[] array)
    {
        if (deduplication_RaycastHit2DHash == null)
        {
            deduplication_RaycastHit2DHash = new HashSet<Rigidbody2D>();
        }
        else
        {
            deduplication_RaycastHit2DHash.Clear();
        }

        foreach (var v in array)
        {
            if (v.rigidbody == null) continue;
            if (deduplication_RaycastHit2DHash.Contains(v.rigidbody)) continue;
            deduplication_RaycastHit2DHash.Add(v.rigidbody);
        }

        return deduplication_RaycastHit2DHash;
    }


    /// <summary>
    /// 数字显示转换
    /// </summary>
    public static string FormatNumber(float number)
    {
        if (number < 1000)
            return number.ToString("F1");

        string[] units = { "", "K", "M", "B", "T" };
        int unitIndex = 0;

        while (number >= 1000 && unitIndex < units.Length - 1)
        {
            number /= 1000;
            unitIndex++;
        }

        // 如果结果是整数，不显示小数点
        if (Mathf.Abs(number - Mathf.Floor(number)) < 0.1)
            return $"{Mathf.Floor(number)}{units[unitIndex]}";

        return $"{number:F1}{units[unitIndex]}";
    }

}

