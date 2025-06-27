/*=====
<TitlePlayer.cs>
└作成者：mori

＞内容
タイトルシーンの処理

＞注意事項


＞更新履歴
Y25         
_M06
__D  
___06:プログラム作成:mori

=====*/

using UnityEngine;

public class TitlePlayer : MonoBehaviour
{

    public float moveSpeed;
    private bool isMoving = false;
    private float posY;

    void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator animator;
        animator = GetComponent<Animator>();
        animator.Update(0f);           // Animatorの内部初期化
        animator.Play("Run_1", 0, 0.0f); // 第2引数: Layer、第3引数: 時間（0秒から）
        animator.Update(0f);           // 再度更新して即座に反映させる
        posY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // エンターキーを押したら移動開始
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isMoving = true;
        }

        // 移動処理
        if (isMoving)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }

        // Y座標が下がってくるので固定にする
        Vector3 pos = transform.position;
        pos.y = posY;
        transform.position = pos;
    }
}
