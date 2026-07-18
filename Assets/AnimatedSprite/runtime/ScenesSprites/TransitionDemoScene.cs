using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace FlexiSprite
{
    public class TransitionDemoScene : MonoBehaviour
    {
        [SerializeField] private List<GameObject> transitionObjects; // scene images sitting behind fullscreenSprite
        [SerializeField] private SpriteRenderer fullscreenSprite;    // black fullscreen sprite with AnimatedSpriteShader

        [SerializeField] private float holdDuration = 0.8f;
        [SerializeField] private float fadeDuration = 0.8f;

        private AnimatedSpriteShader _shader;
        private EffectDef[] _dissolveEffects;

        void Start()
        {
            _shader = fullscreenSprite.GetComponent<AnimatedSpriteShader>();

            // Collect every dissolve-category effect from the registry
            var list = new List<EffectDef>();
            foreach (var def in EffectRegistry.All)
                if (def.category == EffectCategory.Dissolve)
                    list.Add(def);
            _dissolveEffects = list.ToArray();

            // Scene images always idle, only the first one active at start
            for (int i = 0; i < transitionObjects.Count; i++)
            {
                transitionObjects[i].SetActive(i == 0);
                var anim = transitionObjects[i].GetComponent<Animator>();
                if (anim != null) anim.Play("idle");
            }



            StartCoroutine(LoopTransitions());
        }

        private IEnumerator LoopTransitions()
        {
            int sceneIdx = 0;
            int dissolveIdx = 0;

            while (true)
            {
                if (_dissolveEffects[dissolveIdx % _dissolveEffects.Length].id == EffectId.AlphaCutoff)
                {
                    dissolveIdx++;
                    continue;
                }
                SetDissolveEffect(dissolveIdx % _dissolveEffects.Length);
                SetDissolveEffectAmount(dissolveIdx % _dissolveEffects.Length, 1f);

                yield return new WaitForSeconds(holdDuration);

         
                yield return AnimateFade(1f, 0f, fadeDuration, dissolveIdx % _dissolveEffects.Length);


                int nextScene = (sceneIdx + 1) % transitionObjects.Count;
                transitionObjects[sceneIdx].SetActive(false);
                transitionObjects[nextScene].SetActive(true);
           

        
                yield return AnimateFade(0f, 1f, fadeDuration, dissolveIdx % _dissolveEffects.Length);

                sceneIdx = nextScene;
                dissolveIdx++;
            }
        }


        private void SetDissolveEffect(int effectIndex)
        {
            foreach (var def in _dissolveEffects)
                SetField(def.enableField, false);

            SetField(_dissolveEffects[effectIndex].enableField, true);
            _shader.IncrementalPush();
        }

        private void SetDissolveEffectAmount(int effectIndex , float t)
        {
            SetField(_dissolveEffects[effectIndex].parms.Where(t=> t.field.Contains("Amount")).First().field, t);
            _shader.IncrementalPush();
        }


        private void SetField(string fieldName, bool value)
        {
            FieldInfo field = typeof(AnimatedSpriteShader)
                .GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            field?.SetValue(_shader, value);
        }

        private void SetField(string fieldName, float value)
        {
            FieldInfo field = typeof(AnimatedSpriteShader)
                .GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            field?.SetValue(_shader, value);
        }


        private IEnumerator AnimateFade(float from, float to, float duration, int effectIdx)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                SetDissolveEffectAmount(effectIdx, Mathf.Lerp(from, to, elapsed / duration));
                yield return null;
            }
            SetDissolveEffectAmount(effectIdx, to);
        }
    }
    }
