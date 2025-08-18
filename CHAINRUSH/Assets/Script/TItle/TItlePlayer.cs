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
_M08
__D
___03:パッドに対応:tooyama

=====*/

using UnityEngine;
using UnityEngine.InputSystem;

public class TitlePlayer : MonoBehaviour
{

    public float moveSpeed;
    private bool isMoving = false;
    private float posY;
    private Gamepad gamepad; // ゲームパッドを使えるように宣言

    void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator animator;
        animator = GetComponent<Animator>();
        animator.Update(0f);           // Animatorの内部初期化
        animator.Play("Run_2", 0, 0.0f); // 第2引数: Layer、第3引数: 時間（0秒から）
        animator.Update(0f);           // 再度更新して即座に反映させる
        posY = transform.position.y;
        gamepad = Gamepad.current;
    }

    // Update is called once per frame
    void Update()
    {
        gamepad = Gamepad.current; // 毎フレーム更新
        // エンターキーを押したら移動開始
        if (Input.GetKeyDown(KeyCode.Return) || (gamepad != null && gamepad.aButton.wasPressedThisFrame))
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
