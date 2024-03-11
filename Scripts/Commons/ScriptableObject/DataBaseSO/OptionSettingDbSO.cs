using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// オプション設定データベース
public class OptionSettingDbSO : ScriptableObject
{
    // BGM音量
    public float bgmVolume;

    // SE音量
    public float seVolume;

    // バトル時のコマンド記憶
    public bool commandMemory;
}