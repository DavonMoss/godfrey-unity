using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NprPaintFilter
{
	public class PaintFeature : ScriptableRendererFeature
	{
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public class Pass : ScriptableRenderPass
		{
			Material m_Mat;
			RenderTargetIdentifier m_Source;
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
				m_Source = source;
				m_Mat = mat;
			}
			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
	
				RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
				opaqueDesc.depthBufferBits = 0;
				cmd.GetTemporaryRT(m_TempColorTexture.id, opaqueDesc, FilterMode.Bilinear);

				Blit(cmd, m_Source, m_TempColorTexture.Identifier(), m_Mat, 0);
				Blit(cmd, m_TempColorTexture.Identifier(), m_Source);

				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}
			public override void FrameCleanup(CommandBuffer cmd)
			{
				cmd.ReleaseTemporaryRT(m_TempColorTexture.id);
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public class WaterColorPass : ScriptableRenderPass
		{
			Material m_Mat;
			string m_ProfilerTag;
			RenderTargetIdentifier m_RtID1;
			RenderTargetIdentifier m_RtID2;
			RenderTargetIdentifier m_Source;
			Texture2D m_Paper1;
			Texture2D m_Paper2;

			public WaterColorPass(string tag)
			{
				this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
				m_ProfilerTag = tag;
			}
			public void Setup(RenderTargetIdentifier source, Material mat, Texture2D paper1, Texture2D paper2)
			{
				m_Source = source;
				m_Mat = mat;
				m_Paper1 = paper1;
				m_Paper2 = paper2;
			}
			public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
			{
				int width = cameraTextureDescriptor.width;
				int height = cameraTextureDescriptor.height;

				int id1 = Shader.PropertyToID("tmpRT1");
				int id2 = Shader.PropertyToID("tmpRT2");
				cmd.GetTemporaryRT(id1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
				cmd.GetTemporaryRT(id2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
				m_RtID1 = new RenderTargetIdentifier(id1);
				m_RtID2 = new RenderTargetIdentifier(id2);
				ConfigureTarget(m_RtID1);
				ConfigureTarget(m_RtID2);
			}
			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
				cmd.Blit(m_Source, m_RtID1, m_Mat, 0);

				m_Mat.SetTexture("_PaperTex", m_Paper1);
				cmd.Blit(m_RtID1, m_RtID2, m_Mat, 1);
				cmd.Blit(m_RtID2, m_RtID1, m_Mat, 2);

				m_Mat.SetTexture("_PaperTex", m_Paper2);
				cmd.Blit(m_RtID1, m_RtID2, m_Mat, 1);
				cmd.Blit(m_RtID2, m_Source, m_Mat, 2);

				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}
			public override void FrameCleanup(CommandBuffer cmd)
			{
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public class OilPaintPass : ScriptableRenderPass
		{
			Material m_Mat;
			string m_ProfilerTag;
			int m_RtPropID1 = 0;
			int m_RtPropID2 = 0;
			RenderTargetIdentifier m_RtID1;
			RenderTargetIdentifier m_RtID2;
			RenderTargetIdentifier m_Source;

			public OilPaintPass(string tag)
			{
				this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
				m_ProfilerTag = tag;
			}
			public void Setup(RenderTargetIdentifier source, Material mat)
			{
				m_Source = source;
				m_Mat = mat;
			}
			public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
			{
				int width = cameraTextureDescriptor.width;
				int height = cameraTextureDescriptor.height;

				m_RtPropID1 = Shader.PropertyToID("tmpRT1");
				m_RtPropID2 = Shader.PropertyToID("tmpRT2");
				cmd.GetTemporaryRT(m_RtPropID1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
				cmd.GetTemporaryRT(m_RtPropID2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
				m_RtID1 = new RenderTargetIdentifier(m_RtPropID1);
				m_RtID2 = new RenderTargetIdentifier(m_RtPropID2);
				ConfigureTarget(m_RtID1);
				ConfigureTarget(m_RtID2);
			}
			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
				cmd.Blit(m_Source, m_RtID1, m_Mat, 0);
				cmd.Blit(m_RtID1, m_RtID2, m_Mat, 0);
				cmd.Blit(m_RtID2, m_RtID1, m_Mat, 0);
				cmd.Blit(m_RtID1, m_Source, m_Mat, 1);
				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}
			public override void FrameCleanup(CommandBuffer cmd)
			{
				cmd.ReleaseTemporaryRT(m_RtPropID1);
				cmd.ReleaseTemporaryRT(m_RtPropID2);
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public Material m_Mat;
		[HideInInspector] public Texture2D m_WaterColorPaper1;
		[HideInInspector] public Texture2D m_WaterColorPaper2;
		Pass m_Pass;
		WaterColorPass m_WaterColorPass;
		OilPaintPass m_OilPaintPass;
		[HideInInspector] public int m_UsePass = 0;

		public override void Create()
		{
			m_Pass = new Pass(name);
			m_WaterColorPass = new WaterColorPass(name);
			m_OilPaintPass = new OilPaintPass(name);
		}
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			RenderTargetIdentifier src = renderer.cameraColorTarget;
			if (m_Mat == null)
			{
				Debug.LogWarningFormat("Missing material. {0} pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
				return;
			}

			if (m_UsePass == 0)
			{
				m_Pass.Setup(src, m_Mat);
				renderer.EnqueuePass(m_Pass);
			}
			else if (m_UsePass == 1)
			{
				m_WaterColorPass.Setup(src, m_Mat, m_WaterColorPaper1, m_WaterColorPaper2);
				renderer.EnqueuePass(m_WaterColorPass);
			}
			else if (m_UsePass == 2)
			{
				m_OilPaintPass.Setup(src, m_Mat);
				renderer.EnqueuePass(m_OilPaintPass);
			}
		}
	}
}