using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Modify the given vertices so they follow a sin curve
 */
[ExecuteInEditMode]
public class VertBend : BaseMeshEffect
{
	protected readonly float canvasWidth=3072f;
	public float scale = 100f;
	RectTransform myRectTransform;
	protected override void OnEnable(){
		myRectTransform=GetComponent<RectTransform>();
	}

	public override void ModifyMesh (VertexHelper vertexHelper)
	{
		if (!this.IsActive())
			return;

		List<UIVertex> verts = new List<UIVertex>();
		vertexHelper.GetUIVertexStream(verts);

		for (int index = 0; index < verts.Count; index++) {
			UIVertex uiVertex = verts [index];
			float xPercent = Mathf.Clamp01((uiVertex.position.x / (myRectTransform.rect.width*.5f) + 1f)/2f);
			float newZ = Mathf.Sin (xPercent * Mathf.PI)*scale;
			uiVertex.position.z = newZ;
			verts [index] = uiVertex;
		}

		vertexHelper.Clear();
		vertexHelper.AddUIVertexTriangleStream(verts);
	}

	public void Update(){
		//		Debug.Log ("update");
		graphic.SetVerticesDirty();
	}
}
