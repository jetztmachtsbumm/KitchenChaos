using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float moveSpeed;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);

        animator.SetBool("IsWalking", moveDir != Vector3.zero);
    }

}
