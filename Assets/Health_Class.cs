using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthClass
{
    private int _health;
    private int _maxHealth;
    public HealthClass(int maxHealth)
    {
        this._maxHealth = maxHealth;
        _health = maxHealth;
    }

    public int GetHealth()
    {
        return _health;
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        if (_health < 0) _health = 0;
        
    }

    public void Heal(int healAmount)
    {
        _health += healAmount;
        if (_health > _maxHealth) _health = _maxHealth;
    }
    
}
