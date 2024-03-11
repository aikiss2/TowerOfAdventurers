using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 戦闘キャラのベースクラス
public abstract class BattleCharacter : MonoBehaviour
{
    // 攻撃順にランダム性を持たせるための一時的なAGI
    [HideInInspector] public int OrderAGI; 

    // 選択したスキル
    [HideInInspector] public SkillSO selectSkill;

    // 所持スキルのリスト
    [HideInInspector] public List<SkillSO> skills = new();

    // 選択したスキル対象
    [HideInInspector] public BattleCharacter chooseTarget;

    // キャラのパラメータ(外部参照用)
    public string Name { get { return _name; } }
    public int HP { get { return _HP; } }
    public int MaxHP { get { return _MaxHP; } }
    public int TP { get { return _TP; } }
    public int MaxTP { get { return _MaxTP; } }
    public int ATK { get { return (int)((float)_ATK * _AtkBuff); } }
    public int DEF { get { return (int)((float)_DEF * _DefBuff); } }
    public int INT { get { return (int)((float)_INT * _IntBuff); } }
    public int RES { get { return (int)((float)_RES * _ResBuff); } }
    public int DEX { get { return (int)((float)_DEX * _DexBuff); } }
    public int AGI { get { return (int)((float)_AGI * _AgiBuff); } }
    public float AtkBuff { get { return _AtkBuff; } }
    public float DefBuff { get { return _DefBuff; } }
    public float IntBuff { get { return _IntBuff; } }
    public float ResBuff { get { return _ResBuff; } }
    public float DexBuff { get { return _DexBuff; } }
    public float AgiBuff { get { return _AgiBuff; } }
    public int FireDef { get { return _FireDef; } }
    public int WaterDef { get { return _WaterDef; } }
    public int WindDef { get { return _WindDef; } }
    public int SoilDef { get { return _SoilDef; } }
    public int ThunderDef { get { return _ThunderDef; } }
    public int CutDef { get { return _CutDef; } }
    public int ThrustDef { get { return _ThrustDef; } }
    public int HitDef { get { return _HitDef; } }

    // キャラのパラメータ(内部用)
    protected string _name;
    protected int _HP;
    protected int _MaxHP;
    protected int _TP;
    protected int _MaxTP;
    protected int _ATK;
    protected int _DEF;
    protected int _INT;
    protected int _RES;
    protected int _DEX;
    protected int _AGI;
    protected float _AtkBuff;
    protected float _DefBuff;
    protected float _IntBuff;
    protected float _ResBuff;
    protected float _DexBuff;
    protected float _AgiBuff;
    protected int _FireDef;
    protected int _WaterDef;
    protected int _WindDef;
    protected int _SoilDef;
    protected int _ThunderDef;
    protected int _CutDef;
    protected int _ThrustDef;
    protected int _HitDef;


    // 戦闘キャラのタイプ(外部参照用)
    public BattlerType BattlerType { get { return _BattlerType; } }

    //戦闘タイプのタイプ
    protected BattlerType _BattlerType;


    // ダメージや回復等の演出を実行中フラグ(外部参照用)
    public bool IsExecuting { get { return _IsExecuting; } }

    // ダメージや回復等の演出を実行中フラグ
    protected bool _IsExecuting = false; //ダメージや回復等の演出を実行中フラグ


    // 戦闘不能フラグ(外部参照用)
    public bool IsDead { get { return _IsDead; } }

    // 戦闘不能フラグ
    protected bool _IsDead = false;


    // スキル対象(外部参照用)
    public BattleCharacter Target { get { return _Target; } }

    // スキル対象
    protected BattleCharacter _Target;


    // 逃げる実行フラグ
    public bool IsEscape { get; set; } = false;


    // 攻撃ミスのプレハブ
    [SerializeField] protected TextMeshProUGUI missTextPrefab;

    // HPダメージのプレハブ
    [SerializeField] protected NumberImageDamage hpDamagePrefab;

    // HP回復のプレハブ
    [SerializeField] protected NumberImageHeal hpHealPrefab;

    // イメージを描画するオブジェクト
    [SerializeField] protected SpriteRenderer spriteDrawObject;

    // HPを描画するオブジェクト
    [SerializeField] protected PointDraw hpDrawObject;

    // HPのテキスト(ダメージ等のポップアップの起点に必要)
    [SerializeField] protected TextMeshProUGUI hpText;

    // スキル実行前メソッド
    public abstract IEnumerator SkillStart();

    // スキル実行メソッド
    public abstract IEnumerator SkillExecute(List<BattlePlayer> players, List<BattleEnemy> enemies, RawImage effectScreen);

    // スキル実行後メソッド
    public abstract IEnumerator SkillEnd();

    // ダメージメソッド
    public abstract void Damage(int damagePoint);

    // 回復メソッド
    public abstract void Heal(int healPoint, bool effect);

    // TP消費メソッド
    public abstract void SpendTp(int spendPoint);

    // HP/TP初期化メソッド
    public abstract void InitializeHpTp();

    // TP回復メソッド
    public abstract void HealTp(int healPoint);


