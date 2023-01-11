using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class RigWeightHandler : MonoBehaviour
{
    public List<Rig> Rigs;
   
    public void LerpWeight(Rig targetRig, float desiredValue, float desiredTime)
    {
       targetRig.weight =  Mathf.Lerp(targetRig.weight, desiredValue, desiredTime);
        
    }
   
}
