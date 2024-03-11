using System.Collections.Generic;
using UnityEngine;


// BGMタイプ
public enum BGMType
{
    None,       //なし
    BgmNo1,
    BgmNo2,
    BgmNo3,
    BgmNo4,
    BgmNo5,
    BgmNo6,
    BgmNo7,
    BgmNo8,
    BgmNo9,
    BgmNo10,
    BgmNo11,
    BgmNo12,
    BgmNo13,
    BgmNo14,
    BgmNo15,
    BgmNo16,
    BgmNo17,
    BgmNo18,
    BgmNo19,
    BgmNo20,
    BgmNo21,
    BgmNo22,
    BgmNo23,
    BgmNo24,
    BgmNo25,
    BgmNo26,
    BgmNo27,
    BgmNo28,
    BgmNo29,
    BgmNo30,
}


// SEタイプ
public enum SEType
{
    None,       //なし
    SeNo1,
    SeNo2,
    SeNo3,
    SeNo4,
    SeNo5,
    SeNo6,
    SeNo7,
    SeNo8,
    SeNo9,
    SeNo10,
    SeNo11,
    SeNo12,
    SeNo13,
    SeNo14,
    SeNo15,
    SeNo16,
    SeNo17,
    SeNo18,
    SeNo19,
    SeNo20,
    SeNo21,
    SeNo22,
    SeNo23,
    SeNo24,
    SeNo25,
    SeNo26,
    SeNo27,
    SeNo28,
    SeNo29,
    SeNo30,
    SeNo31,
    SeNo32,
    SeNo33,
    SeNo34,
    SeNo35,
    SeNo36,
    SeNo37,
    SeNo38,
    SeNo39,
    SeNo40,
    SeNo41,
    SeNo42,
    SeNo43,
    SeNo44,
    SeNo45,
    SeNo46,
    SeNo47,
    SeNo48,
    SeNo49,
    SeNo50,
    SeNo51,
    SeNo52,
    SeNo53,
    SeNo54,
    SeNo55,
    SeNo56,
    SeNo57,
    SeNo58,
    SeNo59,
    SeNo60,
    SeNo61,
    SeNo62,
    SeNo63,
    SeNo64,
    SeNo65,
    SeNo66,
    SeNo67,
    SeNo68,
    SeNo69,
    SeNo70,
    SeNo71,
    SeNo72,
    SeNo73,
    SeNo74,
    SeNo75,
    SeNo76,
    SeNo77,
    SeNo78,
    SeNo79,
    SeNo80,
}


// プレイヤー入力に対応するSE
public enum InputReactionSE
{
    None,
    Decide,     // 決定
    Cancel,     // キャンセル
    Reject,     // 拒否
}


// サウンド管理クラス
public class SoundManager : MonoBehaviour
{
    // ゲーム中の全BGMデータ
    [SerializeField] private AudioClip[] bgm;

    // ゲーム中の全SEデータ
    [SerializeField] private AudioClip[] seClip;

    // SE用オーディオソース
    [SerializeField] private AudioSource seAudioSource;

    // BGM用オーディオソース
    [SerializeField] private AudioSource bgmAudioSource;

    // SE用オーディオソース(再生途中で停止可)
    [SerializeField] private AudioSource stopAbleSeAudioSource;

    // SEを途中で停止するためのカウンタ(音源の数)
    private int stopAbleSeCount = 0;

    // 再生中のBGM
    private BGMType plyingBGM = BGMType.None;

    // サウンドマネージャーを保存するstatic変数
    public static SoundManager soundManager;


    // static変数に自身を保存する(シングルトン)
    private void Awake()
    {
        // ゲーム起動時
        if (soundManager == null)
        {
            // static変数に自分を保存する
            soundManager = this;  

            // シーンが変わってもゲームオブジェクトを破棄しない
            DontDestroyOnLoad(gameObject);
        }
        // シーン切り替え時
        else
        {
            // シーン先のサウンドマネージャーを破棄する
            Destroy(gameObject);
        }
    }


    // BGMを再生する(loop:trueでBGMをループさせる)
    public void PlayBgm(BGMType type, bool loop = true)
    {
        if (type != plyingBGM)
        {
            plyingBGM = type;
            bgmAudioSource.clip = bgm[(int)type];
            bgmAudioSource.loop = loop;

            bgmAudioSource.Play();
        }
    }


    // BGMを停止する
    public void StopBgm()
    {
        bgmAudioSource.Stop();
        plyingBGM = BGMType.None;
    }


    // SEを再生する
    public void SEPlay(SEType type)
    {
        Debug.Log($"SEPlay:{type}");
        seAudioSource.PlayOneShot(seClip[(int)type]);

    }


    // プレイヤー入力に対するSEを再生する
    public void InputReactionSEPlay(InputReactionSE inputReactionSE)
    {
        Debug.Log($"InputReactionSEPlay:{inputReactionSE}");
        switch (inputReactionSE)
        {
            case InputReactionSE.Decide:
                seAudioSource.PlayOneShot(seClip[(int)SEType.SeNo3]);
                break;
            case InputReactionSE.Cancel:
                seAudioSource.PlayOneShot(seClip[(int)SEType.SeNo72]);
                break;
            case InputReactionSE.Reject:
                seAudioSource.PlayOneShot(seClip[(int)SEType.SeNo35]);
                break;
        }
    }


    // 停止可能なSEを再生する
    public void SEPlayStopAble(SEType type)
    {
        stopAbleSeAudioSource.PlayOneShot(seClip[(int)type]);
        stopAbleSeCount++;
    }


    // 停止可能なSEをストップする(音源なくなったら停止する)
    public void SEStop()
    {
        stopAbleSeCount--;

        // 音源がなくなったら停止
        if(stopAbleSeCount < 1)
        {
            stopAbleSeAudioSource.Stop();
            stopAbleSeCount = 0;
        }
    }


    // BGMの音量を設定する
    public void SetBgmVolume(float volume)
    {
        bgmAudioSource.volume = volume;
    }


    // SEの音量を設定する
    public void SetSeVolume(float volume)
    {
        seAudioSource.volume = volume;
        stopAbleSeAudioSource.volume = volume;
    }

}

