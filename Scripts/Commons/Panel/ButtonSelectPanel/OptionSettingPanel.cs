using DG.Tweening;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SelectButtonPanel))]
// オプション設定パネルのクラス
public class OptionSettingPanel : MonoBehaviour
{
    // 選択中のボタンID
    public int CurrentID
    {
        get { return _selectButtonPanel.CurrentID; }
    }

    // BGM音量を操作するスライダー
    [SerializeField] Slider bgmSlider;

    // SE音量を操作するスライダー
    [SerializeField] Slider seSlider;

    // コマンド記憶ON/OFFを切り替えるスイッチ
    [SerializeField] SwitchManager commandMemorySwitch;

    // コマンド記憶の設定状態(ON/OFF)を示すテキスト
    [SerializeField] TextMeshProUGUI commandMemorySwitchText;

    // 選択可能のボタンパネルを制御するためのコンポーネント
    [SerializeField] private SelectButtonPanel _selectButtonPanel;


    //ウィンドウを開く
    public void Open()
    {
        _selectButtonPanel.Open();
    }


    //ウィンドウを閉じる
    public void Close()
    {
        _selectButtonPanel.Close();
    }


    // BGM音量を変更した際、サウンドマネージャーにBGM音量の変更を伝える
    public void BgmVolumeChange()
    {
        SoundManager.soundManager.SetBgmVolume(bgmSlider.value / 100.0f);
    }


    // SE音量を変更した際、サウンドマネージャーにSE音量の変更を伝え、SEを鳴らす
    public void SeVolumeChange()
    {
        SoundManager.soundManager.SetSeVolume(seSlider.value / 100.0f);
        SoundManager.soundManager.SEPlay(SEType.SeNo2);
    }


    // BGM音量を変更する
    public void ChangeBgmVolumeValue(float value)
    {
        bgmSlider.value = Mathf.Clamp(bgmSlider.value + value, 0.0f, 100.0f);
    }


    // SE音量を変更する
    public void ChangeSeVolumeValue(float value)
    {
        seSlider.value = Mathf.Clamp(seSlider.value + value, 0.0f, 100.0f);
    }


    // コマンド記憶設定を変更する(isONがtrueで、コマンド記憶ON)
    public void ChangeCommandMemorySwitch(bool isOn)
    {
        if (commandMemorySwitch.isOn != isOn)
        {
            if (isOn)
            {
                commandMemorySwitch.SetOn();
            }
            else
            {
                commandMemorySwitch.SetOff();
            }
        }
    }


    // コマンド記憶のテキストを「ON」にする
    public void CommandMemorySwitchOn()
    {
        commandMemorySwitchText.text = "ON";
    }


    // コマンド記憶のテキストを「OFF」にする
    public void CommandMemorySwitchOff()
    {
        commandMemorySwitchText.text = "OFF";
    }


    // オプションデータをオプション設定画面に反映する
    public void SetOptionSettingWindow(OptionSettingDbSO optionDb)
    {
        bgmSlider.value = optionDb.bgmVolume;
        seSlider.value = optionDb.seVolume;
        commandMemorySwitch.isOn = optionDb.commandMemory;
    }


    // オプション設定画面の内容を、オプションデータにセーブする
    public void SaveOptionSettingDb(OptionSettingDbSO optionDb)
    {
        optionDb.bgmVolume = bgmSlider.value;
        optionDb.seVolume = seSlider.value;
        optionDb.commandMemory = commandMemorySwitch.isOn;
    }
}
