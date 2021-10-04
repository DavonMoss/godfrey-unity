using UnityEngine;

namespace NprPaintFilter
{
	public class FiltersController : MonoBehaviour
	{
		[Header("Basic")]
		public PaintFeature m_PaintFeature;
		public enum EFilter {
			None = 0,
			Lego = 1,
			Polygonization = 2,
			Ascii = 3,
			Cartoon = 4,
			WaterColor = 5,
			OilPaint = 6,
			Knitwear = 7,
			CrossHatch = 8,
			CmykHalftone = 9,
			Halftone = 10,
			PencilDot = 11,
			Pencil = 12,
			Tiles = 13,
			MosaicWindmill = 14,
			MosaicTriangles = 15,
			MosaicCircle = 16,
			MosaicDiamond = 17,
			MosaicHexagon = 18,
			SketchOnNotebook = 19,
			DotDrawing = 20,
			EdgeSobel = 21,
			LineDrawing = 22, }
		public EFilter m_EnableFilter = EFilter.None;
		EFilter m_PrevEnableFilter = EFilter.None;
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class Lego
		{
			public Material m_Mat;
			[Range(0.01f, 0.08f)] public float m_Size = 0.02f;
			[Range(1f, 6f)] public float m_GridShadow = 2f;
			[Range(1.5f, 3.5f)] public float m_CircleSize = 2f;

			public void Apply()
			{
				m_Mat.SetFloat("_Size", m_Size);
				m_Mat.SetFloat("_GridShadow", m_GridShadow);
				m_Mat.SetFloat("_CircleSize", m_CircleSize);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class Ascii
		{
			public Material m_Mat;
			public void Apply()
			{}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class Polygonization
		{
			public Material m_Mat;
			[Range(0f, 1f)] public float m_Strength = 1f;
			[Range(1f, 32f)] public float m_Size = 8f;
			[Range(1f, 4f)] public float m_Blur = 2f;
			public enum EType { Custom = 0, Type1, Type2 };
			public EType m_Fx = EType.Type1;

			public void Apply()
			{
				if (m_Fx == EType.Type1)
				{
					m_Strength = 1f;
					m_Size = 8f;
					m_Blur = 2f;
				}
				else if (m_Fx == EType.Type2)
				{
					m_Strength = 1f;
					m_Size = 32f;
					m_Blur = 1f;
				}
				m_Mat.SetFloat("_Strength", m_Strength);
				m_Mat.SetFloat("_Size", m_Size);
				m_Mat.SetFloat("_Blur", m_Blur);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class Cartoon
		{
			public Material m_Mat;
			[Range(0f, 1f)] public float m_Threshold = 0.35f;
			public Vector2 m_Resolution = new Vector2(512f, 512f);

			public void Apply()
			{
				m_Mat.SetFloat("_Threshold", m_Threshold);
				m_Mat.SetVector("_Resolution", m_Resolution);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class CrossHatch
		{
			public Material m_Mat;
			[Range(0f, 6f)] public float m_LineWidth = 5f;
			public Vector2 m_Resolution = new Vector2(800f, 800f);

			public void Apply()
			{
				m_Mat.SetFloat("_LineWidth", m_LineWidth);
				m_Mat.SetVector("_Resolution", m_Resolution);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class CmykHalftone
		{
			public Material m_Mat;
			[Range(0.1f, 1.2f)] public float m_Scale = 1f;
			[Range(0.1f, 8f)] public float m_Angle = 4f;
			[Range(1f, 8f)] public float m_Strength = 4f;

			public void Apply()
			{
				m_Mat.SetFloat("_Scale", m_Scale);
				m_Mat.SetFloat("_Angle", m_Angle);
				m_Mat.SetFloat("_Strength", m_Strength);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class Halftone
		{
			public Material m_Mat;
			[Range(1f, 16f)] public float m_Size = 3f;
			[Range(0f, 1f)] public float m_Intensity = 0.3f;

			public void Apply()
			{
				m_Mat.SetFloat("_Intensity", m_Intensity);
				m_Mat.SetFloat("_Size", m_Size);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class PencilDot
		{
			public Material m_Mat;
			[Range(6f, 9.7f)] public float m_Thickness = 9f;

			public void Apply()
			{
				m_Mat.SetFloat("_Thickness", m_Thickness);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class Pencil
		{
			public Material m_Mat;
			public Color m_Color = new Color(0.4f, 0.075f, 0.075f, 1f);
			[Range(-0.5f, 0.03f)] public float m_Strength = 0.02f;
			[Range(1f, 5f)] public float m_Offset = 2f;

			public void Apply()
			{
				m_Mat.SetFloat("_Strength", m_Strength);
				m_Mat.SetFloat("_Offset", m_Offset);
				m_Mat.SetColor("_LineColor", m_Color);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class WaterColor
		{
			public Material m_Mat;
			public Texture2D m_WobbleTex;
			public float m_WobbleScale = 1f;
			[Range(0.001f, 0.006f)] public float m_WobblePower = 0.003f;
			[Range(0f, 2f)] public float m_EdgeSize = 1f;
			[Range(-3f, 3f)] public float m_EdgePower = 3f;
			public Texture2D m_Paper1;
			float m_Paper1Power = 1f;
			public Texture2D m_Paper2;
			float m_Paper2Power = 1f;

			public void Apply()
			{
				m_Mat.SetTexture("_WobbleTex", m_WobbleTex);
				m_Mat.SetFloat("_WobbleScale", m_WobbleScale);
				m_Mat.SetFloat("_WobblePower", m_WobblePower);
				m_Mat.SetFloat("_EdgeSize", m_EdgeSize);
				m_Mat.SetFloat("_EdgePower", m_EdgePower);
				m_Mat.SetFloat("_PaperPower", m_Paper1Power);
				m_Mat.SetFloat("_PaperPower", m_Paper2Power);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class OilPaint
		{
			public Material m_Mat;
			[Range(0.1f, 3f)] public float m_Resolution = 0.9f;
			public float m_EdgeSize = 1f;
			public float m_EdgePower = 3f;
			public enum EType { Custom = 0, Type1, Type2, Type3 };
			public EType m_Fx = EType.Custom;

			public void Apply()
			{
				if (m_Fx == EType.Type1)
				{
					m_Resolution = 2f;
					m_EdgeSize = 1f;
					m_EdgePower = 1f;
				}
				else if (m_Fx == EType.Type2)
				{
					m_Resolution = 2f;
					m_EdgeSize = 2f;
					m_EdgePower = 3f;
				}
				else if (m_Fx == EType.Type3)
				{
					m_Resolution = 2f;
					m_EdgeSize = 2f;
					m_EdgePower = 9f;
				}
				m_Mat.SetFloat("_Resolution", m_Resolution);
				m_Mat.SetFloat("_EdgeSize", m_EdgeSize);
				m_Mat.SetFloat("_EdgePower", m_EdgePower);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class Knitwear
		{
			public Material m_Mat;
			[Range(-2f, 2f)] public float m_Shear = 1f;
			[Range(1f, 200f)] public float m_Division = 60f;
			[Range(0.2f, 5f)] public float m_Aspect = 1f;
			public Texture2D m_Tex;

			public void Apply()
			{
				m_Mat.SetFloat("_KnitwearShear", m_Shear);
				m_Mat.SetFloat("_KnitwearDivision", m_Division);
				m_Mat.SetFloat("_KnitwearAspect", m_Aspect);
				m_Mat.SetTexture("_KnitwearTex", m_Tex);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class MosaicWindmill
		{
			public Material m_Mat;
			[Range(1f, 128f)] public float m_PixelSize = 64f;
			[Range(0.2f, 5f)] public float m_PixelRatio = 1f;
			[Range(0.2f, 5f)] public float m_PixelScaleX = 1f;
			[Range(0.2f, 5f)] public float m_PixelScaleY = 1f;

			public void Apply()
			{
				m_Mat.SetFloat("_PixelSize", m_PixelSize);
				m_Mat.SetFloat("_PixelRatio", m_PixelRatio);
				m_Mat.SetFloat("_PixelScaleX", m_PixelScaleX);
				m_Mat.SetFloat("_PixelScaleY", m_PixelScaleY);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class Tiles
		{
			public Material m_Mat;
			[Range(1f, 128f)] public float m_NumTiles = 48f;
			[Range(0f, 1f)] public float m_Threshhold = 0.2f;
			public Color m_EdgeColor = new Color(0.7f, 0.7f, 0.7f, 1f);

			public void Apply()
			{
				m_Mat.SetFloat("_NumTiles", m_NumTiles);
				m_Mat.SetFloat("_Threshhold", m_Threshhold);
				m_Mat.SetColor("_EdgeColor", m_EdgeColor);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class MosaicCircle
		{
			public Material m_Mat;
			[Range(0.7f, 0.9f)] public float m_Size = 0.85f;
			[Range(0.01f, 1f)] public float m_Radius = 0.45f;
			[Range(0.9f, 5f)] public float m_Interval = 1f;
			public Color m_Background = new Color(0f, 0f, 0f, 1f);

			public void Apply()
			{
				float size = (1.01f - m_Size) * 300f;
				Vector4 param = new Vector4(size, m_Interval, m_Radius, 0f);
				m_Mat.SetVector("_Params", param);
				m_Mat.SetColor("_BackgroundColor", m_Background);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class MosaicTriangles
		{
			public Material m_Mat;
			[Range(10, 60)] public float m_TileNumX = 40f;
			[Range(10, 60)] public float m_TileNumY = 20f;

			public void Apply()
			{
				m_Mat.SetVector("_TileNum", new Vector4(m_TileNumX, m_TileNumY, 0, 0));
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class MosaicDiamond
		{
			public Material m_Mat;
			[Range(0.01f, 1f)] public float m_PixelSize = 0.2f;

			public void Apply()
			{
				m_Mat.SetFloat("_PixelSize", m_PixelSize);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class MosaicHexagon
		{
			public Material m_Mat;
			[Range(0.02f, 0.5f)] public float m_PixelSize = 0.05f;
			[Range(0.01f, 5f)] public float m_GridWidth = 1f;

			public void Apply()
			{
				m_Mat.SetFloat("_PixelSize", m_PixelSize);
				m_Mat.SetFloat("_GridWidth", m_GridWidth);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class SketchOnNotebook
		{
			public Material m_Mat;
			[Range(0.01f, 3f)] public float m_BrushStrength = 1f;
			[Range(90f, 1500f)] public float m_BrushExpand = 400f;
			[Range(0.01f, 6f)] public float m_Vignette = 1f;
			public bool m_BackgroundGrid = true;
			[Range(1f, 600f)] public float m_GridSize = 400f;
			[Range(-0.4f, 0.1f)] public float m_Colorful = -0.33f;

			public void Apply()
			{
				m_Mat.SetVector("_Features", new Vector4(m_BackgroundGrid ? 1f : 0f, m_Vignette, 0f, 0f));
				m_Mat.SetFloat("_BrushStrength", m_BrushStrength);
				m_Mat.SetFloat("_BrushExpand", m_BrushExpand);
				m_Mat.SetFloat("_GridSize", m_GridSize);
				m_Mat.SetFloat("_Colorful", m_Colorful);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class DotDrawing
		{
			public Material m_Mat;
			[Range(1f, 16f)] public float m_DotSize = 9f;
			[Range(1f, 64f)] public float m_Darkness = 3f;

			public void Apply()
			{
				m_Mat.SetFloat("_DotSize", m_DotSize);
				m_Mat.SetFloat("_Darkness", m_Darkness);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class EdgeSobel
		{
			public Material m_Mat;
			public Vector2 m_Resolution = new Vector2(1024f, 1024f);
			public Color m_EdgeColor = new Color(1f, 1f, 1f, 1f);
			public Color m_BgColor = new Color(0f, 0f, 0f, 1f);

			public void Apply()
			{
				m_Mat.SetVector("_Resolution", m_Resolution);
				m_Mat.SetColor("_LineColor", m_EdgeColor);
				m_Mat.SetColor("_BgColor", m_BgColor);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		[System.Serializable] public class LineDrawing
		{
			public Material m_Mat;
			[Range(0.1f, 4f)] public float m_Steps = 1f;
			[Range(0f, 16f)] public float m_Offset = 1f;

			public void Apply()
			{
				m_Mat.SetFloat("_Steps", m_Steps);
				m_Mat.SetFloat("_Offset", m_Offset);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public Lego m_Lego;
		public Polygonization m_Polygonization;
		public Ascii m_Ascii;
		public Cartoon m_Cartoon;
		public WaterColor m_WaterColor;
		public OilPaint m_OilPaint;
		public Knitwear m_Knitwear;
		public CrossHatch m_CrossHatch;
		public CmykHalftone m_CmykHalftone;
		public Halftone m_Halftone;
		public PencilDot m_PencilDot;
		public Pencil m_Pencil;
		public Tiles m_Tiles;
		public MosaicTriangles m_MosaicTriangles;
		public MosaicWindmill m_MosaicWindmill;
		public MosaicCircle m_MosaicCircle;
		public MosaicDiamond m_MosaicDiamond;
		public MosaicHexagon m_MosaicHexagon;
		public SketchOnNotebook m_SketchOnNotebook;
		public DotDrawing m_DotDrawing;
		public EdgeSobel m_EdgeSobel;
		public LineDrawing m_LineDrawing;
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void Start()
		{
			ApplyFilter();
		}
		void Update()
		{
			// change material
			if (m_PrevEnableFilter != m_EnableFilter)
			{
				ApplyFilter();
				m_PrevEnableFilter = m_EnableFilter;
			}

			// setup material parameters
			if (EFilter.PencilDot == m_EnableFilter)              m_PencilDot.Apply();
			else if (EFilter.Polygonization == m_EnableFilter)    m_Polygonization.Apply();
			else if (EFilter.Pencil == m_EnableFilter)            m_Pencil.Apply();
			else if (EFilter.Cartoon == m_EnableFilter)           m_Cartoon.Apply();
			else if (EFilter.CrossHatch == m_EnableFilter)        m_CrossHatch.Apply();
			else if (EFilter.CmykHalftone == m_EnableFilter)      m_CmykHalftone.Apply();
			else if (EFilter.Halftone == m_EnableFilter)          m_Halftone.Apply();
			else if (EFilter.WaterColor == m_EnableFilter)        m_WaterColor.Apply();
			else if (EFilter.MosaicTriangles == m_EnableFilter)   m_MosaicTriangles.Apply();
			else if (EFilter.OilPaint == m_EnableFilter)          m_OilPaint.Apply();
			else if (EFilter.Knitwear == m_EnableFilter)          m_Knitwear.Apply();
			else if (EFilter.Lego == m_EnableFilter)              m_Lego.Apply();
			else if (EFilter.MosaicWindmill == m_EnableFilter)    m_MosaicWindmill.Apply();
			else if (EFilter.Tiles == m_EnableFilter)             m_Tiles.Apply();
			else if (EFilter.MosaicCircle == m_EnableFilter)      m_MosaicCircle.Apply();
			else if (EFilter.MosaicDiamond == m_EnableFilter)     m_MosaicDiamond.Apply();
			else if (EFilter.MosaicHexagon == m_EnableFilter)     m_MosaicHexagon.Apply();
			else if (EFilter.Ascii == m_EnableFilter)             m_Ascii.Apply();
			else if (EFilter.SketchOnNotebook == m_EnableFilter)  m_SketchOnNotebook.Apply();
			else if (EFilter.DotDrawing == m_EnableFilter)        m_DotDrawing.Apply();
			else if (EFilter.EdgeSobel == m_EnableFilter)         m_EdgeSobel.Apply();
			else if (EFilter.LineDrawing == m_EnableFilter)       m_LineDrawing.Apply();
		}
		void ApplyFilter()
		{
			if (EFilter.None == m_EnableFilter)
			{
				m_PaintFeature.SetActive(false);
				return;
			}
			m_PaintFeature.SetActive(true);
			if (EFilter.Lego == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_Lego.m_Mat;
			}
			else if (EFilter.Polygonization == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_Polygonization.m_Mat;
			}
			else if (EFilter.PencilDot == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_PencilDot.m_Mat;
			}
			else if (EFilter.Pencil == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_Pencil.m_Mat;
			}
			else if (EFilter.Cartoon == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_Cartoon.m_Mat;
			}
			else if (EFilter.CrossHatch == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_CrossHatch.m_Mat;
			}
			else if (EFilter.CmykHalftone == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_CmykHalftone.m_Mat;
			}
			else if (EFilter.Halftone == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_Halftone.m_Mat;
			}
			else if (EFilter.WaterColor == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 1;
				m_PaintFeature.m_WaterColorPaper1 = m_WaterColor.m_Paper1;
				m_PaintFeature.m_WaterColorPaper2 = m_WaterColor.m_Paper2;
				m_PaintFeature.m_Mat = m_WaterColor.m_Mat;
			}
			else if (EFilter.MosaicTriangles == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_MosaicTriangles.m_Mat;
			}
			else if (EFilter.OilPaint == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 2;
				m_PaintFeature.m_Mat = m_OilPaint.m_Mat;
			}
			else if (EFilter.Knitwear == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_Knitwear.m_Mat;
			}
			else if (EFilter.MosaicWindmill == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_MosaicWindmill.m_Mat;
			}
			else if (EFilter.Tiles == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_Tiles.m_Mat;
			}
			else if (EFilter.MosaicCircle == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_MosaicCircle.m_Mat;
			}
			else if (EFilter.MosaicDiamond == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_MosaicDiamond.m_Mat;
			}
			else if (EFilter.MosaicHexagon == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_MosaicHexagon.m_Mat;
			}
			else if (EFilter.Ascii == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_Ascii.m_Mat;
			}
			else if (EFilter.SketchOnNotebook == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_SketchOnNotebook.m_Mat;
			}
			else if (EFilter.DotDrawing == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_DotDrawing.m_Mat;
			}
			else if (EFilter.EdgeSobel == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_EdgeSobel.m_Mat;
			}
			else if (EFilter.LineDrawing == m_EnableFilter)
			{
				m_PaintFeature.m_UsePass = 0;
				m_PaintFeature.m_Mat = m_LineDrawing.m_Mat;
			}
		}
	}
}