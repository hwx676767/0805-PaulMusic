using UnityEngine;

namespace InkCloud
{
    [ExecuteAlways, RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    [DisallowMultipleComponent]
    public sealed class InkCloudBackground : MonoBehaviour
    {
        [Header("Camera")]
        [Tooltip("Assign the gameplay camera. When empty, uses MainCamera.")]
        public Camera targetCamera;
        [Min(0.1f)] public float distance = 50;
        [Header("Art Direction")]
        public Color paperColor = new Color(0.985f, 0.987f, 0.988f, 1);
        public Color inkColor = new Color(0.40f, 0.48f, 0.55f, 1);
        [Range(0,2)] public float density = 1;
        [Range(0.5f,1.5f)] public float coverage = 0.85f;
        [Range(0,1)] public float watercolorVariation = 0.65f;
        [Header("Motion And Print")]
        [Range(0,2)] public float flowSpeed = 0.6f;
        [Range(0,1.5f)] public float cloudTravel = 1;
        [Tooltip("Protected lower-left track height in screen UVs; left 70% stays ink-free below this height.")]
        [Range(0.15f,0.45f)] public float whiteSpace = 0.28f;
        [Range(80,600)] public float dotsAcrossWidth = 340;
        [Range(0,1)] public float halftoneStrength = 0.85f;
        public float seed = 7;
        public bool animateInEditor = true;
        [Min(0)] public float previewTime = 0;
        private MeshRenderer meshRenderer;
        private MaterialPropertyBlock block;

        private void OnEnable() { Refresh(); }
        private void LateUpdate() { Refresh(); }
        private void OnValidate()
        {
            density = Mathf.Clamp(density,0,2);
            coverage = Mathf.Clamp(coverage,0.5f,1.5f);
        }
        private void Refresh()
        {
            if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
            if (block == null) block = new MaterialPropertyBlock();
            Camera cam = targetCamera ? targetCamera : Camera.main;
            float aspect = cam ? cam.aspect : 16f/9f;
            if (cam)
            {
                float depth = Mathf.Clamp(distance,cam.nearClipPlane+0.01f,cam.farClipPlane*0.95f);
                float height = cam.orthographic ? cam.orthographicSize*2 : 2*depth*Mathf.Tan(cam.fieldOfView*Mathf.Deg2Rad*0.5f);
                transform.SetPositionAndRotation(cam.transform.position+cam.transform.forward*depth,cam.transform.rotation);
                transform.localScale = new Vector3(height*aspect*1.002f,height*1.002f,1);
            }
            double time = Time.timeAsDouble;
            #if UNITY_EDITOR
            if (!Application.isPlaying) time = animateInEditor ? UnityEditor.EditorApplication.timeSinceStartup : previewTime;
            #endif
            meshRenderer.GetPropertyBlock(block);
            block.SetColor("_PaperColor",paperColor);
            block.SetColor("_InkColor",inkColor);
            block.SetFloat("_Density",density);
            block.SetFloat("_Coverage",coverage);
            block.SetFloat("_Detail",watercolorVariation);
            block.SetFloat("_Speed",flowSpeed);
            block.SetFloat("_Travel",cloudTravel);
            block.SetFloat("_WhiteSpace",whiteSpace);
            block.SetFloat("_DotCount",dotsAcrossWidth);
            block.SetFloat("_DotStrength",halftoneStrength);
            block.SetFloat("_Seed",seed);
            block.SetFloat("_Aspect",aspect);
            block.SetFloat("_CloudTime",(float)time);
            meshRenderer.SetPropertyBlock(block);
        }
    }
}
