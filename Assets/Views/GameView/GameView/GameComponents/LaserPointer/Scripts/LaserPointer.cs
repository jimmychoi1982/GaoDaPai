using System.Collections;

using UnityEngine;

public class LaserPointer : MonoBehaviour {
	//
	[Header("Pointer Tip")]
	public GameObject pointer;
	public bool useDirection = false;
	public float pointerOffset = 0;

	//
	[Header("Line Body")]
	public LineRenderer line;
	public bool tileTexture = true;
	public float scrollSpeed = 5;

	//
	[Header("Line Position")]
	public Vector2 startPos;
	public Vector2 endPos;

	//
	[Header("Line Width")]
	public float lineWidth = 1;

	//
	[Header("Line Color")]
	public Color lineColor = Color.white;
	
	//
	[Header("Sorting")]
	public int sortingOrder = 0;


	// Update is called once per frame
	void Update () {
		// Set line renderer properties
		line.SetPosition(0, startPos);
		line.SetPosition(1, endPos);
		line.SetWidth(lineWidth, lineWidth);
		line.SetColors(lineColor, lineColor);
		line.sortingOrder = sortingOrder;

		if (tileTexture) {
			// Determine how many tiles we need
			float lineLength = line.bounds.size.magnitude;
			float tiles = lineLength / lineWidth;
			line.material.mainTextureScale = new Vector2(tiles, 1);
		} else {
			// Reset texture tiles
			line.material.mainTextureScale = new Vector2(1, 1);
		}

		if (scrollSpeed > 0) {
			float timePos = Time.time * scrollSpeed;
			line.material.mainTextureOffset = new Vector2(1 - (timePos % 1), 1);
		}

		if (useDirection) {
			// Set pointer position and rotation
			Vector2 direction = endPos - startPos;
			pointer.transform.localPosition = endPos + (direction.normalized * pointerOffset);
			pointer.transform.localRotation = Quaternion.FromToRotation(Vector2.up, direction);
		} else {
			// Reset pointer position and rotation
			pointer.transform.localPosition = endPos;
			pointer.transform.localRotation = Quaternion.FromToRotation(Vector2.up, Vector2.up);
		}
	}
}
