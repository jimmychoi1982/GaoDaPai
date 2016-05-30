using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutoriaDialogueDataStep3 : TutorialData {
	
	public int currentIndex = -1;

	public List<string> DialogueList = new List<string>() {
		"続いて【制圧】のユニット効果について説明します。" +
		"+相手の【制圧】効果を持つユニットが戦場にいると、" +
		"+パイロットが搭乗していないユニットでは\n相手に攻撃することができなくなってしまいます。" +
		"+【制圧】効果を持ったユニットがいるときには\nパイロットが重要になります。" +
		"+パイロットはユニットに搭乗させることで\n能力を上昇させることができます。",  
		
		"ハイザック・カスタムの【制圧】効果は「パイロットが搭乗していない\n総コスト1～3のユニットは攻撃ができない」です。" +
		"+このグレイズはパイロットが搭乗しておらず、コストも2なので\n【制圧】の効果を受けてしまいます。" +
		"+まずは【制圧】を持つユニットを撃破しましょう。",                                                                                                  

		"ユニットのセットアップが完了しましたね。" +
		"+それではユニットにパイロットを乗せてみましょう。",    

		"ガンダム・エアマスターはコスト3ですが、\nパイロットが搭乗していたので攻撃ができました。" +
		"+パイロットが搭乗していないときは【制圧】の指定コスト以外の\nコストのユニットであれば攻撃可能です。" +
		"+【制圧】効果を持つユニットを撃破したので\n相手の母艦を攻撃しましょう。",      

		"相手の母艦を攻撃できましたね。" +
		"+このようにユニットには様々な効果が存在します。" +
		"+戦況に合わせてユニットをうまく活用しましょう。",                                                                                                  
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
