using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class HorizonHeight : MonoBehaviour {
	public const float EPSILON = 1e-6f;

	public float maxDistance;
	public float surfaceHeight = 0f;
	public ScreenSpaceGrid grid;

	void Update () {
		var x = ForwardDistance(maxDistance);
		if (x < 0)
			return;
		var posCamera = transform.position;
		var rotYCamera = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		var posMarker = new Vector3(posCamera.x + x * Mathf.Sin(rotYCamera), 0f, posCamera.z + x * Mathf.Cos(rotYCamera));
		var posScreen = camera.WorldToScreenPoint(posMarker);
		var screenHeight = posScreen.y;
		if (screenHeight <= 0)
			return;

		grid.screenHeight = screenHeight;
		var vertices = grid.SurfaceGrid();

		var cameraPos = transform.position;
		var m = grid.transform.worldToLocalMatrix;
		for (var i = 0; i < vertices.Length; i++) {
			var v = vertices[i];
			var d = v - cameraPos;
			var t = Distance(cameraPos, d);
			vertices[i] = m.MultiplyPoint3x4(cameraPos + t * d);
		}

		grid.Fill();
	}

	float ForwardDistance(float dist) {
		var posCamera = transform.position;
		var cy = posCamera.y - surfaceHeight;
		var sqrX = dist * dist - cy * cy;
		if (sqrX < 0)
			return -1f;
		return Mathf.Sqrt(sqrX);
	}

	float Distance(Vector3 origin, Vector3 direction) {
		var nDotVd = direction.y;
		var nDotVo = origin.y;
		if (-EPSILON < nDotVd && nDotVd < EPSILON)
			return -1f;
		return -(nDotVo + surfaceHeight) / nDotVd;
	}
}
