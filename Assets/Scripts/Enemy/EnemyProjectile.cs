using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage;
    public float speed;
    public Vector2 direction;

    public int getDamage()
    {
        return damage;
    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.tag == "PlayerTag"))
        {
            Destroy(gameObject);
        }
    }
}
