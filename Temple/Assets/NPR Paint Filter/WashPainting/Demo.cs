using UnityEngine;

namespace NprPaintFilter
{
	public class Demo : MonoBehaviour
	{
		public WashPaintingFeature m_Feature;
		public Material m_Mat;
		public Texture2D m_Paper;
		public Color m_Color = Color.white;
		[Range(0f, 1f)] public float m_Alpha = 1f;
		[Range(0f, 0.001f)] public float m_Evaporation = 0.0001f;
//		[Range(0, 10)] public int m_Iterations = 4;
		bool m_Dragging = false;
		Vector2 m_Previous;

		void Start()
		{
			m_Feature.SetMaterial(m_Mat);
			m_Mat.SetTexture("_PaperTex", m_Paper);
			Initialize();
		}
		void Update()
		{
			Vector3 mousePos = Input.mousePosition;
			Vector2 current = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
			m_Mat.SetVector("_Prev", m_Previous);

			if (m_Dragging)
				m_Mat.SetVector("_Brush", new Vector3(current.x, current.y, 0.015f));
			else
				m_Mat.SetVector("_Brush", new Vector3(0, 0, 0));

			if (Input.GetMouseButtonDown(0))
				m_Dragging = true;
			else if (Input.GetMouseButtonUp(0))
				m_Dragging = false;

			m_Previous = current;

			m_Mat.SetFloat("_Alpha", m_Alpha);
			m_Mat.SetFloat("_Evaporation", m_Evaporation);
			m_Mat.SetColor("_PaintColor", m_Color);
		}
		void Initialize()
		{
			m_Feature.CustomInitialize();
		}
		void OnGUI()
		{
			if (GUI.Button(new Rect(10, 10, 60, 30), "Reset"))
				Initialize();
		}
	}
}
