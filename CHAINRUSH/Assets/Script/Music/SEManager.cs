/*=====
<SEManager.cs> 
└作成者：banno

＞内容
SEを取得して、再生するための管理・使用スクリプト

＞注意事項   
他のスクリプトで使用する場合は、SEManager.Instance.Play("AudioDataのリストに格納した名前")
で呼び出せます

＞更新履歴
Y25   
_M05    
__D       
___31:プログラム作成banno   

=====*/
using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance { get; private set; }

    [Tooltip("AudioDataを参照する")] public AudioData audioData;
    [Tooltip("SE再生用の変数")] private AudioSource audioSource;

    /*＞Awake関数
    引数：なし   
    ｘ
    戻値：なし
    ｘ
    概要：このオブジェクトが生成された時に一度だけ処理を行う
    */
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    /*＞Play関数
        引数：string : 名前   
        ｘ
        戻値：なし
        ｘ
        概要：再生したいSEファイルを名前指定で読み込んで再生する関数
        */
    public void Play(string name)
    {
        AudioClip clip = audioData.GetSE(name);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);  // 同時に10個ぐらい(パソコンにもよるけど)までならSEを鳴らせる
        }
    }
}
