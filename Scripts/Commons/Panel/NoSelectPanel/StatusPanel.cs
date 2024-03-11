using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// キャラ図鑑のキャラステータス表示用のパネル
public sealed class StatusPanel : MonoBehaviour
{
    // 表示するテキスト群
    [SerializeField] TextMeshProUGUI LvText;            // レベル
    [SerializeField] TextMeshProUGUI HpText;            // HP
    [SerializeField] TextMeshProUGUI TpText;            // TP
    [SerializeField] TextMeshProUGUI AtkText;           // ATK
    [SerializeField] TextMeshProUGUI DefText;           // DEF
    [SerializeField] TextMeshProUGUI IntText;           // INT
    [SerializeField] TextMeshProUGUI ResText;           // RES
    [SerializeField] TextMeshProUGUI AgiText;           // AGI
    [SerializeField] TextMeshProUGUI ExpText;           // EXP
    [SerializeField] TextMeshProUGUI GoldText;          // ゴールド
    [SerializeField] TextMeshProUGUI FireDefText;       // 火耐性
    [SerializeField] TextMeshProUGUI WaterDefText;      // 氷耐性
    [SerializeField] TextMeshProUGUI WindDefText;       // 風耐性
    [SerializeField] TextMeshProUGUI SoilDefText;       // 土耐性
    [SerializeField] TextMeshProUGUI ThunderDefText;    // 雷耐性


    // ウィンドウを開く
    public void Open()
    {
        gameObject.SetActive(true);
    }


    // ウィンドウを閉じる
    public void Close()
    {
        gameObject.SetActive(false);
    }


    // テキストに、プレイヤーキャラのステータスをセットする
    public void SetPlayerStatus(PlayerPartyDbDataSO status)
    {
        int level = status.Level;

        LvText.text  = status.Level.ToString();
        HpText.text = status.playerUpStatusTableSO.playerUpStatusTableData[level].MaxHP.ToString();
        TpText.text = status.playerUpStatusTableSO.playerUpStatusTableData[level].MaxTP.ToString();
        AtkText.text = status.playerUpStatusTableSO.playerUpStatusTableData[level].ATK.ToString();
        DefText.text = status.playerUpStatusTableSO.playerUpStatusTableData[level].DEF.ToString();
        IntText.text = status.playerUpStatusTableSO.playerUpStatusTableData[level].INT.ToString();
        ResText.text = status.playerUpStatusTableSO.playerUpStatusTableData[level].RES.ToString();
        AgiText.text = status.playerUpStatusTableSO.playerUpStatusTableData[level].AGI.ToString();
        ExpText.text = "―";
        GoldText.text = "―";
        FireDefText.text = status.FireDef.ToString();
        WaterDefText.text = status.WaterDef.ToString();
        WindDefText.text = status.WindDef.ToString();
        SoilDefText.text = status.SoilDef.ToString();
        ThunderDefText.text = status.ThunderDef.ToString();
    }


    // テキストに、プレイヤーキャラのステータスをセットする
    public void SetEnemyStatus(EnemyStatusData status)
    {
        LvText.text = status.Level.ToString();
        HpText.text = status.MaxHP.ToString();
        TpText.text = "―";
        AtkText.text = status.ATK.ToString();
        DefText.text = status.DEF.ToString();
        IntText.text = status.INT.ToString();
        ResText.text = status.RES.ToString();
        AgiText.text = status.AGI.ToString();
        ExpText.text = status.Exp.ToString();
        GoldText.text = status.DropGold.ToString();
        FireDefText.text = status.FireDef.ToString();
        WaterDefText.text = status.WaterDef.ToString();
        WindDefText.text = status.WindDef.ToString();
        SoilDefText.text = status.SoilDef.ToString();
        ThunderDefText.text = status.ThunderDef.ToString();
    }

}
