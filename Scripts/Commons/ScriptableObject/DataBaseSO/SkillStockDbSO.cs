using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// スキルストック状態データベース
public class SkillStockDbSO : ScriptableObject
{
    public List<SkillStockDbData> skillStockDbData = new();


    // 未購入のスキルリストを取得する
    public List<SkillSO> GetSkillSoOfNoBuySkills()
    {
        List<SkillSO> list = new();
        foreach (var one in skillStockDbData)
        {
            if (one.stockCondition == StockCondition.NoBuy)
            {
                list.Add(one.skill);
            }
        }
        return list;
    }

    // 未購入のデータベースリストを取得する
    public List<SkillStockDbData> GetNoBuySkills()
    {
        List<SkillStockDbData> list = new();
        foreach (var one in skillStockDbData)
        {
            if (one.stockCondition == StockCondition.NoBuy)
            {
                list.Add(one);
            }
        }
        return list;
    }


    // ストック(購入済みで未習得)しているスキルリストを取得する
    public List<SkillSO> GetSkillSoOfStockSkills()
    {
        List<SkillSO> list = new();
        foreach (var one in skillStockDbData)
        {
            if (one.stockCondition == StockCondition.Stocked)
            {
                list.Add(one.skill);
            }
        }
        return list;
    }

    // ストック(購入済みで未習得)しているデータベースリストを取得する
    public List<SkillStockDbData> GetStockSkills()
    {
        List<SkillStockDbData> list = new();
        foreach (var one in skillStockDbData)
        {
            if (one.stockCondition == StockCondition.Stocked)
            {
                list.Add(one);
            }
        }
        return list;
    }
}

[System.Serializable]
// スキルストック状態データ
public class SkillStockDbData
{
    // スキル
    public SkillSO skill;

    // ストック状態
    public StockCondition stockCondition;
}