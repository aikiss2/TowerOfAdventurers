using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 戦闘用プレイヤークラス
public class BattlePlayer : BattleCharacter
{
    // １戦闘での取得経験値
    [HideInInspector] public int GetExp;

    // 該当レベルでの累積経験値
    [HideInInspector] public int TotalExp;

    // パーティにおける位置(外部参照用)
    public int Position { get { return _Position; } }

    // パーティにおける位置
    private int _Position;

    // 前回選択スキル
    [HideInInspector] public SkillSO memorySkill;

    // レベル
    [HideInInspector] public int Level;

    // ステータステーブル
    [HideInInspector] private PlayerUpStatusTableSO playerUpStatusTableSO;

    // 経験値テーブル
    [HideInInspector] public PlayerExpTableSO playerExpTableSO;

    // TP回復のプレハブ
    [SerializeField] protected NumberImageHeal tpHealPrefab;

    // TPを描画するオブジェクト
    [SerializeField] protected PointDraw TpDrawObject;

    // TPのテキスト(回復等のポップアップの起点に必要)
    [SerializeField] protected TextMeshProUGUI tpText;

    // 生存時のイメージ
    private Sprite _aliveTexture;

    // 戦闘不能時のイメージ
    private Sprite _deadTexture;


    // 初期化
    public override void Initialize()
    {
        // プレイヤータイプを設定
        _BattlerType = BattlerType.Player;

        // パーティ位置(Position)により、データベースからプレイヤー情報を取得し、設定する
        _name = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].playerName;
        _aliveTexture = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].AliveTexture;
        _deadTexture = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].DeadTexture;
        playerUpStatusTableSO = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].playerUpStatusTableSO;
        playerExpTableSO = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].playerExpTableSO;
        Level = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].Level;
        _HP = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].HP;
        _TP = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].TP;
        TotalExp = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].Exp;

        // 戦闘不能の場合
        if (HP == 0)
        {
            // 戦闘不能用のイメージを設定
            spriteDrawObject.sprite = _deadTexture;
            _IsDead = true;
        }
        // 生存の場合
        else
        {
            // 生存用のイメージを設定
            spriteDrawObject.sprite = _aliveTexture;
            _IsDead = false;
        }

        // 所持スキルを設定
        skills = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].skills;

        // ステータステーブルとレベルにより、ステータスを設定
        _MaxHP = playerUpStatusTableSO.playerUpStatusTableData[Level].MaxHP;
        _MaxTP = playerUpStatusTableSO.playerUpStatusTableData[Level].MaxTP;
        _ATK = playerUpStatusTableSO.playerUpStatusTableData[Level].ATK;
        _DEF = playerUpStatusTableSO.playerUpStatusTableData[Level].DEF;
        _INT = playerUpStatusTableSO.playerUpStatusTableData[Level].INT;
        _RES = playerUpStatusTableSO.playerUpStatusTableData[Level].RES;
        _DEX = playerUpStatusTableSO.playerUpStatusTableData[Level].DEX;
        _AGI = playerUpStatusTableSO.playerUpStatusTableData[Level].AGI;
        _FireDef = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].FireDef;//火
        _WaterDef = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].WaterDef;//水
        _WindDef = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].WindDef;//風
        _SoilDef = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].SoilDef;//土
        _ThunderDef = GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].ThunderDef;//雷

        // その他初期化をベースクラスに委譲
        base.Initialize();

        // TP表示を初期化
        TpDrawObject.InitializePoint(TP, MaxTP);
    }


    // スキル実行
    public override IEnumerator SkillExecute(List<BattlePlayer> players, List<BattleEnemy> enemies, RawImage effectScreen)
    {
        // スキルクラスに渡すためのリストを作成する
        List<BattleCharacter> playersList = new ();
        List<BattleCharacter> enemiesList = new ();

        playersList.AddRange(players);
        if(enemies != null)
        {
            enemiesList.AddRange(enemies);
        }

        // 選択しているスキルを実行する
        yield return selectSkill.Execute(this, Target, playersList, enemiesList, effectScreen);
    }


    // スキル実行前にパネルをせり出す
    public override IEnumerator SkillStart()
    {
        RectTransform rect = transform.parent.transform.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, -0.2f);

        yield break;
    }


    // スキル実行後に、パネルのせり出しを戻す
    public override IEnumerator SkillEnd()
    {
        RectTransform rect = transform.parent.transform.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 0.5f);

        yield break;
    }


    // 自身を蘇生する
    public override void Rebirth()
    {
        base.Rebirth();
        spriteDrawObject.sprite = _aliveTexture;
    }


    // ダメージ演出を開始する
    public override void Damage(int damagePoint)
    {
        StartCoroutine(DamageAndDying(damagePoint));
    }


    // ダメージおよび戦闘不能の処理を実行する
    private IEnumerator DamageAndDying(int damagePoint)
    {
        _IsExecuting = true;

        _HP = Math.Clamp(HP - damagePoint, Define.HP_MIN_LIMIT, Define.HP_MAX_LIMIT);

        yield return StartCoroutine(DamageCoroutine(damagePoint));

        // 戦闘不能の場合
        if (HP == 0)
        {
            StartDying();
            yield return new WaitForSeconds(0.5f);
            _IsDead = true;
        }

        _IsExecuting = false;
    }


    // 戦闘不能時の処理を実行する
    private void StartDying()
    {
        BuffReset();

        SoundManager.soundManager.SEPlay(SEType.SeNo25);
        spriteDrawObject.sprite = _deadTexture;
    }


    // 回復演出を開始する
    public override void Heal(int healPoint, bool effect)
    {
        StartCoroutine(HealCoroutine(healPoint, effect));
    }


    // TP消費演出を開始する
    public override void SpendTp(int spendPoint)
    {
        _TP = Math.Clamp(TP - spendPoint, Define.TP_MIN_LIMIT, Define.TP_MAX_LIMIT);

        StartCoroutine(TpDrawObject.SetPoint(TP, MaxTP));
    }


    // TP回復処理を開始する
    public override void HealTp(int healPoint)
    {
        var changeValueObj = Instantiate(tpHealPrefab, tpText.transform);
        changeValueObj.SetNumber(healPoint, 1);

        _TP = Math.Clamp(TP + healPoint, 1, MaxTP);

        StartCoroutine(TpDrawObject.SetPoint(TP, MaxTP));
    }


    // HP/TPを初期化する
    public override void InitializeHpTp()
    {
        _HP = MaxHP;
        _TP = MaxTP;

        hpDrawObject.InitializePoint(HP, MaxHP);
        TpDrawObject.InitializePoint(TP, MaxTP);
    }

    // プレイヤー情報をセーブする
    public void SavePlayerStatus()
    {
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].Level = Level;
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].HP = HP;
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].TP = TP;
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].Exp = TotalExp;
}


    // 所持スキルをセーブする
    public void SetSkill()
    {
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[Position].skills = skills;
    }


    // パーティ位置を設定する
    public void SetPosition(int position)
    {
        _Position = position;
    }


    // 着脱可能なスキルのリストを取得する
    public List<SkillSO> GetMoveAbleSkills()
    {
        List<SkillSO> list = new();
        foreach (SkillSO skill in skills)
        {
            if (skill.IsNoMove != true)
            {
                list.Add(skill);
            }

        }
        return list;
    }


    // プレイヤーのみ
    public void DelateSkill(SkillSO skill)
    {
        skills.Remove(skill);
    }


    // レベルアップさせ、ステータスを更新する
    public void LevelUp()
    {
        Level++;

        _HP = playerUpStatusTableSO.playerUpStatusTableData[Level].MaxHP;
        _TP = playerUpStatusTableSO.playerUpStatusTableData[Level].MaxTP;
        _MaxHP = playerUpStatusTableSO.playerUpStatusTableData[Level].MaxHP;
        _MaxTP = playerUpStatusTableSO.playerUpStatusTableData[Level].MaxTP;
        _ATK = playerUpStatusTableSO.playerUpStatusTableData[Level].ATK;
        _DEF = playerUpStatusTableSO.playerUpStatusTableData[Level].DEF;
        _INT = playerUpStatusTableSO.playerUpStatusTableData[Level].INT;
        _RES = playerUpStatusTableSO.playerUpStatusTableData[Level].RES;
        _DEX = playerUpStatusTableSO.playerUpStatusTableData[Level].DEX;
        _AGI = playerUpStatusTableSO.playerUpStatusTableData[Level].AGI;

        InitializeHpTp();
    }
}
