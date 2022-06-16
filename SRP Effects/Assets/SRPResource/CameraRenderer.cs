using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{

	ScriptableRenderContext context;

	Camera camera;
	const string buffer_name = "RenderCamera";

	CommandBuffer buffer = new CommandBuffer{
		name = buffer_name
	};

	CullingResults culling_result;

	public void Render(ScriptableRenderContext context, Camera camera)
	{
		this.context = context;
		this.camera = camera;

		if (!Cull()) {
			return;
		}
		Setup();

        // draw visble geometry
        Draw();

        Submit();
	}

	static ShaderTagId shader_tag = new ShaderTagId("SRPDefaultUnlit");

	void Draw() {
		// render opaque objects
		SortingSettings sortingSettings = new SortingSettings(camera){
			criteria = SortingCriteria.CommonOpaque
		};
		DrawingSettings  drawingSettings = new DrawingSettings(
			shader_tag, sortingSettings
		);
		FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

		context.DrawRenderers(
			culling_result, ref drawingSettings, ref filteringSettings
		);

		context.DrawSkybox(camera);

		// render transparent obejcts
		sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        context.DrawRenderers(culling_result, ref drawingSettings, ref filteringSettings);
	}

	void Setup() {
		context.SetupCameraProperties(camera);
		buffer.ClearRenderTarget(true, true, Color.clear);
		buffer.BeginSample(buffer_name);
		ExecuteBuffer();

	}

    void Submit(){
		buffer.EndSample(buffer_name);
		ExecuteBuffer();
		context.Submit();
    }

	void ExecuteBuffer () {
		context.ExecuteCommandBuffer(buffer);
		buffer.Clear();
	}


	bool Cull() {
		if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)) {
			culling_result = context.Cull(ref p);
			return true;
		}
		return false;
	}
}