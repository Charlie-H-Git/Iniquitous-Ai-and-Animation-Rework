using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
   private AiAgent _aiAgent;
   
   private void Awake()
   {
      _aiAgent = GetComponentInParent<AiAgent>();
      anim = GetComponent<Animator>();
   }

   private void OnAnimatorMove()
   {
      float delta = Time.deltaTime;
      _aiAgent._enemyRigidbody.drag = 0;
      Vector3 deltaPosition = anim.deltaPosition;
      deltaPosition.y = 0;
      Vector3 velocity = deltaPosition / delta;
      _aiAgent._enemyRigidbody.velocity = velocity;
   }
}
