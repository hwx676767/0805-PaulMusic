using UnityEngine;


namespace FlexiSprite
{
    public enum AnimEasing
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        Bounce,
        Elastic
    }

    public static class AnimSpriteEasing
    {
        public static float Apply(float t, AnimEasing e)
        {
            switch (e)
            {
                case AnimEasing.EaseIn:
                    return t * t;

                case AnimEasing.EaseOut:
                    return 1f - (1f - t) * (1f - t);

                case AnimEasing.EaseInOut:
                    return t < 0.5f
                        ? 2f * t * t
                        : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

                case AnimEasing.Bounce:
                    return BounceOut(t);

                case AnimEasing.Elastic:
                    return ElasticOut(t);

                default:
                    return t;
            }
        }

        private static float BounceOut(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            float x = 1f - t;
            float b;

            if (x < 1f / d1)
            {
                b = n1 * x * x;
            }
            else if (x < 2f / d1)
            {
                x -= 1.5f / d1;
                b = n1 * x * x + 0.75f;
            }
            else if (x < 2.5f / d1)
            {
                x -= 2.25f / d1;
                b = n1 * x * x + 0.9375f;
            }
            else
            {
                x -= 2.625f / d1;
                b = n1 * x * x + 0.984375f;
            }

            return 1f - b;
        }

        private static float ElasticOut(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;

            return Mathf.Pow(2f, -10f * t) *
                   Mathf.Sin((t * 10f - 0.75f) * (2f * Mathf.PI) / 3f) +
                   1f;
        }
    }
}
