using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class BNIDManager {

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private BNID bnid { get { return BNID.Instance; }}
	private Logger logger { get { return mage.logger("Steward"); } }
	
	private Steward steward;

	public BNIDManager () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
	}

	public void OpenInheritWindow () {
		GetWindow (1);
	}

	public void OpenMigratedWindow () {
		GetWindow (92);
	}
	
	public void OpenContinueWindow () {
		GetWindow (6);
	}
	
	public void SetUpAssociateBNID () {
		steward.SetNewTask (bnid.startAssociate ((Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error startAssociate:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			if (steward.ResponseHasErrorCode(result)) return;
			
			Application.OpenURL (result ["url"].ToString ());
			return;
		}));
	}
	
	public void SetUpMigrateBNID () {
		steward.SetNewTask (bnid.startDeviceMigration ((Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error startAssociate:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode(result)) return;
			
			GameSettings.IsMigrated = 1;
			GetWindow (5);
			return;
		}));
	}
	
	public void SetUpRegisterBNID () {
		new Task (bnid.startRegisterNewDevice ((Exception e, JToken result) => {
			if (e != null) {
				logger.data (e).error ("Error startAssociate:");
				steward.OpenExceptionPopUpWindow ();
				return;
			}
			
			if (steward.ResponseHasErrorCode(result)) return;

			Application.OpenURL (result["url"].ToString());
			return;
		}));
	}
	
	public void ExecIntent (Uri uri) {
		string urlSchemeHost = uri.Host;
		string urlSchemeParam = null;
		Dictionary<string, string> urlSchemeParams = new Dictionary<string, string> ();
		foreach (var queryPart in uri.Query.Substring (1).Split('&')) {
			var keyValue = queryPart.Split ('=');
			if (keyValue.Length == 2) {
				urlSchemeParams.Add (keyValue [0], keyValue [1]);
			}
		}
		if (urlSchemeParams.Count > 0 && urlSchemeParams.ContainsKey ("code")) {
			urlSchemeParam = urlSchemeParams ["code"];
		}
		
		if (!String.IsNullOrEmpty (urlSchemeHost)) {
			if (urlSchemeHost == "bnid") {
				steward.SetNewTask (bnid.endAssociate (urlSchemeParam, (Exception e, JToken result) => {
					if (e != null) {
						logger.data (e).error ("Error endAssociate:");
						steward.OpenExceptionPopUpWindow ();
						return;
					}
					if (steward.ResponseHasErrorCode (result)) return;
					
					GetWindow (3);
					return;
				}));
			} else if (urlSchemeHost == "migration") {
				steward.SetNewTask (bnid.endDeviceMigration (urlSchemeParam, (Exception e, JToken result) => {
					if (e != null) {
						logger.data (e).error ("Error endDeviceMigration:");
						steward.OpenExceptionPopUpWindow ();
						return;
					}
					if (steward.ResponseHasErrorCode (result)) return;
					
					GetWindow (5);
					return;
				}));
			} else if (urlSchemeHost == "register") {
				steward.SetNewTask (bnid.endRegisterNewDevice (urlSchemeParam, (Exception e, JToken result) => {
					if (e != null) {
						logger.data (e).error ("Error endRegisterNewDevice:");
						steward.OpenExceptionPopUpWindow ();
						return;
					}
					
					if (steward.ResponseHasErrorCode (result)) return;

					if (bool.Parse (result ["result"].ToString ()) == false) {
						logger.data (result).error ("Error endRegisterNewDevice:");
						GetWindow (91);
						return;
					}

					GetWindow (8);
					return;
				}));
			}
		}
	}

	private void GetWindow (int pattern) {
		switch (pattern) {
		case 1:
			Action nextAction;
			if (bool.Parse (user.tUser["isAssociated"].ToString ())) {
				nextAction = () => {GetWindow (4);};
			} else {
				nextAction = () => {GetWindow (2);};
			}
			steward.OpenDialogWindowL ("機種変更設定", "【機種変更の流れ】\n\n" +
			                           "１．機種変更をする端末から専用のWEBサイトにアクセスします\n" +
			                           "２．WEBサイトの案内に従い、機種変更のための準備をします\n" +
			                           "　　※登録しているバンダイナムコIDでログインを行います\n" +
			                           "３．メニュー/タイトルの「機種変更」から専用のWEBサイトにアクセスします\n" +
			                           "４．WEBサイトの案内に従い、機種変更の手順を完了します\n" +
			                           "５．別の端末から「データ引き継ぎ」を行います", "次へ", nextAction, "閉じる", () => {});
			break;

		case 2:
			steward.OpenDialogWindow ("機種変更設定", "機種変更の設定には、バンダイナムコIDに登録する必要があります\n\n" +
			                          "バンダイナムコIDを登録後にお試しください", "会員登録", () => {SetUpAssociateBNID ();}, "閉じる", () => {});
			break;

		case 3:
			steward.OpenMessageWindow ("登録完了", "バンダイナムコIDの登録が完了しました", (Application.loadedLevelName == "TitleView" ? "リロード" : "タイトルへ"), () => {steward.LoadNextScene ("TitleView", Application.loadedLevelName == "TitleView" ? true : false);});
			break;

		case 4:
			var inheritNoticeWindow1 = steward.InitCustomWindow ("InheritNoticeWindow")
				.SetSubject ("機種変更設定")
				.SetText ("Panel/MessageText", "機種変更をするためにデータ引き継ぎを開始します\n" +
				          "よろしいですか？\n\n" +
					      "隊員番号：" + user.tUser["dogTag"].ToString () + "\n\n" +
					      "※注意事項を読んでからデータの引き継ぎ設定を行ってください\n" +
					      "※隊員番号を記録（スクリーンショットなど）しておいてください\n" +
					      "　トラブルが発生した際に必要になる場合がございます\n\n" +
					      " -----------------------------------------------------\n\n" +
					      "【注意事項】\n" +
					      "・ひとつのバンダイナムコIDで保存できる引き継ぎデータはひとつだけです\n" +
					      "・ひとつのバンダイナムコIDに引き継ぎデータを保存できるのは1度だけです\n" +
					      "・バンダイナムコIDに保存した引き継ぎデータの削除、変更、上書きへの\n" +
					      "対応は行っておりません\n");

			steward.OpenCustomWindow (inheritNoticeWindow1, "OK", () => {SetUpMigrateBNID ();}, "戻る", () => {});
			break;
			
		case 5:
			steward.OpenMessageWindow ("機種変更準備完了", "引き継ぎデータの生成に成功しました", (Application.loadedLevelName == "TitleView" ? "リロード" : "タイトルへ"), () => {steward.LoadNextScene ("TitleView", Application.loadedLevelName == "TitleView" ? true : false);});
			break;

		case 6:
			steward.OpenDialogWindowL ("データ引き継ぎ設定", "【データ引き継ぎの流れ】\n\n" +
			                           "１．データを引き継ぎしたい端末から専用のWEBページにアクセスします\n" +
			                           "２．「機種変更」よりデータ引き継ぎの準備を完了します\n" +
			                           "３．新しい端末から「データ引き継ぎ設定」をタップします\n" +
			                           "４．専用のWEBページにアクセスし、案内に従ってデータ引き継ぎを完了します", "次へ", () => {GetWindow (7);}, "閉じる", () => {});
			break;

		case 7:
			var inheritNoticeWindow2 = steward.InitCustomWindow ("InheritNoticeWindow")
				.SetSubject ("データ引き継ぎ設定")
				.SetText ("Panel/MessageText", "専用のWEBページを起動します\n" +
					      "WEBページの案内に従って、データ引き継ぎの準備を進めてください\n\n" +
					      "※注意事項を読んでからデータの引き継ぎ設定を行ってください\n" +
					      "データの引き継ぎを開始してもよろしいですか？\n\n" +
					      " -----------------------------------------------------\n\n" +
					      "【注意事項】\n" +
					      "・ひとつのバンダイナムコIDで保存できる引き継ぎデータはひとつだけです\n" +
					      "・ひとつのバンダイナムコIDに引き継ぎデータを保存できるのは1度だけです\n" +
					      "・バンダイナムコIDに保存した引き継ぎデータの削除、変更、上書きへの\n" +
					      "対応は行っておりません\n");
			
			steward.OpenCustomWindow (inheritNoticeWindow2, "WEBサイト", () => {SetUpRegisterBNID ();}, "戻る", () => {});
			break;
			
		case 8:
			steward.OpenMessageWindow ("データ引き継ぎ完了", "データ引き継ぎに成功しました", (Application.loadedLevelName == "TitleView" ? "リロード" : "タイトルへ"), () => {steward.LoadNextScene ("TitleView", Application.loadedLevelName == "TitleView" ? true : false);});
			break;
			
		case 90:
			steward.OpenMessageWindow ("登録失敗", "バンダイナムコIDの登録に失敗しました\n再度お試しください", "閉じる", () => {});
			break;
			
		case 91:
			steward.OpenMessageWindow ("登録失敗", "引き継ぎデータの生成に失敗しました\n再度お試しください", "閉じる", () => {});
			break;
			
		case 92:
			steward.OpenMessageWindow ("データ引き継ぎ完了", "データの引き継ぎが完了している端末です\n\n" +
			                           "再度この端末でゲームをプレイする場合" +
			                           "アプリをアンインストールした後" +
			                           "再インストールをしてください", "アプリ終了", () => {Application.Quit ();});
			break;
		}
	}
}
