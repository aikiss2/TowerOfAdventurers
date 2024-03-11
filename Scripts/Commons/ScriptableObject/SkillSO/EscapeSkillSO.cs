using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skill/EscapeSkillSO")]
// バトルから逃げるスキルクラス
public class EscapeSkillSO : SkillSO
{
    // 逃げるSE
    [SerializeField] SEType seType;

    // スキル終了時のWAIT時間(秒)
    [SerializeField] float waitTime;

    // スキル実行メソッド
    public override IEnumerator Execute(BattleCharacter executer, BattleCharacter target, List<BattleCharacter> friends, List<BattleCharacter> opponents, RawImage effectScreen)
    {
        // SEを鳴らす
        SoundManager.soundManager.SEPlay(seType);

        // イベントバトルの場合は逃げられない旨を表示する
        if (Utility.nextEvent != EventTableIndex.None)
        {
            GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("この戦闘では逃げられない！");
        }
        // イベントバトル以外
        else
        {
            int judgePoint = Random.Range(0, 100);
            
            // 逃げられる確率の値を下回ったら、逃げることに成功
            if(judgePoint < Define.BATTLE_ESCAPE_RATE)
            {
                executer.IsEscape = true;
            }
            else
            {
                GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("逃げられなかった！");
            }
        }

        yield return new WaitForSeconds(waitTime);
    }
}
