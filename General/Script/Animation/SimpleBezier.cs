using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SimpleBezier : MonoBehaviour
{
    [Header("节点")]
    public List<Transform> points = new List<Transform>();
    [Header("显示采样")]
    [Tooltip("显示采样，显示精细度，不影响实际游戏表现")]
    public int drawGizmosSampling = 50;

    [SerializeField]
    [Header("update中自动遍历childs作为节点，方便查看，请勿在实际游戏中打开")]
    bool isAutoForeachChilds = false;

    private void Update()
    {
        if (!isAutoForeachChilds) return;
        points.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            points.Add(transform.GetChild(i));
        }
    }

    void OnDrawGizmos()
    {
        if (!isAutoForeachChilds) return;
        if (Application.isPlaying) return;
        if (points == null) return;
        if (points.Count == 0) return;

        List<Vector3> linePoints = new List<Vector3>();
        for (float i = 0; i < drawGizmosSampling; i++)
        {
            linePoints.Add(Beziers(points, i / 50));
        }

        for (int i = 0; i < linePoints.Count - 1; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(linePoints[i], linePoints[i + 1]);
        }
    }


    /// <summary>
    /// 获得坐标
    /// t范围是(0-1)假设贝塞尔从头到尾是0-1，t则代表贝塞尔上某一点的位置
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetVector(float t)
    {
        return Beziers(points, t);
    }




    #region 静态
    public static Vector3 Beziers(List<Vector3> ps, float t)
    {
        if (ps.Count < 2) return ps[0];
        List<Vector3> temps = new List<Vector3>();
        for (int i = 0; i < ps.Count - 1; i++)
        {
            temps.Add((1 - t) * ps[i] + t * ps[i + 1]);
            //temps.Add((ps[i + 1] - ps[i]) * (t / 1));
        }
        return Beziers(temps, t);
    }

    public static Vector3 Beziers(List<Transform> ps, float t)
    {
        if (ps.Count < 2) return ps[0].position;
        List<Vector3> temps = new List<Vector3>();
        for (int i = 0; i < ps.Count - 1; i++)
        {
            temps.Add((1 - t) * ps[i].position + t * ps[i + 1].position);
            //temps.Add((ps[i + 1] - ps[i]) * (t / 1));
        }
        return Beziers(temps, t);
    }
    #endregion
}
