using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering; // 需要引入这个命名空间

public class RTDrawingController : MonoBehaviour
{
    public Material brushMaterial;
    public float brushSize = 30f;
    public Vector3 areaSize = new Vector3(10, 2, 10);

    [SerializeField]
    private List<GameObject> painter = new List<GameObject>();

    private RenderTexture renderTexture;
    private Mesh quadMesh;
    private List<Vector2> pointsToDraw = new List<Vector2>();

    private CommandBuffer commandBuffer;

    void Start()
    {
        // 1. 创建RenderTexture
        renderTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        // 将RT应用到一个UI Image或场景中的物体上以供预览
        GetComponent<Renderer>().material.mainTexture = renderTexture;

        // 2. 将RT清空为白色 (使用CommandBuffer更可靠)
        ClearRenderTexture();

        // 3. 创建一个简单的Quad Mesh
        CreateQuadMesh();

        // 4. 设置CommandBuffer
        commandBuffer = new CommandBuffer();
        commandBuffer.name = "RT Drawing Buffer";
    }

    void Update()
    {
        foreach (var v in painter)
        {
            if (v != null)
            {
                var pos = v.transform.position - transform.position;
                pos += areaSize / 2f;
                if ((pos.x > 0 && pos.x < areaSize.x) && (pos.z > 0 && pos.z < areaSize.z))
                {
                    pointsToDraw.Add(new Vector2((pos.x / areaSize.x) * renderTexture.width, (pos.z / areaSize.z) * renderTexture.height));
                }
            }
        }
    }

    // 我们不再使用LateUpdate，而是每帧重新构建CommandBuffer
    void LateUpdate()
    {
        if (pointsToDraw.Count == 0)
        {
            // 如果没有点要画，可以不清空CommandBuffer，或者清空它
            // 如果不清空，上一帧的绘制命令可能会被再次执行，这取决于具体需求
            // 这里我们选择清空，确保每帧都是新的指令
            commandBuffer.Clear();
            return;
        }

        // 1. 清空上一帧的命令
        commandBuffer.Clear();

        // 2. 设置渲染目标
        commandBuffer.SetRenderTarget(renderTexture);

        // 3. 设置绘制用的正交投影矩阵
        // CommandBuffer没有GL.LoadPixelMatrix，但我们可以设置一个正交矩阵
        // 这个矩阵将屏幕空间坐标(-1 to 1)映射到RT上
        Matrix4x4 projectionMatrix = Matrix4x4.Ortho(0, renderTexture.width, 0, renderTexture.height, -1, 100);
        commandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, projectionMatrix);

        // 4. 录制绘制每个点的命令
        foreach (var point in pointsToDraw)
        {
            // 注意：这里的Y坐标可能需要翻转，取决于你的坐标系习惯
            // GL.LoadPixelMatrix的(0,0)在左上角，而标准Ortho的(0,0)在左下角
            // 这里我们保持左下角为(0,0)
            //float rtX = point.x * (renderTexture.width / (float)Screen.width);
            //float rtY = point.y * (renderTexture.height / (float)Screen.height);

            float rtX = point.x;
            float rtY = point.y;

            Vector3 pos = new Vector3(rtX, rtY, 0);
            Vector3 scale = new Vector3(brushSize, brushSize, 1);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.identity, scale);

            // 使用 CommandBuffer.DrawMesh
            commandBuffer.DrawMesh(quadMesh, matrix, brushMaterial);
        }

        // 5. 清空点列表，为下一帧做准备
        pointsToDraw.Clear();

        Graphics.ExecuteCommandBuffer(commandBuffer);
        // 注意：我们只在LateUpdate中构建了命令列表，
        // 真正的执行是由附加到相机上的事件来触发的。
        // 如果需要立即执行，可以使用 Graphics.ExecuteCommandBuffer(commandBuffer);
        // 但挂在相机上是更标准的做法。
    }

    private void ClearRenderTexture()
    {
        CommandBuffer cmd = new CommandBuffer();
        cmd.SetRenderTarget(renderTexture);
        cmd.ClearRenderTarget(true, true, Color.white);
        Graphics.ExecuteCommandBuffer(cmd); // 立即执行清空命令
        cmd.Release();
    }

    // OnDestroy时清理资源
    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
        if (commandBuffer != null)
        {
            commandBuffer.Release();
        }
    }

    // CreateQuadMesh() 方法保持不变
    private void CreateQuadMesh()
    {
        quadMesh = new Mesh();
        Vector3[] vertices = { new Vector3(-0.5f, -0.5f, 0), new Vector3(0.5f, -0.5f, 0), new Vector3(-0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 0) };
        Vector2[] uv = { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        int[] triangles = { 0, 2, 1, 2, 3, 1 };
        quadMesh.vertices = vertices;
        quadMesh.uv = uv;
        quadMesh.triangles = triangles;
        quadMesh.RecalculateNormals();
    }



    // 当物体被选中时，在 Scene 视图中绘制一个线框盒子
    private void OnDrawGizmosSelected()
    {
        // 设置 Gizmo 的颜色
        Gizmos.color = new Color(1, 1, 0, 0.75F); // 黄色

        // 绘制一个线框立方体
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}