using UnityEngine;

public class Select : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            GameManager.Instance.LoadStage(0);
            ObjectPoolManager.Instance.ReturnAllToPool();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.LoadStage(1);
            ObjectPoolManager.Instance.ReturnAllToPool();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            GameManager.Instance.LoadStage(2);
            ObjectPoolManager.Instance.ReturnAllToPool();
        }

    }
}
