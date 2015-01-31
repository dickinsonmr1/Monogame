using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Entities
{
    public class AudioManager
    {
        Dictionary<string, Sound> sounds;

        public AudioManager()
        {
            this.sounds = new Dictionary<string, Sound>();
        }

        public void AddSound(Sound sound)
        {
            this.sounds.Add(sound.Name, sound);
        }

        public void PlaySound(string name)
        {
            var sound = this.sounds.Where(s => s.Key == name).FirstOrDefault();
            if (sound.Value != null)
            {
                sound.Value.Play();
            }
        }
    }
}
