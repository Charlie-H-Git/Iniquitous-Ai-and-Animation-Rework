using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public int enemyToAttack;

    public List<AiAgent> isAttacking = new List<AiAgent>();

    private void Start()
    {
        StartCoroutine(AttackTick());
    }

    public void RegisterAttacker(AiAgent agent)
    {
        if (!isAttacking.Contains(agent))
        {
            isAttacking.Add(agent);
        }
    }

    public void DeRegisterAttacker(AiAgent agent)
    {
        if (isAttacking.Contains(agent))
        {
            isAttacking.Remove(agent);
        }
    }

    private IEnumerator AttackTick()
    {
        yield return new WaitForSeconds(5);
        int randomInt = Random.Range(0, isAttacking.Count + 1);
        enemyToAttack = randomInt;
    }
    private void Update()
    {
        
    }
}
