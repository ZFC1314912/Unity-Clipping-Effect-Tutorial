using UnityEngine;

[ExecuteInEditMode]
public class ClipController : MonoBehaviour
{
    public Transform bottomCube; // 立方体A的Transform
    private Material[] originalMaterials; // 原始材质
    private Material[] clipMaterials; // 裁剪材质
    private Renderer targetRenderer; // 角色模型的Renderer

    void Start()
    {
        InitializeMaterials(); // 初始化材质
        CreateClipMaterialInstances(); // 创建裁剪材质实例
    }

    void Update()
    {
        if (bottomCube == null || clipMaterials == null) return;

        // 计算裁剪高度
        float cubeHalfHeight = bottomCube.localScale.y * 0.5f;
        float cutoffHeight = bottomCube.position.y + cubeHalfHeight;

        // 更新材质属性
        UpdateMaterialProperties(cutoffHeight);
    }

    void InitializeMaterials()
    {
        // 获取角色模型的Renderer
        targetRenderer = GetComponent<SkinnedMeshRenderer>() as Renderer ?? GetComponent<MeshRenderer>();

        if (targetRenderer != null)
        {
            // 保存原始材质
            originalMaterials = targetRenderer.sharedMaterials;
        }
    }

    void CreateClipMaterialInstances()
    {
        if (targetRenderer == null) return;

        // 创建裁剪材质实例
        clipMaterials = new Material[originalMaterials.Length];

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            // 创建新的材质实例，并应用自定义Shader
            clipMaterials[i] = new Material(originalMaterials[i])
            {
                shader = Shader.Find("Custom/ClipTextureShader")
            };
        }

        // 应用裁剪材质
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        targetRenderer.materials = clipMaterials;
    }

    void UpdateMaterialProperties(float cutoffHeight)
    {
        // 更新材质属性
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
        // 恢复原始材质
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