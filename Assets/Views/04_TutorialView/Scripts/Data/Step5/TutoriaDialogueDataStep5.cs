using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutoriaDialogueDataStep5 : TutorialData {
	
	public int currentIndex = -1;

	public List<string> DialogueList = new List<string>() {
		"カウンターカードについて説明します。" +
		"+カウンターカードは自分のターンにセットすることで、\n敵ユニットの攻撃に対して強力な効果を発動します。" +
		"+ただし効果を発動させるためには、\nブリッジに発動条件を満たすカードが必要になります。" +
		"+発動コストや条件が満たされなければ、\n不発となりますので注意してください。" +
		"+それでは、はじめましょう。",             

		"相手が攻撃をしてきます。 " +
		"+カウンターカードは相手が攻撃することで発動します。" +
		"+それではカウンター効果を確認してみましょう。",

		"カウンター効果で攻撃してきたユニットとその隣のユニットを\n撃破しました。" +
		"+このようにカウンターカードは敵が行動してきたときに発動されます。" +
		"+もう一度カウンターカードを試してみましょう。",

		"カウンター効果で攻撃してきたユニットを撃破して、\nさらに母艦にもダメージを与えました。" +
		"+カウンターカードは複数セットすることもできます。" +
		"+複数セットされた場合は後からセットされたものが先に発動します。" +
		"+カウンターカードには様々な効果があるので、\n戦況によって使い分けてください。",
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
