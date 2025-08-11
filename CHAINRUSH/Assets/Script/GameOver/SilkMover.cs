using UnityEngine;

public class SilkMover : MonoBehaviour
{
    private Vector3 startPos = new Vector3(0f, 0.5f, 55f);
    private Vector3 targetPos = new Vector3(0f, 0.5f, 21f);
    private float timer;

    void Start()
    {
        transform.position = startPos;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < 3.3f)
        {
            // ‘Ò‹@
            transform.position = startPos;
        }
        else if (timer < 4.3f)
        {
            // 3.3•b`4.3•bF‘Oii1•bŠÔj
            float t = (timer - 3.3f) / 1.0f;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
        }
        else if (timer < 4.5f)
        {
            // 4.3•b`4.5•bF’âŽ~i–Ú•WˆÊ’u‚Å‘Ò‹@j
            transform.position = targetPos;
        }
        else if (timer < 5.5f)
        {
            // 4.5•b`5.5•bF–ß‚éi1•bŠÔj
            float t = (timer - 4.5f) / 1.0f;
            transform.position = Vector3.Lerp(targetPos, startPos, t);
        }
        else
        {
            // ‚»‚êˆÈ~‚Í‰ŠúˆÊ’u‚Å’âŽ~
            transform.position = startPos;
        }
    }
}
