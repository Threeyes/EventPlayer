#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering;

namespace Threeyes.Editor
{
    /// <summary>
    /// 根据当前RenderingPipeline，添加对应宏定义：
    /// URP：UNITY_PIPELINE_URP
    /// HDRP：UNITY_PIPELINE_HDRP
    /// 
    /// https://gist.github.com/cjaube/944b0d5221808c2a761d616f29deaf49
    /// </summary>
    [InitializeOnLoad]
    public class DefineSymbolManager_RenderingPipeline
    {
        enum PipelineType
        {
            Unsupported,
            BuiltInPipeline,
            URPipeline,
            HDRPipeline
        }

        static DefineSymbolManager_RenderingPipeline()
        {
            UpdateDefines();
        }

        /// <summary>
        /// Update the unity pipeline defines for URP
        /// </summary>
        static void UpdateDefines()
        {
            List<string> listDefineToAdd = new List<string>();
            List<string> listDefineToRemove = new List<string>();

            var pipeline = GetPipeline();
            if (pipeline == PipelineType.URPipeline)
            {
                listDefineToAdd.Add("UNITY_PIPELINE_URP");
            }
            else
            {
                listDefineToRemove.Add("UNITY_PIPELINE_URP");
            }
            if (pipeline == PipelineType.HDRPipeline)
            {
                listDefineToAdd.Add("UNITY_PIPELINE_HDRP");
            }
            else
            {
                listDefineToRemove.Add("UNITY_PIPELINE_HDRP");
            }

            EditorDefineSymbolTool.ModifyDefines(listDefineToAdd, listDefineToRemove);
        }


        /// <summary>
        /// Returns the type of renderpipeline that is currently running
        /// </summary>
        /// <returns></returns>
        static PipelineType GetPipeline()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                // SRP
                var srpType = GraphicsSettings.renderPipelineAsset.GetType().ToString();
                if (srpType.Contains("HDRenderPipelineAsset"))
                {
                    return PipelineType.HDRPipeline;
                }
                else if (srpType.Contains("UniversalRenderPipelineAsset") || srpType.Contains("LightweightRenderPipelineAsset"))
                {
                    return PipelineType.URPipeline;
                }
                else return PipelineType.Unsupported;
            }
#elif UNITY_2017_1_OR_NEWER
        if (GraphicsSettings.renderPipelineAsset != null) {
            // SRP not supported before 2019
            return PipelineType.Unsupported;
        }
#endif
            // no SRP
            return PipelineType.BuiltInPipeline;
        }

    }
}
#endif