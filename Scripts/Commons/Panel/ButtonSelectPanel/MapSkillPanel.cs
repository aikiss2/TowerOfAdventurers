using DG.Tweening;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SelectButtonPanel))]
[RequireComponent(typeof(CreateButtonPanel))]
// ダンジョンで使用するスキルパネルクラス
public class MapSkillPanel : MonoBehaviour
{
    // 選択中のボタンID(外部公開用)
    public int CurrentID
    {
        get { return selectButtonPanel.CurrentID; }
    }

    // ダンジョンで使用可能なスキルリスト
    private readonly List<SkillSO> _skills = new();

    // 上記「_skills」を所持しているプレイヤーリスト(上記とリスト順同期)
    private readonly List<BattleCharacter> _players = new();

    // 選択可能のボタンパネルを制御するためのコンポーネント
    [SerializeField] private SelectButtonPanel selectButtonPanel;

    // 作成可能なボタンパネルを制御するためのコンポーネント
    [SerializeField] private CreateButtonPanel createButtonPanel;


    //ウィンドウを開く(戻り値について、開ける場合はtrue。表示できるスキルがなく、開けない場合はfalse)
    public bool Open(List<BattlePlayer> players)
    {
        selectButtonPanel.CurrentID = 0;

        List<string> mapSkillName = new();
        List<string> mapSkillTp = new();

        // マップで使用可能なスキルのみ抽出し、リストに加える
        foreach (BattleCharacter player in players)
        {
            foreach (SkillSO skill in player.skills)
            {
                if (skill.IsMapUse == true)
                {
                    mapSkillName.Add(skill.Name);
                    mapSkillTp.Add(skill.SpendTP.ToString());
                    _players.Add(player);
                    _skills.Add(skill);
                }
            }
        }

        // 表示できるスキルがなければfalseを返して終了する。
        if (mapSkillName.Count == 0)
        {
            return false;
        }

        mapSkillName.Add("キャンセル");
        mapSkillTp.Add("0");

        CreateSelectableTextWithTp(mapSkillName.ToArray(), mapSkillTp.ToArray());

        selectButtonPanel.MoveArrowMethod = MoveArrowTo;
        selectButtonPanel.Open();

        UselessButtonGray();

        return true;
    }


    //ウィンドウを閉じる
    public void Close()
    {
        Clear();
        gameObject.SetActive(false);
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
        _players.Clear();
        _skills.Clear();
    }


    // キャンセル項目を選択しているかを取得する(true:選択している)
    public bool IsCancel()
    {
        if (CurrentID >= _players.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // 選択しているスキル情報を取得
    public SkillSO GetNowSkill()
    {
        return _skills[CurrentID];
    }


    // 選択している項目の味方情報を取得
    public BattleCharacter GetNowBattleStatus()
    {
        return _players[CurrentID];
    }


    // 選択しているスキルが使用できるか取得。使用できなければreasonに理由を格納して返す。
    public bool GetUseable(ref string reason)
    {
        bool ret = true;
        reason = "";

        if (_players[CurrentID].IsDead == true)
        {
            reason = "戦闘不能のため、使用できません。";
            ret = false;
        }
        else if (_players[CurrentID].TP < _skills[CurrentID].SpendTP)
        {
            reason = "TPが足りません。";
            ret = false;
        }

        return ret;
    }


    // スクロールバーのRECT領域を取得する
    public RectTransform GetScrollbarRect()
    {
        return createButtonPanel.GetScrollbarRect();
    }


    // 選択されたボタンに呼び出され、そのボタンの子に矢印をセットする。
    // 表示範囲を外れそうな場所を選択された場合、スクロールする。
    public void MoveArrowTo(Transform parent)
    {
        selectButtonPanel.arrow.SetParent(parent);
        selectButtonPanel.CurrentID = parent.GetSiblingIndex();

        createButtonPanel.PanelScroll();

        // 「キャンセル」ボタン以外のときはダイアログ表示する
        if (CurrentID < _skills.Count)
        {
            GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog(_skills[CurrentID].Explain);
        }

        if (selectButtonPanel.IsSePlay == true)
        {
            SoundManager.soundManager.SEPlay(SEType.SeNo2);
        }

        Debug.Log($"カーソル移動:{CurrentID}");
    }


    // スキル名と消費TPを記載したボタンを作成する
    private void CreateSelectableTextWithTp(string[] skills, string[] tp)
    {
        List<string> skillNameList = new();

        int cnt = 0;
        foreach (var one in skills)
        {
            string space = "";
            for (int i = one.Length; i < Define.SKILL_NAME_LENGTH_MAX; i++)
            {
                space += "　";
            }

            for (int i = tp[cnt].Length; i < Define.SKILL_TP_LENGTH_MAX; i++)
            {
                space += "  ";
            }

            string createStr;

            // TPが0以外(=スキル)の場合は、TPも表示する
            if (int.Parse(tp[cnt]) != 0)
            {
                createStr = one + space + tp[cnt];
            }
            // TPが0の場合は「キャンセル」ボタンであり、TPを表示しない
            else
            {
                createStr = one;
            }

            skillNameList.Add(createStr);

            cnt++;
        }

        createButtonPanel.CreateSelectableText(skillNameList.ToArray());
    }


    // TPが足りず、スキルが使えないボタンを灰色にする。
    private void UselessButtonGray()
    {
        // TPが足りないor戦闘不能キャラのスキルは灰色にする
        int count = 0;
        foreach (var one in selectButtonPanel.SelectableButtons)
        {
            if (count < _players.Count)
            {
                if ((_players[count].TP < _skills[count].SpendTP) || (_players[count].IsDead == true))
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
            }

            count++;
        }
    }

}
