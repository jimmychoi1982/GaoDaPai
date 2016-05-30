using UnityEngine; 
using UnityEngine.UI; 
using System.Collections;

public class AspectRatioFixer : MonoBehaviour { 
	public float baseWidth = 800f;
	public float baseHeight = 450f;
	public float baseWebViewMarginTop = 57.2f;
	public int webViewMarginTop = 0;
	public int webViewMarginBottom = 0;

	public void Revise () { 
		CanvasScaler scaler = gameObject.GetComponent<CanvasScaler> ();
		if (scaler != null) {
			baseWidth = scaler.referenceResolution.x;
			baseHeight = scaler.referenceResolution.y;
		}

		float baseAspectRatio = baseHeight / baseWidth; 
		float currentAspectRatio = (float)Screen.height / (float)Screen.width; 
		float revision; 
		
		float marginTop = 0f;
		float windowSizeRatio = 0f;
		foreach (Camera camera in Camera.allCameras) {
			if (camera.name == "Background Camera") continue;
			if (baseAspectRatio > currentAspectRatio) { 
				revision = currentAspectRatio / baseAspectRatio; 
				camera.rect = new Rect ((1 - revision) * 0.5f, 0, revision, 1); 
				marginTop = 0f;
				windowSizeRatio = (float)Screen.height / (float)baseHeight;
			} else { 
				revision = baseAspectRatio / currentAspectRatio; 
				camera.rect = new Rect (0, (1 - revision) * 0.5f, 1, revision); 
				marginTop = ((float)Screen.height - (((float)Screen.width / (float)baseWidth) * (float)baseHeight)) / 2f;
				windowSizeRatio = (float)Screen.width / (float)baseWidth;
			}
		}
		webViewMarginTop = (int)((windowSizeRatio * baseWebViewMarginTop) + marginTop);
		webViewMarginBottom = (int)marginTop;
	} 
}
