using UnityEngine;

/*=====
<Player.cs>
└作成者：saito

＞内容
潮の満ち引きを管理するスクリプト

＞注意事項


＞更新履歴
Y25   
_M06    
__D     
___06:プログラム作成:saito   
     :潮の満ち引きのプログラム完成:saito

=====*/

public class SeaRhythm : MonoBehaviour
{
    // 潮の引く速度
    [SerializeField] private float f_Speed;
    // 満潮の位置
    [SerializeField] private Vector3 f_MaxPosition;
    private Vector3 f_StartPosition;
    private float Distance;
    private bool b_Start;       // 潮を引くか満たすか
    void Start()
    {
        f_StartPosition = transform.position;
        // スタート地点と到達地点との距離を計算
        Distance = Vector3.Distance(f_StartPosition, f_MaxPosition);
        b_Start = true;
    }

    void Update()
    {
        //  0～1の範囲を往復
        float t = Mathf.PingPong(Time.time / f_Speed, 1.0f);
        // MaxPositionまで線形補間
        transform.position = Vector3.Lerp(f_StartPosition, f_MaxPosition, t);
    }
}
