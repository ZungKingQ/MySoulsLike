using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIK : MonoBehaviour
{
    public Animator animator;
    [Range(0, 1)]
    public float weight;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPosition(AvatarIKGoal.RightFoot, Vector3.zero);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, Vector3.zero);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, weight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, weight);
    }
}
