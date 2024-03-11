using DG.Tweening;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using static UnityEngine.EventSystems.EventTrigger;

[RequireComponent(typeof(SelectButtonPanel))]
[RequireComponent(typeof(CreateButtonPanel))]
// スキル選択パネルクラス
public class SkillPanel : MonoBehaviour
{
    // 選択中のボタンID(外部公開用)
    public int CurrentID
    {
        get { return selectButtonPanel.CurrentID; }
    }

    // 味方情報(TPを参照し、TPが足りない場合は選択不可の見た目にする)
    private BattlePlayer _battlePlayer;

    // スキル情報のリスト
    private List<SkillSO> _skills;

    // 選択可能のボタンパネルを制御するためのコンポーネント
    [SerializeField] private SelectButtonPanel selectButtonPanel;

    // 作成可能なボタンパネルを制御するためのコンポーネント
    [SerializeField] private CreateButtonPanel createButtonPanel;


    //ウィンドウを開く
    public void Open(BattlePlayer battlePlayer = null)
    {
        selectButtonPanel.CurrentID = 0;

        // 戦闘中(battleStatusがnullでない)、オプションでコマンド記憶の場合、初期位置を設定する
        int cnt = 0;
        if ((battlePlayer != null) && (GlobalCanvasManager.globalCanvasManager.optionSettingDbSO.commandMemory == true))
        {
            if (battlePlayer.memorySkill != null)
            {
                foreach (var one in battlePlayer.skills)
                {
                    if (one == battlePlayer.memorySkill)
                    {
                        selectButtonPanel.CurrentID = cnt;
                        break;
                    }

                    cnt++;
                }
            }
        }

        // 味方情報を保存
        _battlePlayer = battlePlayer;

        selectButtonPanel.MoveArrowMethod = MoveArrowTo;
        selectButtonPanel.Open(selectButtonPanel.CurrentID);

        // 使用できないボタンを灰色にする
        UselessButtonGray();
    }


    //ウィンドウを閉じる
    public void Close()
    {
        gameObject.SetActive(false);
        Clear();
    }


    //ウィンドウを選択状態にする
    public void Select()
    {
        UselessButtonGray();
        createButtonPanel.Select();
    }


    // ウィンドウを選択解除状態にする(ウィンドウは閉じない)
    public void DeSelect()
    {
        createButtonPanel.DeSelect();
    }


    //ボタンや内部情報を一掃する
    public void Clear()
    {
        createButtonPanel.Clear();

        if (_skills != null)
        {
            _skills.Clear();
            _skills = null;
        }
    }


    // スキル名を記載したボタンを作成する
    public void CreateSelectableText(IReadOnlyCollection<SkillSO> skills)
    {
        List<string> skillNameList = new();

        foreach (var one in skills)
        {
            skillNameList.Add(one.Name);
        }

        _skills = new List<SkillSO>(skills);

        createButtonPanel.CreateSelectableText(skillNameList.ToArray());
    }


    // スキル名と消費TPを記載したボタンを作成する
    public void CreateSelectableTextWithTp(IReadOnlyCollection<SkillSO> skills)
    {
        List<string> skillNameList = new();

        foreach (var one in skills)
        {
            string space = "";
            for (int i = one.Name.Length; i < Define.SKILL_NAME_LENGTH_MAX; i++)
            {
                space += "　";
            }

            for (int i = one.SpendTP.ToString().Length; i < Define.SKILL_TP_LENGTH_MAX; i++)
            {
                space += "  ";
            }

            string createStr = one.Name + space + one.SpendTP.ToString();

            skillNameList.Add(createStr);
        }

        _skills = new List<SkillSO>(skills);

        createButtonPanel.CreateSelectableText(skillNameList.ToArray());
    }


    // スキル名と消費ゴールドを記載したボタンを作成する
    public void CreateSelectableTextWithGold(IReadOnlyCollection<SkillSO> skills)
    {
        List<string> skillNameList = new();

        foreach (var one in skills)
        {
            string space = "";
            for (int i = one.Name.Length; i < Define.SKILL_NAME_LENGTH_MAX; i++)
            {
                space += "　";
            }

            // 枠を大きく使う
            space += "　";

            for (int i = one.SpendGold.ToString().Length; i < Define.SKILL_GOLD_LENGTH_MAX; i++)
            {
                space += "  ";
            }

            string createStr = one.Name + space + one.SpendGold.ToString() + "G";

            skillNameList.Add(createStr);
        }

        _skills = new List<SkillSO>(skills);

        createButtonPanel.CreateSelectableText(skillNameList.ToArray());
    }


    // 選択されたボタンに呼び出され、そのボタンの子に矢印をセットする。
    // 表示範囲を外れそうな場所を選択された場合、スクロールする。
    public void MoveArrowTo(Transform parent)
    {
        selectButtonPanel.arrow.SetParent(parent);
        selectButtonPanel.CurrentID = parent.GetSiblingIndex();

        createButtonPanel.PanelScroll();

        GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog(_skills[CurrentID].Explain);

        if (selectButtonPanel.IsSePlay == true)
        {
            SoundManager.soundManager.SEPlay(SEType.SeNo2);
        }

        Debug.Log($"カーソル移動:{CurrentID}");
    }


    // スキルが使用できるかを取得する(true:使用できる)
    public bool GetUseable()
    {
        // ステータス情報がない場合は使用可として返す
        if(_battlePlayer == null)
        {
            return true;
        }
        
        if (_battlePlayer.TP < _battlePlayer.skills[CurrentID].SpendTP)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    // スクロールバーのRECT領域を取得する
    public RectTransform GetScrollbarRect()
    {
        return createButtonPanel.GetScrollbarRect();
    }


    // TPが足りず、スキルが使えないボタンを灰色にする。
    private void UselessButtonGray()
    {
        // バトル中(_battleStatusがnullでない)は、TPが足りないものは灰色にする
        if (_battlePlayer != null)
        {
            int count = 0;
            foreach (var one in selectButtonPanel.SelectableButtons)
            {
                if (_battlePlayer.TP < _battlePlayer.skills[count].SpendTP)
                {
                    Color color = one.GetComponent<ButtonManager>().normalText.color;
                    one.GetComponent<ButtonManager>().normalText.color = new Color(color.r, color.g, color.b, 70f / 255f);

                    color = one.GetComponent<ButtonManager>().highlightedText.color;
                    one.GetComponent<ButtonManager>().highlightedText.color = new Color(color.r, color.g, color.b, 70f / 255f);
                }
                else
                {
                    Color color = one.GetComponent<ButtonManager>().normalText.color;
                    one.GetComponent<ButtonManager>().normalText.color = new Color(color.r, color.g, color.b, 255f / 255f);

                    color = one.GetComponent<ButtonManager>().highlightedText.color;
                    one.GetComponent<ButtonManager>().highlightedText.color = new Color(color.r, color.g, color.b, 255f / 255f);
                }

                count++;
            }
        }
    }
}
