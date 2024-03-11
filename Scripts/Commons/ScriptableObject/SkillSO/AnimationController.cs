using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    // OnPlaySeメソッドで順番に鳴らされるSE
    [SerializeField] SEType[] seType;

    // 画面エフェクトの色
    [SerializeField] Color screenEffectColor;

    // 画面エフェクトの長さ(時間)
    [SerializeField] float screenEffectTime;

    // ターゲットエフェクトの色
    [SerializeField] Color targetEffectColor;

    // ターゲットエフェクトの長さ(時間)
    [SerializeField] float targetEffectTime;

    // 生成したエフェクトを操作するTweenリスト
    private readonly List<Tween> _tweenList = new ();

    // SEをどの回数まで鳴らしたかのシーケンス番号
    private int _seCount = 0;

    // 画面エフェクトのオブジェクト
    private RawImage _effectScreen;

    // ターゲットエフェクトのオブジェクト
    private BattleCharacter _effectTarget;


    // 画面エフェクトとターゲットエフェクトのオブジェクトを設定する
    public void SetEffectObject(RawImage effectScreen, BattleCharacter effectTarget)
    {
        _effectScreen = effectScreen;
        _effectTarget = effectTarget;
    }


    // 画面エフェクトを生成する
    public void OnScreenEffect()
    {
        if(_effectScreen != null)
        {
            _effectScreen.color = screenEffectColor;
            Tween tween = _effectScreen.DOFade(0.0f, screenEffectTime);
            _tweenList.Add(tween);
        }
    }

    // ターゲットエフェクトを生成する
    public void OnTargetEffect()
    {
        if (_effectTarget != null)
        {
            var sprite = _effectTarget.GetComponent<SpriteRenderer>();
            sprite.color = targetEffectColor;

            Tween tween = DOTween.To(() => sprite.color, z => sprite.color = z, new Color32(255, 255, 255, 255), targetEffectTime);
            _tweenList.Add(tween);
        }
    }

    // SEを鳴らす
    public void OnPlaySe()
    {
        // 配列以内であればSE再生
        if(_seCount < seType.Length)
        {
            //SE再生
            SoundManager.soundManager.SEPlay(seType[_seCount]);
            _seCount++;
        }

    }


    // アニメーション完了時の処理
    public void OnCompleteAnimation()
    {
        // Tweenを終了させる
        foreach (var one in _tweenList)
        {
            one.Kill();
        }

        // 画面エフェクトを透明にする
        if (_effectScreen != null)
        {
            _effectScreen.color = new Color32(255, 255, 255, 0);
        }

        // ターゲットを通常色にする
        if (_effectTarget != null)
        {
            var sprite = _effectTarget.GetComponent<SpriteRenderer>();
            sprite.color = new Color32(255, 255, 255, 255);
        }

        // 自身を破棄する
        Destroy(this.gameObject);
    }
}
