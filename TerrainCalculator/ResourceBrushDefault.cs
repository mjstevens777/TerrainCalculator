//using System;
//using ColossalFramework;
//using UnityEngine;

//namespace TerrainCalculator
//{
//    public class ResourceBrushDefault
//    {
//        public ResourceBrushDefault()
//        {
//        }

//        // ResourceTool

//	private void ApplyBrush(bool negate)
//	{
//		float[] brushData = m_toolController.BrushData;
//		float radius = m_brushSize * 0.5f;
//		float cellSize = 33.75f;
//		int numCells = 512;
//		int numBrushCells = 64;
//		NaturalResourceManager.ResourceCell[] naturalResources = Singleton<NaturalResourceManager>.instance.m_naturalResources;
//		float strength = m_strength;
//		Vector3 mousePosition = m_mousePosition;
//		int left = Mathf.Max((int)((mousePosition.x - radius) / cellSize + (float)numCells * 0.5f), 0);
//		int top = Mathf.Max((int)((mousePosition.z - radius) / cellSize + (float)numCells * 0.5f), 0);
//		int right = Mathf.Min((int)((mousePosition.x + radius) / cellSize + (float)numCells * 0.5f), numCells - 1);
//		int bottom = Mathf.Min((int)((mousePosition.z + radius) / cellSize + (float)numCells * 0.5f), numCells - 1);
//		for (int i = top; i <= bottom; i++)
//		{
//			float iBrush = (((float)i - (float)numCells * 0.5f + 0.5f) * cellSize - mousePosition.z + radius) / m_brushSize * (float)numBrushCells - 0.5f;
//			int iBrushMin = Mathf.Clamp(Mathf.FloorToInt(iBrush), 0, 63);
//			int iBrushMax = Mathf.Clamp(Mathf.CeilToInt(iBrush), 0, 63);
//			for (int j = left; j <= right; j++)
//			{
//				float jBrush = (((float)j - (float)numCells * 0.5f + 0.5f) * cellSize - mousePosition.x + radius) / m_brushSize * (float)numBrushCells - 0.5f;
//				int jBrushMin = Mathf.Clamp(Mathf.FloorToInt(jBrush), 0, 63);
//				int jBrushMax = Mathf.Clamp(Mathf.CeilToInt(jBrush), 0, 63);
//				float brushTL = brushData[iBrushMin * 64 + jBrushMin];
//				float brushTR = brushData[iBrushMin * 64 + jBrushMax];
//				float brushBL = brushData[iBrushMax * 64 + jBrushMin];
//				float brushBR = brushData[iBrushMax * 64 + jBrushMax];
//				float brushT = brushTL + (brushTR - brushTL) * (jBrush - (float)jBrushMin);
//				float brushB = brushBL + (brushBR - brushBL) * (jBrush - (float)jBrushMin);
//				float brushValRaw = brushT + (brushB - brushT) * (iBrush - (float)iBrushMin);
//				NaturalResourceManager.ResourceCell resourceCell = naturalResources[i * numCells + j];
//				int brushVal = (int)(255f * strength * brushValRaw);
//				if (negate)
//				{
//					brushVal = -brushVal;
//				}
//				if (m_resource == NaturalResourceManager.Resource.Ore)
//				{
//					ChangeMaterial(ref resourceCell.m_ore, ref resourceCell.m_oil, brushVal);
//				}
//				else if (m_resource == NaturalResourceManager.Resource.Oil)
//				{
//					ChangeMaterial(ref resourceCell.m_oil, ref resourceCell.m_ore, brushVal);
//				}
//				else if (m_resource == NaturalResourceManager.Resource.Sand)
//				{
//					ChangeMaterial(ref resourceCell.m_sand, ref resourceCell.m_fertility, brushVal);
//				}
//				else if (m_resource == NaturalResourceManager.Resource.Fertility && resourceCell.m_forest == 0)
//				{
//					ChangeMaterial(ref resourceCell.m_fertility, ref resourceCell.m_sand, brushVal);
//				}
//				naturalResources[i * numCells + j] = resourceCell;
//			}
//		}
//		Singleton<NaturalResourceManager>.instance.AreaModified(left, top, right, bottom);
//	}

//}
//}
