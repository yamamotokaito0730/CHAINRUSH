/*=====
<UIManager.cs>
└作成者：yamamoto

＞内容
UIの表示非表示を管理

＞注意事項


＞更新履歴
Y25   
_M07    
__D     
___16:プログラム作成:yamamoto  
=====*/

using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject[] uiElements; // 表示・非表示対象のUI

    private void Start()
    {
        HideUI(); // ゲーム開始時は非表示
    }

    public void ShowUI()
    {
        foreach (var element in uiElements)
            element.SetActive(true);
    }

    public void HideUI()
    {
        foreach (var element in uiElements)
            element.SetActive(false);
    }
}
