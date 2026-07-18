using System.Reflection;
using UnityEngine;



namespace FlexiSprite
{
    public partial class AnimatedSpriteShader
    {
        
        // Cached reflection handles for UnityEngine.UI.Image so we avoid a hard assembly dependency.

        static System.Type _imageType;
        static PropertyInfo _imageSpriteProp;
        static PropertyInfo _imageMaterialProp;
        static PropertyInfo _imageColorProp;
        static bool _imageReflectionInit;

        static void EnsureImageReflection()
        {
            if (_imageReflectionInit) return;
            _imageReflectionInit = true;
            _imageType = System.Type.GetType("UnityEngine.UI.Image, UnityEngine.UI");
            if (_imageType != null)
            {
                _imageSpriteProp = _imageType.GetProperty("sprite");
                // Image.material lives on UnityEngine.UI.Graphic
                _imageMaterialProp = _imageType.GetProperty("material");
                _imageColorProp = _imageType.GetProperty("color");
            }
        }

        /// <summary>True if UGUI Image type was loaded successfully.</summary>
        public static bool UIAvailable { get { EnsureImageReflection(); return _imageType != null; } }

        /// <summary>Returns the UGUI Image component on this GameObject (or null). Reflection-based.</summary>
        public Component GetUIImage()
        {
            EnsureImageReflection();
            if (_imageType == null) return null;
            return GetComponent(_imageType);
        }

        Sprite GetImageSprite(Component img)
        {
            EnsureImageReflection();
            if (img == null || _imageSpriteProp == null) return null;
            return _imageSpriteProp.GetValue(img) as Sprite;
        }

        void SetImageSprite(Component img, Sprite s)
        {
            EnsureImageReflection();
            if (img == null || _imageSpriteProp == null) return;
            _imageSpriteProp.SetValue(img, s);
        }

        Material GetImageMaterial(Component img)
        {
            EnsureImageReflection();
            if (img == null || _imageMaterialProp == null) return null;
            return _imageMaterialProp.GetValue(img) as Material;
        }

        void SetImageMaterial(Component img, Material mat)
        {
            EnsureImageReflection();
            if (img == null || _imageMaterialProp == null) return;
            _imageMaterialProp.SetValue(img, mat);
        }

        /// <summary>True if current variant targets UGUI Image rather than a sprite/mesh renderer.</summary>
        public bool IsGUIMode => PipelineRouting.IsVariantGUI(_shaderVariant);

        /// <summary>
        /// Ensures the GameObject has the correct render component for the given variant.
        /// - GUI variant: adds an Image (if UGUI available) and removes any SpriteRenderer, migrating the sprite.
        /// - Non-GUI variant: adds a SpriteRenderer and removes any Image, migrating the sprite.
        /// </summary>
        public void SyncRendererForVariant(ShaderVariant v)
        {
            bool wantsGUI = PipelineRouting.IsVariantGUI(v);
            var sr = GetComponent<SpriteRenderer>();
            var img = GetUIImage();

            if (wantsGUI)
            {
                if (!UIAvailable)
                {
                    Debug.LogWarning("[AnimSprite] GUI shader variant selected but UGUI (UnityEngine.UI) is unavailable. " +
                                     "Keeping SpriteRenderer — add the UGUI package / set up a Canvas to use GUI variants.");
                    return;
                }

                Sprite spriteToKeep = sr ? sr.sprite : GetImageSprite(img);

                if (img == null)
                {
                    img = gameObject.AddComponent(_imageType);
                }
                if (spriteToKeep != null)
                    SetImageSprite(img, spriteToKeep);

                if (sr != null) DestroyRendererSafe(sr);
                // Invalidate renderer handles — the old one is gone.
                _rend = null;
            }
            else
            {
                Sprite spriteToKeep = null;
                if (img != null) spriteToKeep = GetImageSprite(img);
                if (spriteToKeep == null && sr != null) spriteToKeep = sr.sprite;

                var existingRenderer = GetComponent<Renderer>();
                if (sr == null && existingRenderer == null)
                    sr = gameObject.AddComponent<SpriteRenderer>();

                if (spriteToKeep != null && sr != null)
                    sr.sprite = spriteToKeep;

                if (img != null) DestroyRendererSafe(img);
                _rend = null;
            }

            InvalidatePushStrategy();
        }

        static void DestroyRendererSafe(Object obj)
        {
            if (!obj) return;
    #if UNITY_EDITOR
            if (!Application.isPlaying) Object.DestroyImmediate(obj);
            else Object.Destroy(obj);
    #else
            Object.Destroy(obj);
    #endif
        }

        /// <summary>
        /// Material-target abstraction: gets the material from either Renderer.sharedMaterial or Image.material
        /// depending on current variant.
        /// </summary>
        Material GetTargetMaterial()
        {
            if (IsGUIMode)
            {
                var img = GetUIImage();
                return img != null ? GetImageMaterial(img) : null;
            }
            return Rend ? Rend.sharedMaterial : null;
        }

        void SetTargetMaterial(Material mat)
        {
            if (IsGUIMode)
            {
                var img = GetUIImage();
                if (img != null) SetImageMaterial(img, mat);
            }
            else if (Rend)
            {
                Rend.sharedMaterial = mat;
            }
        }
    }
}
