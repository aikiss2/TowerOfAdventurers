using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using DG.Tweening;
using System;
using System.Reflection;
using UnityEngine.UI;

// 会話ウィンドウクラス
public class TalkWindow : MonoBehaviour
{
    // 名前テキスト
    [SerializeField] TextMeshProUGUI nameText;

    // 会話テキスト
    [SerializeField] TextMeshProUGUI talkText;

    // 文字送り(▼)テキスト
    [SerializeField] TextMeshProUGUI enterText;

    // 左ウィンドウ
    [SerializeField] Image leftWindow;

    // 左ウィンドウに表示するイメージ
    [SerializeField] SpriteRenderer leftTexture;

    // 右ウィンドウ
    [SerializeField] Image rightWindow;

    // 右ウィンドウに表示するイメージ
    [SerializeField] SpriteRenderer rightTexture;

    // 1文字表示にかける時間
    [SerializeField] float oneLetterTime;

    // 文字送り可能にする文字数(最後の文字からの数)
    [SerializeField] int possibleEnterLetter;

    // 会話キャラデータのテーブル
    [SerializeField] TalkCharaTableSO talkCharaTableSO;


    // 会話を表示
    public IEnumerator TypeTalk(TalkTableIndex index, bool reOpenPlayerPanel = true)
    {
        // プレイヤーパネルを閉じ、会話ウィンドウを開く
        GlobalCanvasManager.globalCanvasManager.playerPanel.Close();
        Open();

        // インデックスから表示する内容を検索し、表示する
        yield return SearchAndTalk(index, false);

        // 会話ウィンドウを閉じる
        Close();

        // プレイヤーパネルを再度開く指定(デフォルト)があれば、プレイヤーパネルを開く
        if(reOpenPlayerPanel == true)
        {
            GlobalCanvasManager.globalCanvasManager.playerPanel.Open(false);
        }

        yield return null;
    }


