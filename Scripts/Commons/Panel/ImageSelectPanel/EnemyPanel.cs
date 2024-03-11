using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[RequireComponent(typeof(SelectImagePanel))]
// 敵パネルクラス
public class EnemyPanel : MonoBehaviour
{
    // 選択中のイメージID(外部公開用)
    public int CurrentID
    {
        get { return selectImagePanel.CurrentID; }
    }

    // 各敵を表示するフレームのプレハブ
    [SerializeField] SelectableImage enemyFramePrefab;

    // 選択矢印のプレハブ
    [SerializeField] TextMeshProUGUI arrowTextPrefab;

    // 選択可能のイメージパネルを制御するためのコンポーネント
    [SerializeField] SelectImagePanel selectImagePanel;


    // 敵データベースを参照し、敵を生成する
    private void Awake()
    {
        for(int i = 0; i < GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers.Count; i++)
        {
            int enemyNumber = GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers[i];
            CreateSelectableImage(enemyNumber);

            // 出会っていない敵であれば、出会ったことを記憶する
            if (GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.encountEnemyList[enemyNumber] == false)
            {
                GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.encountEnemyList[enemyNumber] = true;
            }
        }

        // 矢印を生成する
        CreateArrow();
    }


    //ウィンドウを開く
    public void Open()
    {
        selectImagePanel.CurrentID = 0;

        selectImagePanel.arrow.GetComponent<TextMeshProUGUI>().text = "▼";

        // もしマウスポインターが既に対象を選択している場合は、初期値をその対象に設定する
        int id = 0;
        foreach (var one in selectImagePanel.SelectableImages)
        {
            if (one.IsPointerInside == true)
            {
                selectImagePanel.CurrentID = id;
                break;
            }

            id++;
        }

        selectImagePanel.Open(selectImagePanel.CurrentID);
    }


    //ウィンドウを選択状態にする
    public void Select()
    {
        selectImagePanel.arrow.GetComponent<TextMeshProUGUI>().text = "▼";
        selectImagePanel.Select();
    }


    // ウィンドウを非選択状態にする(ウィンドウは閉じない)
    public void DeSelect()
    {
        selectImagePanel.arrow.GetComponent<TextMeshProUGUI>().text = "";

        selectImagePanel.DeSelect();

        selectImagePanel.IsSePlay = false;
    }


    // 敵番号から敵を生成して登録する
    private void CreateSelectableImage(int enemyNumber)
    {
        SelectableImage enemy = Instantiate(enemyFramePrefab, transform);

        enemy.GetComponent<BattleEnemyFrame>().battleEnemy.SetEnemyNumber(enemyNumber);
        enemy.GetComponent<BattleEnemyFrame>().battleEnemy.Initialize();

        selectImagePanel.SelectableImages.Add(enemy);
    }


    // 倒された敵を削除する
    public void DelateSelectableImage()
    {
        for (int i = selectImagePanel.SelectableImages.Count - 1; i >= 0; i--)
        {
            var selectableImage = selectImagePanel.SelectableImages[i];

            // 倒された敵を削除する
            if (selectableImage.GetComponent<BattleEnemyFrame>().battleEnemy.IsDead == true)
            {
                // 敵がまだいる場合は矢印を退避させる
                if (selectImagePanel.SelectableImages.Count > 1)
                {
                    // 左端の場合は2番目に退避
                    if (i == 0)
                    {
                        selectImagePanel.arrow.SetParent(transform.GetChild(1).gameObject.transform, false);
                    }
                    // 左端以外は左端に退避
                    else
                    {
                        selectImagePanel.arrow.SetParent(transform.GetChild(0).gameObject.transform, false);
                    }
                }

                selectImagePanel.SelectableImages.Remove(selectableImage);
                Destroy(selectableImage.gameObject);
            }
        }
    }


    // 矢印を生成する
    private void CreateArrow()
    {
        // 一番目の子オブジェクトに付加する
        TextMeshProUGUI text = Instantiate(arrowTextPrefab, transform.GetChild(0).gameObject.transform);

        selectImagePanel.arrow = text.rectTransform;
    }


    // 現在選択されている敵を取得する
    public BattleEnemy GetSelectEnemy()
    {
        return selectImagePanel.SelectableImages[CurrentID].GetComponent<BattleEnemyFrame>().battleEnemy;
    }


    // 敵のリストを取得する
    public List<BattleEnemy> GetEnemyList()
    {
        List < BattleEnemy > enemies = new ();

        foreach (var one in selectImagePanel.SelectableImages)
        {
            enemies.Add(one.GetComponent<BattleEnemyFrame>().battleEnemy);
        }

        return enemies;
    }
}
