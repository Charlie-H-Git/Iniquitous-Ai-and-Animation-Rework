using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public HealthClass enemyHealth;
    public int maxHealth;
    public int health;
    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = new HealthClass(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        health = enemyHealth.GetHealth();
    }
}