    // 会話を表示し、プレイヤーの回答を受ける。回答内容をyesAnswerに格納して呼び元に返す。
    public IEnumerator TypeTalkYesNo(TalkTableIndex index, bool[] yesAnswer, bool yesSelect = true, bool reOpenPlayerPanel = true)
    {
        // プレイヤーパネルを閉じ、会話ウィンドウを開く
        GlobalCanvasManager.globalCanvasManager.playerPanel.Close();
        Open();

        // インデックスから表示する内容を検索し、表示する
        yield return SearchAndTalk(index, true);

        // 初期の回答の選択位置が「はい」である場合、「はい」を選択状態で開く
        if (yesSelect == true)
        {
            GlobalCanvasManager.globalCanvasManager.yesNoButtonPanel.Open();
        }
        // 「いいえ」である場合、「いいえ」を選択状態で開く
        else
        {   
            GlobalCanvasManager.globalCanvasManager.yesNoButtonPanel.Open(1);
        }

        // プレイヤーからの入力待ち
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1));

        int currentID = GlobalCanvasManager.globalCanvasManager.yesNoButtonPanel.CurrentID;

        // 「はい」を選択状態　かつ　決定を入力の場合
        if( (currentID == 0) && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0)))
        {
            // 「はい」を選択
            yesAnswer[0] = true;
        }
        else
        {
            // 「いいえ」を選択
            yesAnswer[0] = false;
        }

        // 回答パネルおよび会話ウィンドウを閉じる
        GlobalCanvasManager.globalCanvasManager.yesNoButtonPanel.Close();
        Close();

        // プレイヤーパネルを再度開く指定(デフォルト)があれば、プレイヤーパネルを開く
        if (reOpenPlayerPanel == true)
        {
            GlobalCanvasManager.globalCanvasManager.playerPanel.Open(false);
        }

        yield return null;
    }


    // シーン遷移時にリセットフラグが立っているトークをリセットする
    public void TalkTableSceneReset()
    {
        int index = 0;
        foreach (TalkTableData one in GlobalCanvasManager.globalCanvasManager.talkTableSO.talkTableData)
        {
            if (one.sceneReset == true)
            {
                GlobalCanvasManager.globalCanvasManager.talkMemoryDbSO.done[index] = false;
            }

            index++;
        }
    }


    // インデックスから表示する内容を検索し、表示する
    private IEnumerator SearchAndTalk(TalkTableIndex index, bool noWait)
    {
        int indexNum = 0;

        // イベントトーク＞ステージトーク＞デフォルトの優先順で探す。
        if (EventTalkSearch(ref indexNum, index) == false)
        {
            if (IndexSearch(ref indexNum, index, GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.storyStage) == false)
            {
                IndexSearch(ref indexNum, index, StoryStage.None);
            }
        }

        // 会話を表示する
        yield return StartCoroutine(Talk(indexNum, noWait));
    }


    // 会話を表示する
    private IEnumerator Talk(int indexNum, bool noWait)
    {
        // 最初の会話かどうか
        bool firstTalk = true;

        // 一連の会話を続けて表示する
        for (; indexNum < GlobalCanvasManager.globalCanvasManager.talkTableSO.talkTableData.Count; indexNum++)
        {
            // 最初の会話ではなく、別の会話(None以外)であれば、会話を終了する
            if ( (firstTalk == false) && (GlobalCanvasManager.globalCanvasManager.talkTableSO.talkTableData[indexNum].index != TalkTableIndex.None))
            {
                break;
            }
            firstTalk = false;

            // キャラ情報と会話位置を取得
            TalkChara chara = GlobalCanvasManager.globalCanvasManager.talkTableSO.talkTableData[indexNum].chara;
            TalkSide side = GlobalCanvasManager.globalCanvasManager.talkTableSO.talkTableData[indexNum].side;
            nameText.text = talkCharaTableSO.talkCharaData[(int)chara].Name;

            // 会話位置により、会話するキャライメージを明るくし、会話していないキャラを暗くする
            if (side == TalkSide.None)
            {
                leftTexture.color = NoTalkCharaColor();
                rightTexture.color = NoTalkCharaColor();
            }
            else if(side == TalkSide.Left)
            {
                leftTexture.color = TalkCharaColor();
                rightTexture.color = NoTalkCharaColor();
                leftTexture.sprite = talkCharaTableSO.talkCharaData[(int)chara].Sprite;
                leftWindow.color = DisplayCharaWindow();
            }
            else if(side == TalkSide.Right)
            {
                leftTexture.color = NoTalkCharaColor();
                rightTexture.color = TalkCharaColor();
                rightTexture.sprite = talkCharaTableSO.talkCharaData[(int)chara].Sprite;
                rightWindow.color = DisplayCharaWindow(); 
            }
            // 左右両方
            else
            {
                leftTexture.color = TalkCharaColor();
                rightTexture.color = TalkCharaColor();
            }

            // 文字数により会話表示にかける時間を算出し、会話を開始する
            float talkTime = GlobalCanvasManager.globalCanvasManager.talkTableSO.talkTableData[indexNum].talk.Length * oneLetterTime;
            var tween1 = talkText.DOText(GlobalCanvasManager.globalCanvasManager.talkTableSO.talkTableData[indexNum].talk, talkTime);
            
            // SE再生
            SoundManager.soundManager.SEPlayStopAble(SEType.SeNo73);

            // 文字送りマークを点滅させる
            enterText.color = new Color32(255, 255, 255, 200);
            var tween2 = enterText.DOFade(0.2f, 0.8f).SetLoops(-1, LoopType.Yoyo);

            // 文字送り可能になるまでWAITする
            float waitTime = Mathf.Clamp(talkTime - (oneLetterTime * possibleEnterLetter), 0.07f, 1.0f);
            yield return new WaitForSeconds(waitTime);

            // SE停止
            SoundManager.soundManager.SEStop();

            // 文字送りを待つ場合は、プレイヤーの入力を待つ (待たない場合はプレイヤー回答を受け付ける会話を想定)
            if (noWait == false)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1));
            }

            // 念のためSE停止(文字表示のOnComplete時にSE停止は実行されている)
            SoundManager.soundManager.SEStop();

            // 文字表示と文字送りマークの制御を終了させる
            tween1.Kill();
            tween2.Kill();

            // 文字送りを待った場合は、テキストを消去する
            if (noWait == false)
            {
                talkText.text = "";
            }

            // 文字送りマークを見えなくする
            enterText.color = new Color32(255, 255, 255, 0);

            // 会話済みにする
            GlobalCanvasManager.globalCanvasManager.talkMemoryDbSO.done[indexNum] = true;
        }

        yield return new WaitForSeconds(0.1f);
    }


    // 会話しているキャラの色を取得する
    private Color32 TalkCharaColor()
    {
        return new Color32(255, 255, 255, 255);
    }


    // 会話していないキャラの色を取得する
    private Color32 NoTalkCharaColor()
    {
        return new Color32(100, 100, 100, 255);
    }


    // キャラウィンドウを表示するための色を取得する
    private Color32 DisplayCharaWindow()
    {
        return new Color32(255, 255, 255, 220);
    }


    // 会話ウィンドウを開く
    private void Open()
    {
        nameText.text = "";
        talkText.text = "";
        leftWindow.color = new Color32(255, 255, 255, 0);
        rightWindow.color = new Color32(255, 255, 255, 0);
        leftTexture.sprite = null;
        rightTexture.sprite = null;
        gameObject.SetActive(true);
    }


    // 会話ウィンドウを閉じる
    private void Close()
    {
        nameText.text = "";
        talkText.text = "";
        leftWindow.color = new Color32(255, 255, 255, 0);
        rightWindow.color = new Color32(255, 255, 255, 0);
        leftTexture.sprite = null;
        rightTexture.sprite = null;
        gameObject.SetActive(false);
    }


    // イベント固有の会話を検索する
    private bool EventTalkSearch(ref int indexNum, TalkTableIndex index)
    {
        bool ret = false; // 見つけられたらtrueを返す
        int num = 0;

        // そもそもイベントが設定されていなければ検索しない
        if (GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.eventTalk == EventTalk.None)
        {
            return ret;
        }

        foreach (TalkTableData one in GlobalCanvasManager.globalCanvasManager.talkTableSO.talkTableData)
        {
            if ((one.index == index) && (one.eventTalk == GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.eventTalk))
            {
                // 未実行の場合は、この台詞に決定
                if (GlobalCanvasManager.globalCanvasManager.talkMemoryDbSO.done[num] == false)
                {
                    ret = true;
                    indexNum = num;
                    break;
                }
            }

            num++;
        }

        return ret;
    }


    // ストーリー進捗を考慮しながら会話を検索する
    private bool IndexSearch(ref int indexNum, TalkTableIndex index, StoryStage searchStoryStage)
    {
        bool ret = false; // 見つけられたらtrueを返す
        int num = 0;
        bool nextSearch = false;
        foreach (TalkTableData one in GlobalCanvasManager.globalCanvasManager.talkTableSO.talkTableData)
        {
            if ( ((one.index == index) && (one.storyStage == searchStoryStage)) || (nextSearch == true))
            {
                // 未実行またはループ台詞の場合は、この台詞に決定
                if ( (GlobalCanvasManager.globalCanvasManager.talkMemoryDbSO.done[num] == false) || (one.loop == true))
                {
                    ret = true;
                    indexNum = num;
                    break;
                }
                else
                {
                    // 実行済みでありループ台詞でもなければ、次の台詞を探す
                    nextSearch = true;
                }
            }

            num++;
        }

        return ret;
    }
}
