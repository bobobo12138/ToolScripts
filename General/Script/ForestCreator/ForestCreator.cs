using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public class ForestCreator : MonoBehaviour
{
    [Header("--- 区域设置 ---")]
    public List<Transform> boundaryPoints = new List<Transform>();

    [Header("--- 生成设置 ---")]
    public int totalTreeCount = 100;
    [Tooltip("树木之间的最小间距，防止穿模或过密")]
    public float minSpacing = 2.0f; // <--- 新增参数
    public float raycastHeight = 100f;
    public LayerMask groundLayer;

    [Header("--- 分布规则 ---")]
    public AnimationCurve densityCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
    public float noiseScale = 5f;

    [Header("--- 树木参数 ---")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.5f);
    public Vector2 rotationRange = new Vector2(0f, 360f);

    [System.Serializable]
    public class TreeType
    {
        public GameObject prefab;
        [Range(0, 100)] public float weight = 10f;
    }
    public List<TreeType> treeTypes = new List<TreeType>();

    private Transform _forestContainer;
    // 用于记录已生成树的位置，进行间距比对
    private List<Vector3> _spawnedPositions = new List<Vector3>();

    [Button]
    public void GenerateForest()
    {
        if (boundaryPoints.Count < 3) return;

        CleanUp();
        CreateContainer();
        _spawnedPositions.Clear();

        Rect bounds = GetPolygonBounds();
        Vector3 centroid = GetCentroid();
        float maxDistToCenter = GetMaxDistanceToCenter(centroid);

        int spawnedCount = 0;
        int safetyLoop = 0;
        // 适当增加尝试次数，因为间距限制会导致某些点采样失败
        int maxLoops = totalTreeCount * 50;

        while (spawnedCount < totalTreeCount && safetyLoop < maxLoops)
        {
            safetyLoop++;

            Vector2 randomPoint = new Vector2(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax)
            );

            if (IsPointInPolygon(randomPoint))
            {
                if (ShouldSpawnAt(randomPoint, centroid, maxDistToCenter))
                {
                    Vector3 rayOrigin = new Vector3(randomPoint.x, transform.position.y + raycastHeight, randomPoint.y);
                    if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundLayer))
                    {
                        // --- 关键修改：间距检查 ---
                        if (IsSpaceAvailable(hit.point))
                        {
                            SpawnTree(hit.point, hit.normal);
                            _spawnedPositions.Add(hit.point); // 记录位置
                            spawnedCount++;
                        }
                    }
                }
            }
        }
        Debug.Log($"生成完成！目标: {totalTreeCount}, 实际: {spawnedCount}。");
    }

    // 检查该位置周围是否有其他树
    private bool IsSpaceAvailable(Vector3 pos)
    {
        if (minSpacing <= 0) return true;

        // 这里使用简单的距离遍历。对于中/远景（几百到几千棵树），这种性能在编辑器下完全足够。
        // 如果树木上万，建议改用空间分区（如 Grid 或 QuadTree）
        for (int i = 0; i < _spawnedPositions.Count; i++)
        {
            if (Vector3.Distance(pos, _spawnedPositions[i]) < minSpacing)
            {
                return false; // 太近了，放弃这个点
            }
        }
        return true;
    }
    [Button]
    public void ClearForest()
    {
        CleanUp();
    }
    private void SpawnTree(Vector3 position, Vector3 groundNormal)
    {
        GameObject prefabToSpawn = GetRandomTreePrefab();
        if (prefabToSpawn == null) return;

        GameObject tree = Instantiate(prefabToSpawn, position, Quaternion.identity, _forestContainer);
        float randomScale = Random.Range(scaleRange.x, scaleRange.y);
        tree.transform.localScale = Vector3.one * randomScale;
        float randomRotY = Random.Range(rotationRange.x, rotationRange.y);
        tree.transform.rotation = Quaternion.Euler(0, randomRotY, 0);
    }

    private GameObject GetRandomTreePrefab()
    {
        float totalWeight = treeTypes.Sum(t => t.weight);
        if (totalWeight <= 0) return null;
        float randomValue = Random.Range(0, totalWeight);
        float currentSum = 0;
        foreach (var tree in treeTypes)
        {
            currentSum += tree.weight;
            if (randomValue <= currentSum) return tree.prefab;
        }
        return treeTypes[0].prefab;
    }

    private bool ShouldSpawnAt(Vector2 point, Vector3 centroid3D, float maxDist)
    {
        Vector2 center2D = new Vector2(centroid3D.x, centroid3D.z);
        float dist = Vector2.Distance(point, center2D);
        float normalizedDist = Mathf.Clamp01(dist / maxDist);
        float curveProbability = densityCurve.Evaluate(normalizedDist);

        float noiseVal = 0;
        if (noiseScale > 0)
        {
            float perlin = Mathf.PerlinNoise(point.x * 0.1f * noiseScale, point.y * 0.1f * noiseScale);
            noiseVal = (perlin - 0.5f) * 0.4f;
        }

        return Random.value < Mathf.Clamp01(curveProbability + noiseVal);
    }

    private bool IsPointInPolygon(Vector2 point)
    {
        bool inside = false;
        int j = boundaryPoints.Count - 1;
        for (int i = 0; i < boundaryPoints.Count; i++)
        {
            if (((boundaryPoints[i].position.z > point.y) != (boundaryPoints[j].position.z > point.y)) &&
                (point.x < (boundaryPoints[j].position.x - boundaryPoints[i].position.x) * (point.y - boundaryPoints[i].position.z) / (boundaryPoints[j].position.z - boundaryPoints[i].position.z) + boundaryPoints[i].position.x))
            {
                inside = !inside;
            }
            j = i;
        }
        return inside;
    }

    private Rect GetPolygonBounds()
    {
        float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
        foreach (var pt in boundaryPoints)
        {
            minX = Mathf.Min(minX, pt.position.x); maxX = Mathf.Max(maxX, pt.position.x);
            minZ = Mathf.Min(minZ, pt.position.z); maxZ = Mathf.Max(maxZ, pt.position.z);
        }
        return new Rect(minX, minZ, maxX - minX, maxZ - minZ);
    }

    private Vector3 GetCentroid()
    {
        Vector3 center = Vector3.zero;
        foreach (var pt in boundaryPoints) center += pt.position;
        return center / boundaryPoints.Count;
    }

    private float GetMaxDistanceToCenter(Vector3 center)
    {
        float maxDist = 0f;
        foreach (var pt in boundaryPoints) maxDist = Mathf.Max(maxDist, Vector3.Distance(center, pt.position));
        return maxDist;
    }

    private void CreateContainer()
    {
        _forestContainer = new GameObject("Generated_Forest").transform;
        _forestContainer.SetParent(this.transform);
    }

    private void CleanUp()
    {
        Transform old = transform.Find("Generated_Forest");
        if (old != null) DestroyImmediate(old.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (boundaryPoints == null || boundaryPoints.Count < 2) return;
        Gizmos.color = Color.green;
        for (int i = 0; i < boundaryPoints.Count; i++)
        {
            Gizmos.DrawLine(boundaryPoints[i].position, boundaryPoints[(i + 1) % boundaryPoints.Count].position);
            Gizmos.DrawWireSphere(boundaryPoints[i].position, 0.5f);
        }
    }


}