    // ダメージ処理を実行する
    public IEnumerator DamageCoroutine(int damagePoint)
    {
        // ダメージポップアップを生成
        var changeValueObj = Instantiate(hpDamagePrefab, hpText.transform);
        changeValueObj.SetNumber(damagePoint, 1);

        // HPメーターを更新する
        yield return hpDrawObject.SetPoint(HP, MaxHP);

        // ダメージ割合によって、ダメージ音や揺らす大きさを変える
        float changeRate = ((float)damagePoint / (float)MaxHP) * 100.0f;

        // 最大HPの5%未満ダメージ
        if (changeRate < 5.0f)
        {
            SoundManager.soundManager.SEPlay(SEType.SeNo21);
            transform.DOShakePosition(0.3f, new Vector3(5f, 5f, 0), 30);
        }
        // 最大HPの80%未満未満ダメージ
        else if (changeRate < 80.0f)
        {
            SoundManager.soundManager.SEPlay(SEType.SeNo21);
            transform.DOShakePosition(0.3f, new Vector3(10f, 10f, 0), 30);
        }
        // 最大HPの80%以上ダメージ
        else
        {
            SoundManager.soundManager.SEPlay(SEType.SeNo28);
            transform.DOShakePosition(0.3f, new Vector3(30f, 30f, 0), 30);
        }

        yield return new WaitForSeconds(0.7f);
    }

    // 回避処理を開始する
    public void Avoided()
    {
        StartCoroutine(AvoidedCoroutine());
    }


    // 回避処理を実行する
    public IEnumerator AvoidedCoroutine()
    {
        _IsExecuting = true;

        Instantiate(missTextPrefab, hpText.transform);

        yield return new WaitForSeconds(1.0f);

        _IsExecuting = false;
    }


    // 回復処理を実行する(effect:trueでは、メーターを徐々に回復させる)
    public IEnumerator HealCoroutine(int healPoint, bool effect)
    {
        _IsExecuting = true;

        // 回復ポップアップを生成
        var changeValueObj = Instantiate(hpHealPrefab, hpText.transform);
        changeValueObj.SetNumber(healPoint, 1);

        _HP = Math.Clamp(HP + healPoint, 1, MaxHP);

        yield return hpDrawObject.SetPoint(HP, MaxHP, effect);

        _IsExecuting = false;
    }


    // スキル対象を確定させる
    public void SetTarget()
    {
        // 自身に実行するスキルの場合は、対象を自オブジェクトにする
        if (selectSkill.TargetType == TargetType.Self)
        {
            _Target = this;
        }
        // 対象を選択するスキルは、選択している対象を設定する
        else if ((selectSkill.TargetType == TargetType.OpponentChoose) || (selectSkill.TargetType == TargetType.FriendChoose) || (selectSkill.TargetType == TargetType.DeadFriendChoose))
        {
            _Target = chooseTarget;
        }
    }


    // 自オブジェクトの初期化
    public virtual void Initialize()
    {
        BuffReset();

        _CutDef = 0;
        _ThrustDef = 0;
        _HitDef = 0;

        hpDrawObject.InitializePoint(HP, MaxHP);

        _IsExecuting = false;
        IsEscape = false;
    }


    // バフをリセット
    public void BuffReset()
    {
        _AtkBuff = 1.0f;
        _DefBuff = 1.0f;
        _IntBuff = 1.0f;
        _ResBuff = 1.0f;
        _DexBuff = 1.0f;
        _AgiBuff = 1.0f;
    }


    // 自身を蘇生する
    public virtual void Rebirth()
    {
        _IsExecuting = false;
        _IsDead = false;
    }


    // バフを変更する
    public void ModifyBuff(StatusType statusType, float modifyPoint)
    {
        float beforeBuff = 0.0f;
        float afterBuff = 0.0f;

        // 指定されたステータスのバフを変更する
        switch (statusType)
        {
            case StatusType.ATK:
                beforeBuff = AtkBuff;
                _AtkBuff = Mathf.Clamp(AtkBuff + modifyPoint, Define.BATTLE_BUFF_MIN, Define.BATTLE_BUFF_MAX);
                afterBuff = AtkBuff;
                break;

            case StatusType.DEF:
                beforeBuff = DefBuff;
                _DefBuff = Mathf.Clamp(DefBuff + modifyPoint, Define.BATTLE_BUFF_MIN, Define.BATTLE_BUFF_MAX);
                afterBuff = DefBuff;
                break;

            case StatusType.INT:
                beforeBuff = IntBuff;
                _IntBuff = Mathf.Clamp(IntBuff + modifyPoint, Define.BATTLE_BUFF_MIN, Define.BATTLE_BUFF_MAX);
                afterBuff = IntBuff;
                break;
            case StatusType.RES:
                beforeBuff = AtkBuff;
                _ResBuff = Mathf.Clamp(ResBuff + modifyPoint, Define.BATTLE_BUFF_MIN, Define.BATTLE_BUFF_MAX);
                afterBuff = AtkBuff;
                break;

            case StatusType.DEX:
                beforeBuff = DefBuff;
                _DexBuff = Mathf.Clamp(DexBuff + modifyPoint, Define.BATTLE_BUFF_MIN, Define.BATTLE_BUFF_MAX);
                afterBuff = DefBuff;
                break;

            case StatusType.AGI:
                beforeBuff = IntBuff;
                _AgiBuff = Mathf.Clamp(AgiBuff + modifyPoint, Define.BATTLE_BUFF_MIN, Define.BATTLE_BUFF_MAX);
                afterBuff = IntBuff;
                break;
        }

        // 変化状況をダイアログに表示する
        if (beforeBuff == afterBuff)
        {
            GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("これ以上は変化しない");
        }
        else if (beforeBuff < afterBuff)
        {
            GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog(statusType.ToString() + "が上がった");
        }
        else
        {
            GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog(statusType.ToString() + "が下がった");
        }
    }
}
