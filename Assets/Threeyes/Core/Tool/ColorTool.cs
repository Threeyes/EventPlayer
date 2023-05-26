using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorTool
{

	// specifies the max byte value to use when decomposing a float color into bytes with exposure
	// this is the value used by Photoshop
	private const byte k_MaxByteForOverexposedColor = 191;

	/// <summary>
	/// 从HDR Color中分离出baseColor以及exposure
	///
	/// Ref：https://github.com/Unity-Technologies/UnityCsReference/blob/11bcfd801fccd2a52b09bb6fd636c1ddcc9f1705/Editor/Mono/GUI/ColorMutator.cs
	/// </summary>
	/// <param name="linearColorHdr"></param>
	/// <param name="baseLinearColor"></param>
	/// <param name="exposure"></param>
	public static void DecomposeHdrColor(Color linearColorHdr, out Color32 baseLinearColor, out float exposure)
	{
		baseLinearColor = linearColorHdr;
		var maxColorComponent = linearColorHdr.maxColorComponent;
		// replicate Photoshops's decomposition behaviour
		if (maxColorComponent == 0f || maxColorComponent <= 1f && maxColorComponent > 1 / 255f)
		{
			exposure = 0f;

			baseLinearColor.r = (byte)Mathf.RoundToInt(linearColorHdr.r * 255f);
			baseLinearColor.g = (byte)Mathf.RoundToInt(linearColorHdr.g * 255f);
			baseLinearColor.b = (byte)Mathf.RoundToInt(linearColorHdr.b * 255f);
		}
		else
		{
			// calibrate exposure to the max float color component
			var scaleFactor = k_MaxByteForOverexposedColor / maxColorComponent;
			exposure = Mathf.Log(255f / scaleFactor) / Mathf.Log(2f);

			// maintain maximal integrity of byte values to prevent off-by-one errors when scaling up a color one component at a time
			baseLinearColor.r = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * linearColorHdr.r));
			baseLinearColor.g = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * linearColorHdr.g));
			baseLinearColor.b = Math.Min(k_MaxByteForOverexposedColor, (byte)Mathf.CeilToInt(scaleFactor * linearColorHdr.b));
		}
	}

	/// <summary>
	/// 将Color及exposure组合成HDR Color
	/// </summary>
	/// <param name="baseLinearColor"></param>
	/// <param name="exposure"></param>
	/// <returns></returns>
	public static Color ComposeHdrColor(Color32 baseLinearColor, float exposure)
	{
		Color tempBaseLinearColor = baseLinearColor;
		float alpha = tempBaseLinearColor.a;//Cache alpha
		float factor = Mathf.Pow(2, exposure);
		tempBaseLinearColor *= factor;//https://forum.unity.com/threads/how-to-change-hdr-colors-intensity-via-shader.531861/ #7
		tempBaseLinearColor.a = alpha;//Restore alpha
		return tempBaseLinearColor;
	}
}
