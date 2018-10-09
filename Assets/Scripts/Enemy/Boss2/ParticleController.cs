using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {
    public ParticleSystem chargeUp;
    ParticleSystem.Particle[] m_Particles;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (m_Particles == null || m_Particles.Length < chargeUp.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[chargeUp.main.maxParticles];

        int numParticlesAlive = chargeUp.GetParticles(m_Particles);

        if (numParticlesAlive >= 50)
        {

        }
    }

    void IntitializeWhenNeeded()
    {
        if (chargeUp == null)
            chargeUp = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < chargeUp.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[chargeUp.main.maxParticles];
    }
}
