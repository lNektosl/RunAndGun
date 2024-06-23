using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour {
    [SerializeField] private Enemy enemy;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Update() {
        animator.SetBool("isMoving",enemy.IsMoving());
        animator.SetBool("isDead", enemy.IsDead());
    }
}
