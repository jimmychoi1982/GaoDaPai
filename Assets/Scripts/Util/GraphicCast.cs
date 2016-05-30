using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.UI
{
	public class GraphicCast : Graphic{
		
		protected override void OnFillVBO (System.Collections.Generic.List<UIVertex> vbo)
		{
			base.OnFillVBO (vbo);
			vbo.Clear();
		}
		#if UNITY_EDITOR
		
		[CustomEditor(typeof(GraphicCast))]
		class GraphicCastEditor : Editor
		{
			public override void OnInspectorGUI() {
			}
		}
		
		#endif
	} 
}
