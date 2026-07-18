#if GLP_HAS_URP

using UnityEngine;
namespace GlitchLabPro
{
    public class Rotate_Cube : MonoBehaviour
    {
        public Vector3 speed;

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(speed);
        }
    }
}
#endif
