using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace RPG.Entities
{
    public class Sound
    {
        public string Name { get; private set; }
        SoundEffect sound;
        SoundEffectInstance instance;

        public Sound(SoundEffect sound)
        {
            this.Name = sound.Name;
            this.sound = sound;
            this.instance = sound.CreateInstance();
        }
        public void Play()
        {
            instance.Play();
        }
    }
}
