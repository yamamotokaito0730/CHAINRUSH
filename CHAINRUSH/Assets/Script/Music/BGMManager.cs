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

using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Tooltip("AudioDataを参照する")] public AudioData audioData;
    [Tooltip("BGM再生用の変数")] private AudioSource audioSource;
    private Coroutine currentFadeCoroutine;

    public float fadeDuration = 1.0f;

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

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;    // 初期の時点でループを実行しておく

        Instance = this;
        DontDestroyOnLoad(gameObject);
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

    // フェードイン再生
    public void PlayBGMWithFade(string name, float fadeDuration = 1f)
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeInBGM(name, fadeDuration));
    }

    // フェードアウトして停止させる
    public void StopBGMWithFade(float fadeDuration = 1f)
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeOutAndStop(fadeDuration));
    }

    // フェードでBGMを切り替える
    public void ChangeBGM(string name, float fadeDuration = 1f)
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeOutAndPlayNew(name, fadeDuration));
    }



    // ---- コルーチン処理 ----

    private IEnumerator FadeInBGM(string name, float duration)
    {
        var clip = audioData.GetBGM(name);
        float targetVolume = audioData.GetBGMVolume(name);

        if (clip == null)
        {
            Debug.LogWarning($"BGM '{name}' not found.");
            yield break;
        }

        audioSource.clip = clip;
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, timer / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOutAndStop(float duration)
    {
        float startVolume = audioSource.volume;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private IEnumerator FadeOutAndPlayNew(string newClipName, float duration)
    {
        float startVolume = audioSource.volume;

        // フェードアウト
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        audioSource.Stop();

        // フェードイン
        yield return FadeInBGM(newClipName, duration);

    }
}
