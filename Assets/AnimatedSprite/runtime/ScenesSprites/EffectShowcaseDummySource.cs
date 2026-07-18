using UnityEngine;


namespace FlexiSprite
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class EffectShowcaseDummySource : MonoBehaviour
    {
        [SerializeField] private AnimatedSpriteShader dummy;

        [Tooltip("If true, forces all effect enables OFF on the dummy (recommended).")]
        [SerializeField] private bool keepAllEffectsDisabled = true;

        void OnEnable()
        {
            if (!dummy) dummy = GetComponent<AnimatedSpriteShader>();
            if (keepAllEffectsDisabled && dummy)
                dummy.DeactivateAllEffects();
        }

        void OnValidate()
        {
            if (!dummy) dummy = GetComponent<AnimatedSpriteShader>();
            if (keepAllEffectsDisabled && dummy)
                dummy.DeactivateAllEffects();
        }

        [ContextMenu("Deactivate All Effects (Dummy)")]
        void Deactivate()
        {
            if (!dummy) dummy = GetComponent<AnimatedSpriteShader>();
            if (dummy) dummy.DeactivateAllEffects();
        }

        public AnimatedSpriteShader Dummy => dummy;
    }
}
