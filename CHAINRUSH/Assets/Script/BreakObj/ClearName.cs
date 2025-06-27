/*=====
<ClearName.cs>
└作成者：banno

＞内容
プールオブジェクトを戻す際に、(Clone)を削除するための共通関数をもつスクリプト

＞更新履歴
Y25   
_M06
__D
___14:スクリプトを作成:banno
=====*/using UnityEngine;

public static class NameUtility
{
    public static string GetCleanName(GameObject obj)
    {
        return obj.name.Replace("(Clone)", "").Trim();
    }
}
