using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enut4LJR
{
    public interface ISoundPlay
    {
        void SoundPlay(ref AudioClip clip, float volume);
    }
}
