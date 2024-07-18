using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;

    public static ParticlesController INstance;

    private void Start()
    {
        INstance = this;
    }

    public void SetLevel(int lvl)
    {
        for(var i = 0 ; i < particles.Length; i++)
        {
            if (lvl % i == 0)
            {
                particles[i].Play();
            }
            else
            {
                particles[i].Pause();
            }
        }
    }
}
