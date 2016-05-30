using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameView {
	public class GameChoiceTarget : MonoBehaviour {
		Game game { get { return Game.Instance; }}
		GameView gameView { get { return GameView.Instance; }}

		//
		public BoardIconsManager boardIconsEnemy;
		public BoardIconsManager boardIconsMe;

		//
		public void Open(TomeArray possibleTargets) {
			//
			gameObject.SetActive(true);
			
			//
			boardIconsEnemy.SetData(game.tCurrentGame["board"][gameView.enemyUserId] as TomeArray);
			foreach (BoardIcon boardIcon in boardIconsEnemy.boardIcons.Values) {
				string instanceId = (string)boardIcon.iconData["instanceId"];
				if (possibleTargets.IndexOf(instanceId) < 0) {
					boardIcon.SetTap(true);
				} else {
					boardIcon.gameObject.AddComponent<ChoiceTargetClick>().onClick += () => {
						ConfirmTarget(instanceId);
					};
				}
			}

			//
			boardIconsMe.SetData(game.tCurrentGame["board"][gameView.meUserId] as TomeArray);
			foreach (BoardIcon boardIcon in boardIconsMe.boardIcons.Values) {
				string instanceId = (string)boardIcon.iconData["instanceId"];
				if (possibleTargets.IndexOf(instanceId) < 0) {
					boardIcon.SetTap(true);
				} else {
					boardIcon.gameObject.AddComponent<ChoiceTargetClick>().onClick += () => {
						ConfirmTarget(instanceId);
					};
				}
			}
		}
		
		public void Close() {
			gameObject.SetActive(false);
		}

		public void ConfirmTarget(string instanceId) {
			gameView.AnswerTargetQuestion(instanceId);
		}
	}
}