using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace NprPaintFilter
{
	[RequireComponent(typeof(Camera))]
	public class MangaSpeedLineCamera : MonoBehaviour
	{
		public MangaSpeedLineFeature m_Feature;
		public GameObject m_Target;
		public Camera m_Cam;
		float m_Delta;

		void Update()
		{
			float EDGE_MAX = 0.35f;
			Vector3 position = m_Target.transform.position;
			Vector3 scrpos = m_Cam.WorldToScreenPoint(m_Target.transform.position);
			m_Feature.m_Settings.m_CenterX = scrpos.x / Screen.width;
			m_Feature.m_Settings.m_CenterY = scrpos.y / Screen.height;

			float distance = Vector3.Distance(position, m_Cam.transform.position) / 10f;
			float edge = EDGE_MAX * (1f - Mathf.Clamp(distance / 10f, 0f, EDGE_MAX) / EDGE_MAX);
			m_Feature.m_Settings.m_Edge = edge;
			m_Feature.m_Settings.m_Length = edge + distance / 10f;

			float central = (0.01f*(0.5f*Mathf.Sin(Time.realtimeSinceStartup*32f)+0.5f)+0.2f) + distance/100f;
			if (m_Delta < 0)
			{
				m_Feature.m_Settings.m_Central = central;
				m_Delta = 0;
			}
			else
			{
				m_Delta -= Time.deltaTime;
				m_Feature.m_Settings.m_Central = Mathf.Lerp(central, 0, m_Delta * 2f);
			}
		}
	}
}