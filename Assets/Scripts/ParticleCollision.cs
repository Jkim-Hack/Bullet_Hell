using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision: MonoBehaviour
{
    ParticleSystem particleSystem;
    public List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "EnemyTag") //Should make super class enemy
        {
            int numCollisions = particleSystem.GetCollisionEvents(other, collisionEvents);


            var enemyController = other.GetComponent<EnemyController>();

            int i = 0;
            while (i < numCollisions)
            {
                enemyController.life = enemyController.life - .1f;
                print("Life after Flamethrower damage " + enemyController.life);
                StartCoroutine(enemyController.blink());
                i++;
            }
            
        }
    }
}
