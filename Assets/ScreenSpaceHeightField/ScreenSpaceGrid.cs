using UnityEngine;
using System.Collections;

public class ScreenSpaceGrid : MonoBehaviour {
	public const int VERTEX_LIMIT = 65000;

	public Camera targetCamera;
	public float screenHeight;
	public int pitchPix = 8;

	private Mesh _mesh;
	private int _prevWidth = -1;
	private int _prevHeight = -1;
	private bool _triangleInvalidated;
	private Vector3[] _vertices;
	private int[] _triangles;

	// Use this for initialization
	void Start () {
		GetComponent<MeshFilter>().sharedMesh = _mesh = new Mesh();
		_mesh.MarkDynamic();
		_mesh.bounds = new Bounds(Vector3.zero, 1e4f * Vector3.one);
	}

	public Vector3[] SurfaceGrid() {
		var width = Mathf.CeilToInt((float)Screen.width / pitchPix);
		var height = Mathf.CeilToInt((float)screenHeight / pitchPix);
		var widthPlusOne = width + 1;
		var heightPlusOne = height + 1;
		var vertexCount = widthPlusOne * heightPlusOne;
		if (vertexCount > VERTEX_LIMIT)
			return null;
		
		_triangleInvalidated = (width != _prevWidth || height != _prevHeight);
		if (_triangleInvalidated) {
			_mesh.Clear();
			_prevWidth = width;
			_prevHeight = height;
			_vertices = new Vector3[vertexCount];
		}

		var m = new Matrix4x4();
		m.SetTRS(new Vector3(-1f, -1f, 0f), Quaternion.identity, new Vector3(2f / Screen.width, 2f / Screen.height, 1f));
		m = targetCamera.cameraToWorldMatrix * targetCamera.projectionMatrix.inverse * m;
		var v = new Vector4(0f, 0f, 0f, 1f);
		for (var y = 0; y <= height; y++) {
			for (var x = 0; x <= width; x++) {
				var i = y * widthPlusOne + x;
				v.x = x * pitchPix;
				v.y = screenHeight - y * pitchPix;
				_vertices[i] = m.MultiplyPoint(v);
			}
		}

		if (_triangleInvalidated) {
			_triangles = new int[6 * width * height];
			var counter = 0;
			for (var y = 0; y < height; y++) {
				for (var x = 0; x < width; x++) {
					var i = y * widthPlusOne + x;
					_triangles[counter++] = i;
					_triangles[counter++] = i + 1;
					_triangles[counter++] = i + + widthPlusOne + 1;
					_triangles[counter++] = i;
					_triangles[counter++] = i + widthPlusOne + 1;
					_triangles[counter++] = i + widthPlusOne;
				}
			}
		}

		return _vertices;
	}

	public void Fill() {
		_mesh.vertices = _vertices;
		if (_triangleInvalidated)
			_mesh.triangles = _triangles;
	}
}
