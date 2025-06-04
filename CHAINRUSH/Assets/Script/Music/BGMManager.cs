/*=====
<BGMManager.cs> 
└作成者：banno

＞内容
BGMを取得して、再生するための管理・使用スクリプト

＞注意事項   
他のスクリプトで使用する場合は、BGMManager.Instance.Play("AudioDataのリストに格納した名前")で呼び出せます
止める場合はBGMManager.Instance.Stopで止めることができます

＞更新履歴
Y25   
_M05    
__D       
___31:プログラム作成banno   

=====*/

using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [Tooltip("AudioDataを参照する")] public AudioData audioData;
    [Tooltip("BGM再生用の変数")] private AudioSource audioSource;

    /*＞Awake関数
    引数：なし   
    ｘ
    戻値：なし
    ｘ
    概要：このオブジェクトが生成された時に一度だけ処理を行う
    */
    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;    // 初期の時点でループを実行しておく
    }

    /*＞Play関数
        引数：string : 名前   
        ｘ
        戻値：なし
        ｘ
        概要：再生したいBGMファイルを名前指定で読み込んで再生する関数
        */
    public void Play(string name)
    {
        AudioClip clip = audioData.GetBGM(name);
        if (clip == null) return;

        // 同じ曲ならもう一度再生しないようにする
        if (audioSource.clip == clip && audioSource.isPlaying) return;
        
        audioSource.clip = clip;
        audioSource.Play();
        
    }

    /*＞Stop関数
        引数：なし
        ｘ
        戻値：なし
        ｘ
        概要：再生しているファイルを止める用の関数
        */
    public void Stop()
    {
        audioSource.Stop();
        audioSource = null;
    }

}
