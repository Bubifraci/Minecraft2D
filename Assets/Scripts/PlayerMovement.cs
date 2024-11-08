using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    float speedX, speedY;
    Rigidbody2D rb;
    Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(speedX, speedY).normalized * movementSpeed * Time.deltaTime;
        setAnim();
    }

    private void setAnim()
    {
        bool movingHorizontal = false;
        bool movingVertical = false;

        if(speedX != 0)
        {
            movingHorizontal = true;
            if(speedX < 0)
            {
                animator.SetInteger("movState", 2);

            } else
            {
                animator.SetInteger("movState", 1);
            }
            
        }

        if (speedY != 0)
        {
            movingVertical = true;
            if (speedY < 0)
            {
                animator.SetInteger("movState", 3);

            }
            else
            {
                animator.SetInteger("movState", 4);
            }

        }

        if (!movingHorizontal && !movingVertical)
        {
            animator.SetInteger("movState", 0);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        print(other.GetType().ToString());
    }
}
