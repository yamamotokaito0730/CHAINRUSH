/*=====
<UpdateWallShader.cs>
└作成者：saito

＞内容
壁のテクスチャを、プレイヤーとの距離に応じて表示するための制御スクリプト

＞注意事項


＞更新履歴
Y25   
_M06  
__D     
___13:プログラム作成:saito
=====*/

using UnityEngine;

public class UpdateWallShader : MonoBehaviour
{
    // プレイヤーのTransform（位置情報）
    public Transform player;

    // 壁のRenderer（マテリアルにアクセスするために必要）
    public Renderer wallRenderer;

    // プレイヤーがこの距離以内に来ると表示が始まる
    public float radius = 100.0f;

    void Update()
    {
        if (wallRenderer != null)
        {
            Material mat = wallRenderer.material;
            // プレイヤーの位置をシェーダーに渡す
            mat.SetVector("_PlayerPos", player.position);
            // 表示半径をシェーダーに渡す
            mat.SetFloat("_Radius", radius);

            mat.SetFloat("_FadeStart", 0.0f);   // 表示フェード開始距離
            mat.SetFloat("_FadeEnd", 5.0f);     // 表示フェード終了距離（完全に表示される距離）
        }
    }
}
