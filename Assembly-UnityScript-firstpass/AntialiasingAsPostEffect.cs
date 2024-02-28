using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Antialiasing (Image based)")]
[ExecuteInEditMode]
public class AntialiasingAsPostEffect : PostEffectsBase
{
	public AAMode mode;

	public bool showGeneratedNormals;

	public float offsetScale;

	public float blurRadius;

	public float edgeThresholdMin;

	public float edgeThreshold;

	public float edgeSharpness;

	public bool dlaaSharp;

	public Shader ssaaShader;

	private Material ssaa;

	public Shader dlaaShader;

	private Material dlaa;

	public Shader nfaaShader;

	private Material nfaa;

	public Shader shaderFXAAPreset2;

	private Material materialFXAAPreset2;

	public Shader shaderFXAAPreset3;

	private Material materialFXAAPreset3;

	public Shader shaderFXAAII;

	private Material materialFXAAII;

	public Shader shaderFXAAIII;

	private Material materialFXAAIII;

	public AntialiasingAsPostEffect()
	{
		mode = AAMode.FXAA3Console;
		offsetScale = 0.2f;
		blurRadius = 18f;
		edgeThresholdMin = 0.05f;
		edgeThreshold = 0.2f;
		edgeSharpness = 4f;
	}

	public virtual Material CurrentAAMaterial()
	{
		Material material = null;
		switch (mode)
		{
		case AAMode.FXAA3Console:
			return materialFXAAIII;
		case AAMode.FXAA2:
			return materialFXAAII;
		case AAMode.FXAA1PresetA:
			return materialFXAAPreset2;
		case AAMode.FXAA1PresetB:
			return materialFXAAPreset3;
		case AAMode.NFAA:
			return nfaa;
		case AAMode.SSAA:
			return ssaa;
		case AAMode.DLAA:
			return dlaa;
		default:
			return null;
		}
	}

	public override bool CheckResources()
	{
		CheckSupport(false);
		materialFXAAPreset2 = CreateMaterial(shaderFXAAPreset2, materialFXAAPreset2);
		materialFXAAPreset3 = CreateMaterial(shaderFXAAPreset3, materialFXAAPreset3);
		materialFXAAII = CreateMaterial(shaderFXAAII, materialFXAAII);
		materialFXAAIII = CreateMaterial(shaderFXAAIII, materialFXAAIII);
		nfaa = CreateMaterial(nfaaShader, nfaa);
		ssaa = CreateMaterial(ssaaShader, ssaa);
		dlaa = CreateMaterial(dlaaShader, dlaa);
		if (!ssaaShader.isSupported)
		{
			NotSupported();
			ReportAutoDisable();
		}
		return isSupported;
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
		}
		else if (mode == AAMode.FXAA3Console && materialFXAAIII != null)
		{
			materialFXAAIII.SetFloat("_EdgeThresholdMin", edgeThresholdMin);
			materialFXAAIII.SetFloat("_EdgeThreshold", edgeThreshold);
			materialFXAAIII.SetFloat("_EdgeSharpness", edgeSharpness);
			Graphics.Blit(source, destination, materialFXAAIII);
		}
		else if (mode == AAMode.FXAA1PresetB && materialFXAAPreset3 != null)
		{
			Graphics.Blit(source, destination, materialFXAAPreset3);
		}
		else if (mode == AAMode.FXAA1PresetA && materialFXAAPreset2 != null)
		{
			source.anisoLevel = 4;
			Graphics.Blit(source, destination, materialFXAAPreset2);
			source.anisoLevel = 0;
		}
		else if (mode == AAMode.FXAA2 && materialFXAAII != null)
		{
			Graphics.Blit(source, destination, materialFXAAII);
		}
		else if (mode == AAMode.SSAA && ssaa != null)
		{
			Graphics.Blit(source, destination, ssaa);
		}
		else if (mode == AAMode.DLAA && dlaa != null)
		{
			source.anisoLevel = 0;
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(source, temporary, dlaa, 0);
			Graphics.Blit(temporary, destination, dlaa, (!dlaaSharp) ? 1 : 2);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (mode == AAMode.NFAA && nfaa != null)
		{
			source.anisoLevel = 0;
			nfaa.SetFloat("_OffsetScale", offsetScale);
			nfaa.SetFloat("_BlurRadius", blurRadius);
			Graphics.Blit(source, destination, nfaa, showGeneratedNormals ? 1 : 0);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}
}
