using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDamageController : MonoBehaviour
{
    public ParticleSystem fireParticleSystemRight;
    public ParticleSystem fireParticleSystemLeft;
    public ParticleSystem smokeParticleSystem;

    private void Start()
    { 
        disableParticleSystem();
    }

    public void updateDamageEffect(float healthPercentage)
    {
        if ((healthPercentage < 50) && !particleSystemEnabled(fireParticleSystemRight))
        {
            enableFireStage1();
        }
        else if(healthPercentage < 30 && !particleSystemEnabled(fireParticleSystemLeft))
        {
            enableFireStage2();
        }
        else if(healthPercentage < 15 && !particleSystemEnabled(smokeParticleSystem))
        {
            enableSmokeStage();
        }
    }

    private bool particleSystemEnabled(ParticleSystem particleSystem)
    {
        bool isParticleSystemEnabled = particleSystem.emission.enabled;

        if (isParticleSystemEnabled == false)
            return false;
        return true;
    }

    private void disableParticleSystem()
    {
        var emissionFireRight = fireParticleSystemRight.emission;
        emissionFireRight.enabled = false;

        var emissionFireLeft = fireParticleSystemLeft.emission;
        emissionFireLeft.enabled = false;

        var emissionSmoke = smokeParticleSystem.emission;
        emissionSmoke.enabled = false;

    }

    public void enableFireStage1()
    {
        var emissionFire = fireParticleSystemRight.emission;
        emissionFire.enabled = enabled;
    }

    public void enableFireStage2()
    {
        var emissionFire = fireParticleSystemLeft.emission;
        emissionFire.enabled = enabled;
    }

    public void enableSmokeStage()
    {
        var emissionSmoke = smokeParticleSystem.emission;
        emissionSmoke.enabled = enabled;
    }

 
}
