using DG.Tweening;
using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 各シーンマネージャーのベースクラス
public class SceneManagerBase : MonoBehaviour
{
    // フェードイン・フェードアウトを表現するキャンバス
    [SerializeField] private GameObject fadeScreenCanvas;

    // メインキャンバス
    [SerializeField] protected Canvas mainCanvas;

    // メインカメラ
    [SerializeField] protected Camera mainCamera; // GlovalCanvasに設定するため

    // ボタン押下判定フラグ
    [HideInInspector] public bool buttonClick = false;

    // フェードアウト実行フラグ(フェードイン中にフェードアウトが発生した場合に対処するため)
    private bool _IsFadeout = false;


    // フェードアウトを実行
    public IEnumerator Fadeout()
    {
        _IsFadeout = true;
        var fade = fadeScreenCanvas.GetComponent<RawImage>();
        fade.color = new Color32(0, 0, 0, 0);

        fadeScreenCanvas.SetActive(true);

        yield return fade.DOFade(1.0f, 1.0f).WaitForCompletion();
    }


    // フェードインを実行
    public void Fadein()
    {
        _IsFadeout = false;
        var fade = fadeScreenCanvas.GetComponent<RawImage>();
        fade.color = new Color32(0, 0, 0, 255);

        fadeScreenCanvas.SetActive(true);

        fade.DOFade(0.0f, 0.5f).OnComplete(FadeinEnd);
    }


    // フェードイン完了時にフェードインキャンバスを無効化する
    private void FadeinEnd()
    {
        // フェードアウトが始まっていなければ非アクティブにする(通常パターン)
        if (_IsFadeout != true)
        {
            fadeScreenCanvas.SetActive(false);
        }
    }


    // ボタン押下時にフラグをtrueにする
    public void ButtonClick()
    {
        buttonClick = true;
    }


    // プレイヤーからの入力(決定・キャンセル)を待ち、入力内容を呼び元に返す
    public IEnumerator WaitPlayerInput(PlayerInput[] playerInput)
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            // 決定：スペース・リターン・Z・左クリック
            // キャンセル:エスケープ・X・左右Ctrl・右クリック
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1));
            // 決定
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
            {
                playerInput[0] = PlayerInput.Decide;
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1))
            {
                playerInput[0] = PlayerInput.Cancel;
                break;
            }
        }
    }


    // プレイヤーからの入力(決定・キャンセル)を待ち、入力内容を呼び元に返す
    // 左クリック時はボタンの位置に合わせた押下でないと、決定と判断しない
    public IEnumerator WaitPlayerInputChooseButton(PlayerInput[] playerInput)
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            // 決定：スペース・リターン・Z・ボタン上の左クリック
            // キャンセル:エスケープ・X・左右Ctrl・右クリック
            buttonClick = false;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || buttonClick == true || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1));
            // 決定
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || buttonClick == true)
            {
                playerInput[0] = PlayerInput.Decide;
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1))
            {
                playerInput[0] = PlayerInput.Cancel;
                break;
            }
        }
    }


    // プレイヤーからの入力(決定・キャンセル)を待ち、入力内容を呼び元に返す
    // 左クリックを無効にするRECT領域を指定する(スクロールバー等)
    public IEnumerator WaitPlayerInputNoClickRect(PlayerInput[] playerInput, RectTransform noClickRect)
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            // 決定：スペース・リターン・Z・左クリック
            // キャンセル:エスケープ・X・左右Ctrl・右クリック
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1));
            // 決定
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))
            {
                // 左クリック無効のRECT領域をクリックしたかを判定
                bool clickRect = false;
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = 0.0f;
                    Vector3 clickPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                    var corners = new Vector3[4];
                    noClickRect.GetWorldCorners(corners);

                    Rect rect = new(corners[0].x, corners[0].y, corners[3].x - corners[0].x, corners[1].y - corners[0].y);
                    if (rect.Contains(clickPosition))
                    {
                        clickRect = true;
                    }
                }

                // 無効領域をクリックしていなければ決定されたものとして処理を終了する
                if (clickRect == false)
                {
                    playerInput[0] = PlayerInput.Decide;
                    break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(1))
            {
                playerInput[0] = PlayerInput.Cancel;
                break;
            }
        }
    }
}
