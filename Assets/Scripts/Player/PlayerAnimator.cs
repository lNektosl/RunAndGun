using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour{
    [SerializeField] Player player;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        animator.SetBool("isMoving", player.IsMoving());
        animator.SetBool("isDead", player.IsDead());
    }
}
