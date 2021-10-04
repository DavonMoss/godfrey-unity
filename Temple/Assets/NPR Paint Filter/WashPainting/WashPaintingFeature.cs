using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NprPaintFilter
{
	public class WashPaintingFeature : ScriptableRendererFeature
	{
		public class Pass : ScriptableRenderPass
		{
			Material m_Mat;
			string m_ProfilerTag;
			RenderTargetIdentifier[] m_RtIDs = new RenderTargetIdentifier[2];
			RenderTargetIdentifier m_Source;
			int m_Iterations = 4;
			int m_ReadRt = 0;
			int m_WriteRt = 1;
			bool m_NeedInitialize = false;

			public Pass(string tag)
			{
				this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
				m_ProfilerTag = tag;
			}
			public void Setup(RenderTargetIdentifier source, Material mat, bool needInitialize, RenderTargetIdentifier id0, RenderTargetIdentifier id1)
			{
				m_Source = source;
				m_Mat = mat;
				m_NeedInitialize = needInitialize;
				m_RtIDs[m_ReadRt] = id0;
				m_RtIDs[m_WriteRt] = id1;
			}
			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

				// do I need run initialize clear pass ?
				if (m_NeedInitialize)
				{
					cmd.Blit(m_RtIDs[m_ReadRt], m_RtIDs[m_WriteRt], m_Mat, 0);
					Swap();
					m_NeedInitialize = false;
				}

				// do magic pass now
				for (int i = 0; i < m_Iterations; i++)
				{
					cmd.Blit(m_RtIDs[m_ReadRt], m_RtIDs[m_WriteRt], m_Mat, 1);
					Swap();
				}

				// final blit to screen pass
				cmd.Blit(m_RtIDs[m_ReadRt], m_Source, m_Mat, 2);
				
				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}
			void Swap()
			{
				int tmp = m_ReadRt;
				m_ReadRt = m_WriteRt;
				m_WriteRt = tmp;
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		Material m_Mat;
		bool m_NeedInitialize = true;
		Pass m_Pass;

		public int m_RtSize = 1024;
		RenderTexture[] m_RtBuffers;
		RenderTargetIdentifier[] m_RtIDs;

		public override void Create()
		{
			m_Pass = new Pass(name);
		}
		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			RenderTargetIdentifier src = renderer.cameraColorTarget;
			if (m_Mat == null)
			{
				Debug.LogWarningFormat("Missing material. {0} pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
				return;
			}
			m_Pass.Setup(src, m_Mat, m_NeedInitialize, m_RtIDs[0], m_RtIDs[1]);
			renderer.EnqueuePass(m_Pass);
			m_NeedInitialize = false;
		}
		public void SetMaterial(Material mat)
		{
			m_Mat = mat;
		}
		public void CustomInitialize()
		{
			// cleanup before initialize
			if (m_RtBuffers != null)
			{
				for (int i = 0; i < m_RtBuffers.Length; i++)
				{
					m_RtBuffers[i].Release();
					m_RtBuffers[i].DiscardContents();
					m_RtBuffers[i] = null;
					//m_RtIDs[i] = null;
				}
			}
			// now do initialize happy and safe
			m_NeedInitialize = true;
			m_RtBuffers = new RenderTexture[2];
			m_RtIDs = new RenderTargetIdentifier[2];
			for (int i = 0; i < 2; i++)
			{
				m_RtBuffers[i] = new RenderTexture(m_RtSize, m_RtSize, 0, RenderTextureFormat.ARGBFloat);
				m_RtBuffers[i].hideFlags  = HideFlags.DontSave;
				m_RtBuffers[i].filterMode = FilterMode.Point;
				m_RtBuffers[i].wrapMode   = TextureWrapMode.Repeat;
				m_RtBuffers[i].Create();

				m_RtIDs[i] = new RenderTargetIdentifier(m_RtBuffers[i]);
			}
		}
	}
}