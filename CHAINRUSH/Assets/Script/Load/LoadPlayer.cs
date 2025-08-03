using UnityEngine;

public class LoadPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator animator;
        animator = GetComponent<Animator>();
        animator.Play("Run_2", 0, 0.0f); // ‘æ2ˆø”: LayerA‘æ3ˆø”: ŠÔi0•b‚©‚çj
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
