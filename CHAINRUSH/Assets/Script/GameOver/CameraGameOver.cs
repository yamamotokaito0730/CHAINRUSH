using UnityEngine;

public class CameraGameOver : MonoBehaviour
{
    private float timer = 0f;
    private enum Phase { Phase1_ArcMove, Phase2_PullBack, Phase3_LookAtMovingPlayer }
    private Phase phase = Phase.Phase1_ArcMove;

    // -------- フェーズ1（アーチ移動）
    private Vector3 phase1Start = new Vector3(-5f, 5f, 0f);
    private Vector3 phase1End = new Vector3(0f, 5f, -5f);
    private float phase1Duration = 1.5f;
    private Vector3 phase1Control;

    // -------- フェーズ2（引き）
    private Vector3 phase2Start = new Vector3(-2.5f, 1.25f, -2.5f);
    private Vector3 phase2End = new Vector3(-5f, 2.5f, -5f);
    private float phase2Duration = 1.5f;

    // -------- プレイヤー参照（移動する前提）
    [SerializeField] private Transform playerTransform;

    void Start()
    {
        phase1Control = (phase1Start + phase1End) / 2f + new Vector3(0f, 2.5f, 0f);

        transform.position = phase1Start;
        if (playerTransform != null)
        {
            transform.LookAt(playerTransform.position);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (phase == Phase.Phase1_ArcMove)
        {
            float t = Mathf.Clamp01(timer / phase1Duration);
            Vector3 pos =
                Mathf.Pow(1 - t, 2) * phase1Start +
                2 * (1 - t) * t * phase1Control +
                Mathf.Pow(t, 2) * phase1End;

            transform.position = pos;

            if (playerTransform != null)
                transform.LookAt(playerTransform.position);

            if (t >= 1f)
            {
                phase = Phase.Phase2_PullBack;
                timer = 0f;
                transform.position = phase2Start;
                if (playerTransform != null)
                    transform.LookAt(playerTransform.position);
            }
        }
        else if (phase == Phase.Phase2_PullBack)
        {
            float t = Mathf.Clamp01(timer / phase2Duration);
            transform.position = Vector3.Lerp(phase2Start, phase2End, t);

            if (playerTransform != null)
                transform.LookAt(playerTransform.position);

            if (t >= 1f)
            {
                phase = Phase.Phase3_LookAtMovingPlayer;
                timer = 0f;
                transform.position = phase2End;
            }
        }
        else if (phase == Phase.Phase3_LookAtMovingPlayer)
        {
            // 固定位置から、動くプレイヤーをずっと注視
            if (playerTransform != null)
                transform.LookAt(playerTransform.position);
        }
    }
}
