using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter : MonoBehaviour
{
    public ParticleSystem particles;
    public Sound sound;
    public float soundVolume;
    public float soundPitch;
    public int burstSize;
    public int bursts;
    public float time;
    public float jitter = 0.5f;

    public IEnumerator EmitBursts()
    {
        yield return new WaitForSeconds(1);
        float deltaT = time / bursts;
        for (int i = 0; i < bursts; i++)
        {
            if (!transform.gameObject.activeInHierarchy)
            {
                break;
            }
            particles.Emit(burstSize);
            AudioMan.PlaySound(sound, soundVolume, soundPitch);
            float jitter = Random.Range(0f, this.jitter);
            yield return new WaitForSeconds(deltaT + jitter);
        }
    }
}
