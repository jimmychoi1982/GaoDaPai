using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutoriaDialogueDataStep4 : TutorialData {
	
	public int currentIndex = -1;

	public List<string> DialogueList = new List<string>() {
		"ここではイベントカードの効果について説明します。" +
		"+イベントカードとは手札からコストを支払って使用する、\n1回限りの使い捨てカードです。" +
		"+イベントカードにはユニットを強化させたり、相手ユニットに\nダメージを与えたり様々な効果があります。",     

		"イベントカードをドローしたわ。" +
		"+さっそくコストを支払ってイベントカードを使ってみましょう。",

		"グレイズのATKとDEFが強化されました。" +
		"+これで相手ユニットを撃破できます。",

		"このイベントカードは直接相手ユニットにダメージを与えます。" +
		"+イベントカードを使って相手ユニットを撃破しましょう。",

		"これらがイベントカードの効果です。" +
		"+イベントカードには様々な効果があります。" +
		"+戦況によって使い分けましょう。",
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
