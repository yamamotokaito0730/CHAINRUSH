using UnityEngine;

public class PlayerGameover : MonoBehaviour
{
    private Animator animator;
    private float timer = 0f;
    private bool secondAnimationPlayed = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("GameOver1", 0, 0.0f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!secondAnimationPlayed && timer >= 1.6f)
        {
            animator.Play("GameOver2", 0, 0.0f);
            secondAnimationPlayed = true;
        }
    }
}
