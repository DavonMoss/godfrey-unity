using UnityEngine;
using UnityEditor;
using System.Collections;

namespace NprPaintFilter
{
	[CustomEditor(typeof(FiltersController))]
	public class FiltersControllerInspector : Editor
	{
		class DrawableFilter
		{
			public SerializedProperty m_Sp = null;
			public string m_BtnDisplay;
			public int m_EnumIndex;
			public DrawableFilter(SerializedProperty sp, string display, int enumIdx)
			{
				m_Sp = sp;
				m_BtnDisplay = display;
				m_EnumIndex = enumIdx;
			}
		}
		SerializedProperty m_SpPaintFeature;
		SerializedProperty m_SpEnableFilter;
		DrawableFilter[] m_Drawables;

		void OnEnable()
		{
			m_SpPaintFeature = serializedObject.FindProperty("m_PaintFeature");
			m_SpEnableFilter = serializedObject.FindProperty("m_EnableFilter");
			m_Drawables = new DrawableFilter[22];
			m_Drawables[0] = new DrawableFilter(serializedObject.FindProperty("m_Lego"), "Use Lego", 1);
			m_Drawables[1] = new DrawableFilter(serializedObject.FindProperty("m_Polygonization"), "Use Polygonization", 2);
			m_Drawables[2] = new DrawableFilter(serializedObject.FindProperty("m_Cartoon"), "Use Cartoon", 4);
			m_Drawables[3] = new DrawableFilter(serializedObject.FindProperty("m_WaterColor"), "Use WaterColor", 5);
			m_Drawables[4] = new DrawableFilter(serializedObject.FindProperty("m_OilPaint"), "Use OilPaint", 6);
			m_Drawables[5] = new DrawableFilter(serializedObject.FindProperty("m_Knitwear"), "Use Knitwear", 7);
			m_Drawables[6] = new DrawableFilter(serializedObject.FindProperty("m_CrossHatch"), "Use CrossHatch", 8);
			m_Drawables[7] = new DrawableFilter(serializedObject.FindProperty("m_CmykHalftone"), "Use CmykHalftone", 9);
			m_Drawables[8] = new DrawableFilter(serializedObject.FindProperty("m_Halftone"), "Use Halftone", 10);
			m_Drawables[9] = new DrawableFilter(serializedObject.FindProperty("m_PencilDot"), "Use PencilDot", 11);
			m_Drawables[10] = new DrawableFilter(serializedObject.FindProperty("m_Pencil"), "Use Pencil", 12);
			m_Drawables[11] = new DrawableFilter(serializedObject.FindProperty("m_Tiles"), "Use Tiles", 13);
			m_Drawables[12] = new DrawableFilter(serializedObject.FindProperty("m_Ascii"), "Use Ascii", 3);
			m_Drawables[13] = new DrawableFilter(serializedObject.FindProperty("m_MosaicTriangles"), "Use MosaicTriangles", 15);
			m_Drawables[14] = new DrawableFilter(serializedObject.FindProperty("m_MosaicWindmill"), "Use MosaicWindmill", 14);
			m_Drawables[15] = new DrawableFilter(serializedObject.FindProperty("m_MosaicCircle"), "Use MosaicCircle", 16);
			m_Drawables[16] = new DrawableFilter(serializedObject.FindProperty("m_MosaicDiamond"), "Use MosaicDiamond", 17);
			m_Drawables[17] = new DrawableFilter(serializedObject.FindProperty("m_MosaicHexagon"), "Use MosaicHexagon", 18);
			m_Drawables[18] = new DrawableFilter(serializedObject.FindProperty("m_SketchOnNotebook"), "Use SketchOnNotebook", 19);
			m_Drawables[19] = new DrawableFilter(serializedObject.FindProperty("m_DotDrawing"), "Use DotDrawing", 20);
			m_Drawables[20] = new DrawableFilter(serializedObject.FindProperty("m_EdgeSobel"), "Use EdgeSobel", 21);
			m_Drawables[21] = new DrawableFilter(serializedObject.FindProperty("m_LineDrawing"), "Use LineDrawing", 22);
		}
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(m_SpPaintFeature, true);
			if (GUILayout.Button("Disable Filter"))
				m_SpEnableFilter.enumValueIndex = 0;

			foreach (DrawableFilter v in m_Drawables)
			{
				if (v == null || v.m_Sp == null)
					continue;

				GUI.enabled = (m_SpEnableFilter.enumValueIndex == v.m_EnumIndex) ? false : true;
				if (GUILayout.Button(v.m_BtnDisplay))
					m_SpEnableFilter.enumValueIndex = v.m_EnumIndex;

				if (Application.isPlaying)
					GUI.enabled = (m_SpEnableFilter.enumValueIndex == v.m_EnumIndex) ? true : false;
				else
					GUI.enabled = true;
				EditorGUILayout.PropertyField(v.m_Sp, true);
				GUI.enabled = true;
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}