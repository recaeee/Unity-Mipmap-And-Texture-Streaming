using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TextureStreamingManager : MonoBehaviour
{
    public Text displayText;
    [Header("wlop")] public Texture2D wlopTex;
    // [Header("wlop1")] public Texture2D wlop1Tex;
    [Header("wlop2")] public Texture2D wlop2Tex;
    [Header("Streaming Contorller")] public StreamingController streamingController;
    public GameObject GO;
    public GameObject GO1;
    [Header("纹理串流设置")] public int textureMemoryBudget = 512;
    public int maxLevelReduction = 2;

    [Header("Debug设置")] public Shader mipmapDebugShader;

    private StringBuilder stringBuilder;
    private GameObject instanceGO1;

    public StringBuilder StringBuilder
    {
        get
        {
            if (stringBuilder == null)
            {
                stringBuilder = new StringBuilder();
            }

            return stringBuilder;
        }
    }
    private void Start()
    {
        QualitySettings.streamingMipmapsActive = true;
#if UNITY_EDITOR
        QualitySettings.streamingMipmapsMemoryBudget = 10;
#else
        QualitySettings.streamingMipmapsMemoryBudget = textureMemoryBudget;
#endif
        QualitySettings.streamingMipmapsMaxLevelReduction = maxLevelReduction;
        QualitySettings.streamingMipmapsAddAllCameras = false;
    }

    private void Update()
    {
        Display();
        ShowDebugColor();
    }

    private void Display()
    {
        StringBuilder.Clear();
        StringBuilder.AppendLine("纹理串流设置:");
        StringBuilder.AppendLine("Budget:" + QualitySettings.streamingMipmapsMemoryBudget + " MB");
        StringBuilder.AppendLine("MaxLevelReduction:" + QualitySettings.streamingMipmapsMaxLevelReduction);
        StringBuilder.AppendLine("纹理属性:");
        StringBuilder.AppendLine("当前wlop纹理加载的Mip等级（串流系统）:" + wlopTex.loadedMipmapLevel);
        // StringBuilder.AppendLine("当前wlop1纹理加载的Mip等级（串流系统）:" + wlop1Tex.loadedMipmapLevel);
        StringBuilder.AppendLine("当前wlop2纹理加载的Mip等级（串流系统）:" + wlop2Tex.loadedMipmapLevel);
        StringBuilder.AppendLine("当前wlop2纹理自身的MipmapBias:" + wlop2Tex.mipMapBias);
        StringBuilder.AppendLine("运行时参数:");
        StringBuilder.AppendLine("总纹理内存:" + Texture.currentTextureMemory/1024f/1024f +" MB");
        StringBuilder.AppendLine("是否丢弃未使用Mip:" + Texture.streamingTextureDiscardUnusedMips);
        StringBuilder.AppendLine("MasterTextureLimit:" + QualitySettings.masterTextureLimit);
        StringBuilder.AppendLine("摄像机参数:");
        StringBuilder.AppendLine("Streaming Controller Mipmap Bias:" + streamingController.streamingMipmapBias);
        displayText.text = StringBuilder.ToString();
    }

    private void ShowDebugColor()
    {
        //更改所有材质
        Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
        List<Material> materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            if (!materials.Contains(renderer.material))
            {
                materials.Add(renderer.material);
            }
        }

        foreach (var material in materials)
        {
            material.shader = mipmapDebugShader;
        }
        Texture.SetStreamingTextureMaterialDebugProperties();
    }

    public void SetBudget(bool increase)
    {
        if (increase)
        {
            QualitySettings.streamingMipmapsMemoryBudget++;
        }
        else
        {
            QualitySettings.streamingMipmapsMemoryBudget--;
        }
    }

    public void SetMaxLevelReduction(bool increase)
    {
        if (increase)
        {
            QualitySettings.streamingMipmapsMaxLevelReduction++;
        }
        else
        {
            QualitySettings.streamingMipmapsMaxLevelReduction--;
        }
    }

    public void SetDiscardUnusedMips()
    {
        Texture.streamingTextureDiscardUnusedMips = !Texture.streamingTextureDiscardUnusedMips;
    }

    public void SetGODistance(bool far)
    {
        Transform transform = GO.GetComponent<Transform>();
        if (far)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
        }
    }

    public void InstantiateOrDestroyGO1()
    {
        if (instanceGO1 == null)
        {
            instanceGO1 = GameObject.Instantiate(GO1);
        }
        else
        {
            GameObject.Destroy(instanceGO1);
        }
    }

    public void SetMasterTextureLimit(bool increase)
    {
        if (increase)
        {
            QualitySettings.masterTextureLimit++;
        }
        else
        {
            QualitySettings.masterTextureLimit--;
        }
    }

    public void SetTextureMipmapBias(bool increase)
    {
        if (increase)
        {
            wlop2Tex.mipMapBias++;
        }
        else
        {
            wlop2Tex.mipMapBias--;
        }
    }

    public void SetCameraMipmapBias(bool increase)
    {
        if (increase)
        {
            streamingController.streamingMipmapBias++;
        }
        else
        {
            streamingController.streamingMipmapBias--;
        }
    }
}
