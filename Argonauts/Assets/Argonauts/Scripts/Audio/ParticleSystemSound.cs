using System.Collections;
using UnityEngine;

public class ParticleSystemSound : MonoBehaviour
{
    public string[] _explosionSounds;
    public float _explosionPitchMax = 1.25f;
    public float _explosionPitchMin = 0.75f;
    public float _explosionVolumeMax = 0.75f;
    public float _explosionVolumeMin = 0.25f;

    private ParticleSystem.Particle[] particles;
    private int length;
    private int i;

    private void OnEnable()
    {
        particles = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount];
        length = GetComponent<ParticleSystem>().GetParticles(particles);
        i = 0;

        StartCoroutine(Worker());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Worker()
    {
        i = 0;
        while (i < length) {
            if (_explosionSounds.Length > 0 && particles[i].remainingLifetime < Time.deltaTime) {
                AudioWrapper.I.PlaySfx(_explosionSounds[Random.Range(0, _explosionSounds.Length)], Random.Range(_explosionVolumeMax, _explosionVolumeMin), Random.Range(_explosionPitchMin, _explosionPitchMax), particles[i].position);
            }
            i++;
            yield return null;
        }
    }
}
