using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Depth of Field (HDR, Scatter)")]
public class DepthOfFieldScatter : PostEffectsBase
{
	[Serializable]
	public enum BlurType
	{
		Poisson,
		Production,
		Movie
	}

	[Serializable]
	public enum BlurResolution
	{
		High,
		Low
	}

	public bool foregroundBlur;

	public bool visualizeFocus;

	public float focalPoint;

	public float smoothness;

	public float foregroundCurve;

	public float backgroundCurve;

	public Transform focalTransform;

	public float focalSize;

	public float apertureSize;

	public BlurType blurType;

	public BlurResolution blurResolution;

	public float foregroundOverlap;

	public Shader dofHdrShader;

	private float focalStartCurve;

	private float focalEndCurve;

	private float focalDistance01;

	private Material dofHdrMaterial;

	public DepthOfFieldScatter()
	{
		focalPoint = 1f;
		smoothness = 2.5f;
		foregroundCurve = 1f;
		backgroundCurve = 1f;
		apertureSize = 2.25f;
		blurType = BlurType.Production;
		blurResolution = BlurResolution.Low;
		foregroundOverlap = 0.85f;
		focalStartCurve = 2f;
		focalEndCurve = 2f;
		focalDistance01 = 0.1f;
	}

	public override bool CheckResources()
	{
		CheckSupport(true);
		dofHdrMaterial = CheckShaderAndCreateMaterial(dofHdrShader, dofHdrMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	public override void OnEnable()
	{
		camera.depthTextureMode |= DepthTextureMode.Depth;
	}

	public virtual float FocalDistance01(float worldDist)
	{
		return camera.WorldToViewportPoint((worldDist - camera.nearClipPlane) * camera.transform.forward + camera.transform.position).z / (camera.farClipPlane - camera.nearClipPlane);
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		int num = 0;
		float num2 = apertureSize;
		int num3 = ((blurResolution == BlurResolution.High) ? 1 : 2);
		if (!(smoothness >= 0.4f))
		{
			smoothness = 0.4f;
		}
		if (!(focalSize >= 1E-05f))
		{
			focalSize = 1E-05f;
		}
		if (!(foregroundCurve >= 0.01f))
		{
			foregroundCurve = 0f;
		}
		if (!(backgroundCurve >= 0.01f))
		{
			backgroundCurve = 0f;
		}
		float num4 = focalSize / (camera.farClipPlane - camera.nearClipPlane);
		focalDistance01 = ((!focalTransform) ? FocalDistance01(focalPoint) : (camera.WorldToViewportPoint(focalTransform.position).z / camera.farClipPlane));
		focalStartCurve = focalDistance01 * smoothness;
		focalEndCurve = focalStartCurve;
		bool flag = source.format == RenderTextureFormat.ARGBHalf;
		RenderTexture renderTexture = ((num3 <= 1) ? null : RenderTexture.GetTemporary(source.width / num3, source.height / num3, 0, source.format));
		RenderTexture temporary = RenderTexture.GetTemporary(source.width / (2 * num3), source.height / (2 * num3), 0, source.format);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / (2 * num3), source.height / (2 * num3), 0, source.format);
		temporary.filterMode = FilterMode.Bilinear;
		temporary2.filterMode = FilterMode.Bilinear;
		dofHdrMaterial.SetVector("_CurveParams", new Vector4(foregroundCurve / focalStartCurve, backgroundCurve / focalEndCurve, num4 * 0.5f, focalDistance01));
		dofHdrMaterial.SetVector("_InvRenderTargetSize", new Vector4(1f / (1f * (float)source.width), 1f / (1f * (float)source.height), 0f, 0f));
		if (foregroundBlur)
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / (2 * num3), source.height / (2 * num3), 0, source.format);
			Graphics.Blit(source, temporary3, dofHdrMaterial, 4);
			dofHdrMaterial.SetTexture("_FgOverlap", temporary3);
			float num5 = num2 * foregroundOverlap * 0.225f;
			dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, num5, 0f, num5));
			Graphics.Blit(temporary3, temporary2, dofHdrMaterial, 2);
			dofHdrMaterial.SetVector("_Offsets", new Vector4(num5, 0f, 0f, num5));
			Graphics.Blit(temporary2, temporary3, dofHdrMaterial, 2);
			Graphics.Blit(temporary3, source, dofHdrMaterial, 7);
			RenderTexture.ReleaseTemporary(temporary3);
		}
		else
		{
			dofHdrMaterial.SetTexture("_FgOverlap", null);
		}
		Graphics.Blit(source, source, dofHdrMaterial, foregroundBlur ? 3 : 0);
		RenderTexture renderTexture2 = source;
		if (num3 > 1)
		{
			Graphics.Blit(source, renderTexture, dofHdrMaterial, 6);
			renderTexture2 = renderTexture;
		}
		Graphics.Blit(renderTexture2, temporary2, dofHdrMaterial, 6);
		Graphics.Blit(temporary2, renderTexture2, dofHdrMaterial, 8);
		int pass = 10;
		switch (blurType)
		{
		case BlurType.Poisson:
			pass = ((num3 <= 1) ? 13 : 10);
			break;
		case BlurType.Production:
			pass = ((num3 <= 1) ? 11 : 12);
			break;
		case BlurType.Movie:
			pass = ((num3 <= 1) ? 14 : 15);
			break;
		default:
			Debug.Log("DOF couldn't find valid blur type", transform);
			break;
		}
		if (visualizeFocus)
		{
			Graphics.Blit(source, destination, dofHdrMaterial, 1);
		}
		else
		{
			dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, 0f, 0f, num2));
			dofHdrMaterial.SetTexture("_LowRez", renderTexture2);
			Graphics.Blit(source, destination, dofHdrMaterial, pass);
		}
		if ((bool)temporary)
		{
			RenderTexture.ReleaseTemporary(temporary);
		}
		if ((bool)temporary2)
		{
			RenderTexture.ReleaseTemporary(temporary2);
		}
		if ((bool)renderTexture)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	public override void Main()
	{
	}
}
