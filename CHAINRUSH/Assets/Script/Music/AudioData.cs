/*=====
<AudioData.cs> 
└作成者：banno

＞内容
AudioSourceで鳴らす音のデータの一元管理用スクリプト

＞注意事項   

＞更新履歴
Y25  
_M05   
__D       
___31:プログラム作成:banno   

=====*/

using UnityEngine;
using System.Collections.Generic;

// ScriptableObject生成のための宣言
[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]
public class AudioData : ScriptableObject
{
    
    [System.Serializable, Tooltip("BGMやSEのファイル名と再生クリップ管理用クラス")]
    public class NamedClip  
    {
        public string name;
        public AudioClip audioClip;
    }

    [Header("BGM・SE管理用リスト")]
    public List<NamedClip> bgmClips;
    public List<NamedClip> seClips;


    /*＞BGM・SE取得用関数
     引数:string : 名前  
     ｘ
     戻値：なし
     ｘ
     概要：BGMManagerやSEManagerで音を再生する際に呼び出されます
            再生したい音楽ファイルがList内にあるか探し、
            あった場合はそのファイルを取得して呼び出し元の引き渡します
     */
    public AudioClip GetBGM(string name) =>
        bgmClips.Find(c => c.name == name)?.audioClip;  // ラムダ式を使って探索・取得を短くしています

    public AudioClip GetSE(string name) =>
        seClips.Find(c => c.name == name)?.audioClip;

}
