using UnityEngine;
using System.Collections;

public class TempLevelEditor : MonoBehaviour
{
	public static SpriteRenderer selectedObjectRenderer = null;

	public void Update()
	{
		UpdateInput();
	}

	private void UpdateInput()
	{
		if(Input.GetKeyDown(KeyCode.D) && selectedObjectRenderer != null)
		{
			Destroy(selectedObjectRenderer.gameObject);

			selectedObjectRenderer = null;
		}
	}

	public static void SetSelectedObject(GameObject target)
	{
		if(target == null)
			return;

		selectedObjectRenderer = target.GetComponent<SpriteRenderer>();
	}

	public void OnPostRender()
	{
		if(selectedObjectRenderer == null)
			return;
		
		Material selectionMaterial = new Material("Shader \"Lines/Colored Blended\" { " +
		                              "SubShader { Pass { " +
		                              "Blend SrcAlpha OneMinusSrcAlpha " +
		                              "ZWrite Off Cull Off Fog { Mode Off } " +
		                              "BindChannels { Bind \"vertex\", vertex Bind \"color\", color } " +
		                              "} } }");
		
		selectionMaterial.hideFlags = HideFlags.HideAndDontSave;
		selectionMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		
		selectionMaterial.SetPass(0);
		
		GL.Begin(GL.LINES);
		GL.Color(new Color(0.11f, 0.7f, 0.85f)); // Baby Blue

		// Top Line
		GL.Vertex3(selectedObjectRenderer.bounds.min.x, selectedObjectRenderer.bounds.max.y, 0f);
		GL.Vertex3(selectedObjectRenderer.bounds.max.x, selectedObjectRenderer.bounds.max.y, 0f);

		// Right Line
		GL.Vertex3(selectedObjectRenderer.bounds.max.x, selectedObjectRenderer.bounds.max.y, 0f);
		GL.Vertex3(selectedObjectRenderer.bounds.max.x, selectedObjectRenderer.bounds.min.y, 0f);

		// Bottom Line (heh)
		GL.Vertex3(selectedObjectRenderer.bounds.max.x, selectedObjectRenderer.bounds.min.y, 0f);
		GL.Vertex3(selectedObjectRenderer.bounds.min.x, selectedObjectRenderer.bounds.min.y, 0f);

		// Left Line
		GL.Vertex3(selectedObjectRenderer.bounds.min.x, selectedObjectRenderer.bounds.min.y, 0f);
		GL.Vertex3(selectedObjectRenderer.bounds.min.x, selectedObjectRenderer.bounds.max.y, 0f);
		
		GL.End();
	}
}
