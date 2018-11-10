///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2018 ///
///////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/3D/Binary")]
public class CameraFilterPack_3D_Binary : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private float TimeX = 1.0f;
	public bool _Visualize=false;
	private Material SCMaterial;
   
    [Range(0f, 100f)]
    public float _FixDistance = 2f;  
   [Range(-5f, 5f)]
    public float LightIntensity = 0f;  
 
    [Range(0f, 8f)]
    public float MatrixSize=2f;
   [Range(-4f, 4f)]
    public float MatrixSpeed=1f;
  
    [Range(0f, 1f)]
    public float Fade = 1f;  
    [Range(0f, 1f)]
    public float FadeFromBinary = 0f;  
    public Color _MatrixColor= new Color(1,0.3f,0.3f,1);
  
	public static Color ChangeColorRGB;
    private Texture2D Texture2;
	#endregion
	
	#region Properties
	Material material
	{
		get
		{
			if(SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
			return SCMaterial;
		}
	}
	#endregion
	void Start () 
	{
            Texture2 = Resources.Load ("CameraFilterPack_3D_Binary1") as Texture2D;
			SCShader = Shader.Find("CameraFilterPack/3D_Binary");

		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}
	
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if(SCShader != null)
		{
			TimeX+=Time.deltaTime;
			if (TimeX>100)  TimeX=0;
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_DepthLevel",Fade);
            material.SetFloat("_FadeFromBinary",FadeFromBinary);
          
            material.SetFloat("_FixDistance",_FixDistance);
            material.SetFloat("_MatrixSize",MatrixSize);
            material.SetColor("_MatrixColor",_MatrixColor);
            material.SetFloat("_MatrixSpeed",MatrixSpeed*2);
            material.SetFloat("_Visualize", _Visualize ? 1 : 0);
            material.SetFloat("_LightIntensity",LightIntensity);
          	material.SetTexture("_MainTex2", Texture2);
    
  
            float _FarCamera = GetComponent<Camera>().farClipPlane; 
			material.SetFloat("_FarCamera",1000/_FarCamera);
          	material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        
			Graphics.Blit(sourceTexture, destTexture, material);

		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}

	void Update () 
	{
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/3D_Binary");
    	Texture2 = Resources.Load ("CameraFilterPack_3D_Binary1") as Texture2D;
		
		}
		#endif

	}
	
	void OnDisable ()
	{
		if(SCMaterial)
		{
			DestroyImmediate(SCMaterial);	
		}
		
	}
	
	
}