/*=====
<LimitZoneUI.cs>
└作成者：saito

＞内容
画面に接近したらUIで警告文を表示するスクリプト

＞注意事項


＞更新履歴
Y25   
_M06  
__D     
___13:プログラム作成:saito
=====*/

using UnityEngine;
using UnityEngine.UI;

public class LimitZoneUI : MonoBehaviour
{
    private Collider[] TransparentWall;  // 画面外の壁
    public Transform player;                        // プレイヤー座標
    float StartUIFade = 3.0f;       // UIを表示する距離
    float FadeSpeed = 3.0f;     // UIを表示する時間
    Text DengerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DengerText = GetComponent<Text>();

        GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("Wall");

        TransparentWall = new Collider[wallObjects.Length];
        for (int i = 0; i < wallObjects.Length; i++)
        {
            TransparentWall[i] = wallObjects[i].GetComponent<Collider>();
            if (TransparentWall[i] == null)
            {
                Debug.LogWarning($"壁 '{wallObjects[i].name}' にColliderがありません");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool nearWall = false;

        foreach (Collider t in TransparentWall)
        {

            // 壁の表面上でプレイヤーに一番近い点を取得
            Vector3 closest = t.ClosestPoint(player.position);
            float distance = Vector3.Distance(player.position, closest);
            
            if(distance < StartUIFade)
            {
                nearWall = true;
                break;  
            }
        }

        float targetAlpha = nearWall ? 1f : 0f;

        // 現在の色
        Color currentColor = DengerText.color;

        // アルファ値だけを滑らかに変化
        currentColor.a = Mathf.MoveTowards(currentColor.a, targetAlpha, FadeSpeed * Time.deltaTime);

        DengerText.color = currentColor;
    }
}
