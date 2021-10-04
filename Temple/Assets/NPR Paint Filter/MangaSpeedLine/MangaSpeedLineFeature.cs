using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NprPaintFilter
{
	public class MangaSpeedLineFeature : ScriptableRendererFeature
	{
		public class Pass : ScriptableRenderPass
		{
			Material m_Mat;
			FilterMode filterMode { get; set; }
			RenderTargetIdentifier source { get; set; }
			RenderTargetHandle m_TempColorTexture;
			string m_ProfilerTag;

			public Pass(string tag)
			{
				this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
				m_ProfilerTag = tag;
				m_TempColorTexture.Init("_TemporaryColorTexture");
			}
			public void Setup(RenderTargetIdentifier source, Material mat)
			{
				this.source = source;
				m_Mat = mat;
			}
			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
	
				RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
				opaqueDesc.depthBufferBits = 0;
				cmd.GetTemporaryRT(m_TempColorTexture.id, opaqueDesc, filterMode);

				Blit(cmd, source, m_TempColorTexture.Identifier(), m_Mat, 0);
				Blit(cmd, m_TempColorTexture.Identifier(), source);

				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}
			public override void FrameCleanup(CommandBuffer cmd)
			{
				cmd.ReleaseTemporaryRT(m_TempColorTexture.id);
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable]
		public class Settings
		{
			[Header("Basic")]
			public Material m_Mat;
			[HideInInspector] public float m_CenterX;
			[HideInInspector] public float m_CenterY;
			[HideInInspector] public float m_Edge;
			[HideInInspector] public float m_Length;
			[HideInInspector] public float m_Central;
		}
		public Settings m_Settings = new Settings();
		Pass m_Pass;

		public override void Create()
		{
			m_Pass = new Pass(name);
		}
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			RenderTargetIdentifier src = renderer.cameraColorTarget;
			if (m_Settings.m_Mat == null)
			{
				Debug.LogWarningFormat("Missing material. {0} pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
				return;
			}
			m_Settings.m_Mat.SetFloat("_CenterX", m_Settings.m_CenterX);
			m_Settings.m_Mat.SetFloat("_CenterY", m_Settings.m_CenterY);
			m_Settings.m_Mat.SetFloat("_CentralEdge", m_Settings.m_Edge);
			m_Settings.m_Mat.SetFloat("_CentralLength", m_Settings.m_Length);
			m_Settings.m_Mat.SetFloat("_Central", m_Settings.m_Central);
			m_Pass.Setup(src, m_Settings.m_Mat);
			renderer.EnqueuePass(m_Pass);
		}
	}
}