using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace GameView {
	public class PlayerData : MonoBehaviour {
		Game game { get { return Game.Instance; }}
		User user { get { return User.Instance; }}

		//
		[Header("User name")]
		public Text userName;

		//
		public TomeObject data;

		//
		void Start() {
			userName.text = String.Empty;
		}
		
		//
		public void SetData(TomeObject data) {
			this.data = data;

			// load username data even if coudn't get that
			if (data ["name"] != null) {
				userName.text = (string)data ["name"];
			} else if (user.tUser ["userName"] != null) {
				userName.text = (string)user.tUser ["userName"];
			} else {
				userName.text = "unknown";
			}
		}
	}
}