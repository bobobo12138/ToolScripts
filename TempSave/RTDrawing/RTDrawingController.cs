using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering; // ��Ҫ������������ռ�

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
        // 1. ����RenderTexture
        renderTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        // ��RTӦ�õ�һ��UI Image�򳡾��е��������Թ�Ԥ��
        GetComponent<Renderer>().material.mainTexture = renderTexture;

        // 2. ��RT���Ϊ��ɫ (ʹ��CommandBuffer���ɿ�)
        ClearRenderTexture();

        // 3. ����һ���򵥵�Quad Mesh
        CreateQuadMesh();

        // 4. ����CommandBuffer
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

    // ���ǲ���ʹ��LateUpdate������ÿ֡���¹���CommandBuffer
    void LateUpdate()
    {
        if (pointsToDraw.Count == 0)
        {
            // ���û�е�Ҫ�������Բ����CommandBuffer�����������
            // �������գ���һ֡�Ļ���������ܻᱻ�ٴ�ִ�У���ȡ���ھ�������
            // ��������ѡ����գ�ȷ��ÿ֡�����µ�ָ��
            commandBuffer.Clear();
            return;
        }

        // 1. �����һ֡������
        commandBuffer.Clear();

        // 2. ������ȾĿ��
        commandBuffer.SetRenderTarget(renderTexture);

        // 3. ���û����õ�����ͶӰ����
        // CommandBufferû��GL.LoadPixelMatrix�������ǿ�������һ����������
        // ���������Ļ�ռ�����(-1 to 1)ӳ�䵽RT��
        Matrix4x4 projectionMatrix = Matrix4x4.Ortho(0, renderTexture.width, 0, renderTexture.height, -1, 100);
        commandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, projectionMatrix);

        // 4. ¼�ƻ���ÿ���������
        foreach (var point in pointsToDraw)
        {
            // ע�⣺�����Y���������Ҫ��ת��ȡ�����������ϵϰ��
            // GL.LoadPixelMatrix��(0,0)�����Ͻǣ�����׼Ortho��(0,0)�����½�
            // �������Ǳ������½�Ϊ(0,0)
            //float rtX = point.x * (renderTexture.width / (float)Screen.width);
            //float rtY = point.y * (renderTexture.height / (float)Screen.height);

            float rtX = point.x;
            float rtY = point.y;

            Vector3 pos = new Vector3(rtX, rtY, 0);
            Vector3 scale = new Vector3(brushSize, brushSize, 1);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.identity, scale);

            // ʹ�� CommandBuffer.DrawMesh
            commandBuffer.DrawMesh(quadMesh, matrix, brushMaterial);
        }

        // 5. ��յ��б�Ϊ��һ֡��׼��
        pointsToDraw.Clear();

        Graphics.ExecuteCommandBuffer(commandBuffer);
        // ע�⣺����ֻ��LateUpdate�й����������б�
        // ������ִ�����ɸ��ӵ�����ϵ��¼��������ġ�
        // �����Ҫ����ִ�У�����ʹ�� Graphics.ExecuteCommandBuffer(commandBuffer);
        // ������������Ǹ���׼��������
    }

    private void ClearRenderTexture()
    {
        CommandBuffer cmd = new CommandBuffer();
        cmd.SetRenderTarget(renderTexture);
        cmd.ClearRenderTarget(true, true, Color.white);
        Graphics.ExecuteCommandBuffer(cmd); // ����ִ���������
        cmd.Release();
    }

    // OnDestroyʱ������Դ
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

    // CreateQuadMesh() �������ֲ���
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



    // �����屻ѡ��ʱ���� Scene ��ͼ�л���һ���߿����
    private void OnDrawGizmosSelected()
    {
        // ���� Gizmo ����ɫ
        Gizmos.color = new Color(1, 1, 0, 0.75F); // ��ɫ

        // ����һ���߿�������
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}