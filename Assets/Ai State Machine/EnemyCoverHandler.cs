using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class EnemyCoverHandler : MonoBehaviour
{
   public AiAgent agent;
   public SphereCollider Collider;
   public float FieldOfView = 90f;
   public LayerMask LineOfSightLayers;

   public delegate void GainSightEvent(Transform Target, AiAgent agent);

   public GainSightEvent OnGainSight;

   public delegate void LoseSightEvent(Transform Target, AiAgent agent);

   public LoseSightEvent OnLoseSight;

   private Coroutine CheckForLineOfSightCoroutine;

   private void Awake()
   {
      Collider = GetComponent<SphereCollider>();
   }

   private void OnTriggerEnter(Collider other)
   {
      print(other.name);
      if (!CheckLineOfSight(other.transform))
      {
         CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(other.transform));
      }
   }

   private void OnTriggerExit(Collider other)
   {
      OnLoseSight?.Invoke(other.transform, agent);
      if (CheckForLineOfSightCoroutine != null)
      {
         StopCoroutine(CheckForLineOfSightCoroutine);
      }
   }

   public bool CheckLineOfSight(Transform Target)
   {
      Vector3 direction = (Target.transform.position - transform.position).normalized;
      float dotProduct = Vector3.Dot(transform.forward, direction);
      if (dotProduct >= Mathf.Cos(FieldOfView))
      {
         if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Collider.radius, LineOfSightLayers))
         {
           OnGainSight?.Invoke(Target, agent);
           return true;
         }
      }

      return false;
   }

   private IEnumerator CheckForLineOfSight(Transform Target)
   {
      WaitForSeconds wait = new WaitForSeconds(0.5f);
      while (!CheckLineOfSight(Target))
      {
         yield return wait;
      }
   }
}
