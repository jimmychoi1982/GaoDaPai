using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutoriaDialogueDataStep2 : TutorialData {
	
	public int currentIndex = -1;

	public List<string> DialogueList = new List<string>() {
		"ユニットは特殊な効果を持っている場合があります。" +
		"+ここでは戦闘で重要になる【防衛】について説明しましょう。",                                                           

		"相手が【防衛】効果を持つユニットを出撃させました。" +
		"+【防衛】ユニットが戦場にいると相手の母艦を攻撃できなくなります。",                                                        

		"【防衛】効果を確認してみましょう。",                                                                                  

		"相手の母艦を攻撃するには【防衛】ユニットを撃破しましょう。",                                                                    

		"【防衛】ユニットを撃破しましたね。" +
		"+これで相手の母艦に攻撃が可能になりました。" +
		"+ユニットを出撃させて相手の母艦を攻撃しましょう。",                                                                                     

		"相手の母艦にダメージを与えることができました。" +
		"+このように【防衛】ユニットがある場合は、\n先に【防衛】ユニットを撃破しなければ相手の母艦を攻撃できません。" +
		"+よく覚えておくことね。",                                                                            

		"続いて【防衛】効果に関連する【砲撃】について説明します。" +
		"+【砲撃】は相手の【防衛】効果を無視して\n相手の母艦にダメージを与えることができます。" +
		"+それでは、はじめましょう。" +
		"+一旦戦場をリセットします。",                                                    

		"【砲撃】ユニットのガンタンクをドローしました。\nガンタンクを出撃させてください。" +
		"+ガンタンクを出撃させユニットのアイコンを下にスワイプすると\n【砲撃】を行います。",                                                    

		"相手の母艦にダメージを与えられましたね。" +
		"+このように相手が【防衛】ユニットを出撃させたときは、\n【砲撃】ユニットを使うのも戦略のひとつです。"                                                    
	};

	public override int GetCurrentIndex (){
		return currentIndex;
	}

	public override void SetCurrentIndex (int index){
		currentIndex = index;
	}

	public override string GetDialogueOfIndex (int index){
		return DialogueList [index];
	}
}
