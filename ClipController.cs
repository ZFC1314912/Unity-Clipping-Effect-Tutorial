using UnityEngine;

[ExecuteInEditMode]
public class ClipController : MonoBehaviour
{
    public Transform bottomCube; // ������A��Transform
    private Material[] originalMaterials; // ԭʼ����
    private Material[] clipMaterials; // �ü�����
    private Renderer targetRenderer; // ��ɫģ�͵�Renderer

    void Start()
    {
        InitializeMaterials(); // ��ʼ������
        CreateClipMaterialInstances(); // �����ü�����ʵ��
    }

    void Update()
    {
        if (bottomCube == null || clipMaterials == null) return;

        // ����ü��߶�
        float cubeHalfHeight = bottomCube.localScale.y * 0.5f;
        float cutoffHeight = bottomCube.position.y + cubeHalfHeight;

        // ���²�������
        UpdateMaterialProperties(cutoffHeight);
    }

    void InitializeMaterials()
    {
        // ��ȡ��ɫģ�͵�Renderer
        targetRenderer = GetComponent<SkinnedMeshRenderer>() as Renderer ?? GetComponent<MeshRenderer>();

        if (targetRenderer != null)
        {
            // ����ԭʼ����
            originalMaterials = targetRenderer.sharedMaterials;
        }
    }

    void CreateClipMaterialInstances()
    {
        if (targetRenderer == null) return;

        // �����ü�����ʵ��
        clipMaterials = new Material[originalMaterials.Length];

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            // �����µĲ���ʵ������Ӧ���Զ���Shader
            clipMaterials[i] = new Material(originalMaterials[i])
            {
                shader = Shader.Find("Custom/ClipTextureShader")
            };
        }

        // Ӧ�òü�����
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        targetRenderer.materials = clipMaterials;
    }

    void UpdateMaterialProperties(float cutoffHeight)
    {
        // ���²�������
        var propertyBlock = new MaterialPropertyBlock();

        for (int i = 0; i < clipMaterials.Length; i++)
        {
            targetRenderer.GetPropertyBlock(propertyBlock, i);
            propertyBlock.SetFloat("_CutoffHeight", cutoffHeight);
            targetRenderer.SetPropertyBlock(propertyBlock, i);
        }
    }

    void OnDestroy()
    {
        // �ָ�ԭʼ����
        if (targetRenderer != null && originalMaterials != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                targetRenderer.sharedMaterials = originalMaterials;
            }
            else
#endif
            {
                targetRenderer.materials = originalMaterials;
            }
        }
    }
}