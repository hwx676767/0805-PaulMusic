namespace FlexiSprite
{
    public partial class AnimatedSpriteShader
    {
        private ulong _dirtyMask0;
        private ulong _dirtyMask1;
        private ulong _dirtyMask2;

        public void SetEffectDirtyBit(int idx)
        {
            if (idx < 64) _dirtyMask0 |= 1UL << idx;
            else if (idx < 128) _dirtyMask1 |= 1UL << (idx - 64);
            else _dirtyMask2 |= 1UL << (idx - 128);
        }

        private bool AnyEffectDirty() => _dirtyMask0 != 0 || _dirtyMask1 != 0 || _dirtyMask2 != 0;

        private void ClearEffectDirtyMask()
        { _dirtyMask0 = 0; _dirtyMask1 = 0; _dirtyMask2 = 0; }

        private static readonly int[] _debruijn = new int[64];
        private const ulong DBC = 0x03F79D71B4CA8B09UL;

        private static void InitDebruijn()
        { for (int i = 0; i < 64; i++) _debruijn[(DBC << i) >> 58] = i; }

        private static int TrailingZeros(ulong v) => _debruijn[((v & (ulong)-(long)v) * DBC) >> 58];

    }
    }
