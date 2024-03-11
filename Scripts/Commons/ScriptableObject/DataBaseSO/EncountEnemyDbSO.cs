using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// エンカウントした敵データベース
public class EncountEnemyDbSO : ScriptableObject
{
    // バトルBGM
    public BGMType battleBgm;

    // バトル背景
    public Sprite battleBackground;

    // エンカウントした敵の番号(EnemyStatusTableSOのリスト番号)
    public List<int> encountEnemyNumbers = new();
}