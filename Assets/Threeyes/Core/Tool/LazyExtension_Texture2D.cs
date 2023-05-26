using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LazyExtension_Texture2D
{

    /// <summary>
    /// Try Compress the texture2D with error proof
    /// 
    /// Ref：https://docs.unity3d.com/ScriptReference/Texture2D.Compress.html
    /// </summary>
    /// <param name="textureFrame"></param>
    /// <param name="highQuality">Passing true for highQuality parameter will dither the source texture during compression, which helps to reduce compression artifacts but is slightly slower. This parameter is ignored for ETC compression.</param>
    /// <returns></returns>
    public static bool TryCompress(this Texture2D textureFrame, bool highQuality)
    {
        if (!textureFrame)
            return false;
       
        bool isValid = false;

        ///实现原理：
        ///Compress: If the graphics card does not support compression or the texture is already in compressed format, then Compress does nothing.
        ///【PC】: 
        ///——DXT1(BC1) format if the original texture had no alpha channel, 
        ///——DXT5(BC3) format if it had alpha channel.
        ///——DXT5(BC4) format if the original texture was R8.
        ///——DXT5(BC5) format if the original texture was RG16.
        ///【Android】, 【iOS】and 【tvOS】: Compress the texture to the ETC/EAC family of formats.

        if (textureFrame.width > 4 && textureFrame.height > 4)//忽略创建的空白图
        {
            ///判断不同平台下是否符合压缩规则，避免无法catch的报错（因为不在该线程执行，因此错误不能被catch）：
            ///1.部分平台的压缩格式需要长宽为4的倍数，否则报错：Texture '' has dimensions (218 x 218) which are not multiples of 4. Compress will not work.
            if (textureFrame.width % 4 == 0 && textureFrame.height % 4 == 0)
            {
                isValid = true;
            }
        }

        ///Compress require texture readable
        if (isValid && textureFrame.isReadable)
            textureFrame.Compress(highQuality);

        return isValid;
    }
}
