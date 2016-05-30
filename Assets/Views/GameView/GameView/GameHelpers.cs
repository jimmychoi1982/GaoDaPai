using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameView {
	public class GameHelpers {
		private static Card card { get { return Card.Instance; }}
		private static Game game { get { return Game.Instance; }}
		private static GameView gameView { get { return GameView.Instance; }}
		private static Mage mage { get { return Mage.Instance; }}
		private static Logger logger { get { return mage.logger("GameHelpers"); } }

		//
		public static bool IsUsersTurn(string userId) {
			string currentTurn = (string)game.tCurrentGame["currentTurn"];
			return userId == currentTurn;
		}

		//
		public static TomeObject FindCardData(string instanceId) {
			// My cards
			if (game.tCurrentGameSecrets["hand"]["unitHand"][instanceId] != null) {
				return game.tCurrentGameSecrets["hand"]["unitHand"][instanceId] as TomeObject;
			}
			if (game.tCurrentGame["drawnCards"][gameView.meUserId]["crewCards"][instanceId] != null) {
				return game.tCurrentGame["drawnCards"][gameView.meUserId]["crewCards"][instanceId] as TomeObject;
			}

			// Enemy cards
			if (game.tCurrentGame["drawnCards"][gameView.enemyUserId]["unitCards"][instanceId] != null) {
				return game.tCurrentGame["drawnCards"][gameView.enemyUserId]["unitCards"][instanceId] as TomeObject;
			}
			if (game.tCurrentGame["drawnCards"][gameView.enemyUserId]["crewCards"][instanceId] != null) {
				return game.tCurrentGame["drawnCards"][gameView.enemyUserId]["crewCards"][instanceId] as TomeObject;
			}

			return null;
		}

		//
		public static TomeObject FindBoardIconData(string instanceId) {
			foreach (TomeObject boardIconData in game.tCurrentGame["board"][gameView.meUserId]) {
				if ((string)boardIconData["instanceId"] == instanceId) {
					return boardIconData;
				}
			}
			foreach (TomeObject boardIconData in game.tCurrentGame["board"][gameView.enemyUserId]) {
				if ((string)boardIconData["instanceId"] == instanceId) {
					return boardIconData;
				}
			}
			if (game.tCurrentGame["motherships"][instanceId] != null) {
				return game.tCurrentGame["motherships"][instanceId] as TomeObject;
			}

			return null;
		}

		//
		public static TomeObject FindCounterData(string instanceId) {
			foreach (TomeObject counterData in game.tCurrentGameSecrets["counters"]) {
				if ((string)counterData["instanceId"] == instanceId) {
					return counterData;
				}
			}
			foreach (TomeObject counterData in game.tCurrentGame["counters"][gameView.enemyUserId]) {
				if ((string)counterData["instanceId"] == instanceId) {
					return counterData;
				}
			}
			return null;
		}

		//
		public static BoardIcon FindBoardIcon(string instanceId) {
			if (gameView.myBoardManager.boardIcons.ContainsKey(instanceId)) {
				return gameView.myBoardManager.boardIcons[instanceId];
			}
			if (gameView.enemyBoardManager.boardIcons.ContainsKey(instanceId)) {
				return gameView.enemyBoardManager.boardIcons[instanceId];
			}
			return null;
		}

		//
		public static Mothership FindMothership(string instanceId) {
			if (instanceId == gameView.meUserId) {
				return gameView.myMothership;
			}
			if (instanceId == gameView.enemyUserId) {
				return gameView.enemyMothership;
			}
			return null;
		}

		//
		public static bool PilotExistsOnMyBoard(string cardId) {
			foreach (TomeObject boardIconData in game.tCurrentGame["board"][gameView.meUserId]) {
				if (boardIconData["pilot"] != null && (string)boardIconData["pilot"]["cardId"] == cardId) {
					return true;
				}
			}

			return false;
		}

		//
		public static RectTransform GetTargetIconRect(string instanceId) {
			BoardIcon targetIcon = GameHelpers.FindBoardIcon(instanceId);
			if (targetIcon != null) {
				return targetIcon.icon.GetComponent<RectTransform>();
			}
			Mothership targetMothership = GameHelpers.FindMothership(instanceId);
			if (targetMothership != null) {
				return targetMothership.GetComponent<RectTransform>();
			}
			return null;
		}

		//
		public static int UnitGetAtk(TomeObject iconData, bool originalValue = false) {
			int finalAtk = (iconData["currentAtk"] != null) ? (int)iconData["currentAtk"] : (int)iconData["atk"];
			if (iconData["pilot"] != null) {
				finalAtk += (int)iconData["pilot"]["atk"];
			}

			return (finalAtk >= 0) ? finalAtk : 0;
		}

		//
		public static int UnitGetDef(TomeObject iconData, bool originalValue = false) {
			int finalDef = (iconData["currentDef"] != null) ? (int)iconData["currentDef"] : (int)iconData["def"];
			if (iconData["pilot"] != null) {
				finalDef += (int)iconData["pilot"]["def"];
			}

			return (finalDef >= 0) ? finalDef : 0;
		}

		//
		public static bool UnitHasSkill(TomeObject iconData, string skill) {
			if (iconData == null || iconData["currentEffects"] == null) {
				return false;
			}
			return (iconData["currentEffects"] as TomeArray).IndexOf(skill) >= 0;
		}

		// This is the least disgusting way to do pilots that can board while being tapped, see the related
		// function for that
		public static TomeObject UnitGetSkillOfType(TomeObject iconData, string skillType, string skillFunction) {
			if (iconData == null || iconData["currentEffects"] == null) {
				return null;
			}

			foreach (JToken effectId in iconData["currentEffects"]) {
			    TomeObject effectData = card.GetEffect(effectId.ToString());
				if (effectData == null) {
					logger.warning (String.Format("Could not locate effectId \"{0}\" on card \"{1}\"",
						effectId.ToString(),
						iconData["iconData"]["cardId"].ToString()
					));
					continue;
				}
				if (effectData["type"].ToString() == skillType && effectData["function"].ToString() == skillFunction) {
					return effectData;
				}
			}

			return null;
		}

		//
		public static bool UnitHasCharacteristic(TomeObject iconData, string characteristic) {
			if (iconData == null || iconData["characteristicId"] == null) {
				return false;
			}
			return (iconData["characteristicId"] as TomeArray).IndexOf(characteristic) >= 0;
		}

		//
		public static bool UnitHasTaunt(TomeObject iconData) {
			return UnitHasSkill(iconData, "preventTargetMyMothershipOnEnemyTurn");
		}

		//
		public static bool UnitHasBombardment(TomeObject iconData) {
			return UnitHasCharacteristic(iconData, "bombardment");
		}

		//
		public static bool IconIsTapped(TomeObject iconData) {
			if (iconData["tapped"] == null) {
				return false;
			}
			return (int)iconData["tapped"] > 0;
		}

		//
		public static bool CardHasSkill(TomeObject iconData, string skill) {
			if (iconData == null || iconData["effects"] == null) {
				return false;
			}
			return (iconData["effects"] as TomeArray).IndexOf(skill) >= 0;
		}

		//
		public static bool CardCanUsePilot(TomeObject cardData, TomeObject iconData) {
			string cardId = (string)iconData["cardId"];

			// Use if pilot is not tapped
			if (!IconIsTapped(iconData)) {
				return true;
			}

			// Some pilots have an effect that allows them to ride even if tapped
			if (PilotCanLaunchEvenIfTapped(iconData)) {
				return true;
			}

			// This is the opposite from the above, that card itself allows tapped pilots
			if (CardAllowsImmobilisedPilotLaunch(cardData)) {
				return true;
			}

			// Special cases below (CardX + PilotY)
			if (card.IsUsoEwin(cardId) && CardAllowsImmobilisedUsoEwinLaunch(cardData)) {
				return true;
			}

			if (card.IsRaru(cardId) && CardAllowsImmobilisedRaruLaunch(cardData)) {
				return true;
			}

			if (card.IsAmada(cardId) && CardAllowsImmobilisedAmadaLaunch(cardData)) {
				return true;
			}

			if (CardAllowsRideDelazFleetEvenIfTapped (cardData, iconData)) {
				return true;
			}

			// A "better" (kinda?) generic method to check for this, need to migrate old pilots
			TomeObject effectData = UnitGetSkillOfType(cardData, "special", "allowsImmobilisedXPilotToBoard");
			if (effectData != null && effectData["params"] != null && PilotIsCharacterId(iconData, effectData["params"]["characterId"].ToString())) {
				return true;
			}

			// Didn't find anything, card is not allowed to use this pilot
			return false;
		}

		//
		public static bool CardAllowsImmobilisedPilotLaunch(TomeObject iconData) {
			return CardHasSkill(iconData, "allowsImmobilisedPilotToBoard");
		}

		//
		public static bool CardAllowsImmobilisedUsoEwinLaunch(TomeObject iconData) {
			return CardHasSkill(iconData, "allowsImmobilisedUsoEwinPilotToBoard");
		}

		//
		public static bool CardAllowsImmobilisedRaruLaunch(TomeObject iconData) {
			return CardHasSkill(iconData, "allowsImmobilisedRaruPilotToBoard");
		}

		//
		public static bool CardAllowsImmobilisedAmadaLaunch(TomeObject iconData) {
			return CardHasSkill(iconData, "allowsImmobilisedAmadaPilotToBoard");
		}

		//
		public static bool CardAllowsRideDelazFleetEvenIfTapped(TomeObject unitData, TomeObject iconData) {
			return CardHasSkill (iconData, "allowRideDelazFleetUnitEvenIfTapped") &&
				UnitHasCharacteristic (unitData, "delaz-fleet");
		}

		//
		public static bool PilotCanLaunchEvenIfTapped(TomeObject iconData) {
			return CardHasSkill(iconData, "allowRideUnitEvenIfTapped");
		}

		public static bool PilotIsCharacterId(TomeObject iconData, string characterId) {
			return iconData["characterId"].ToString() == characterId;
		}

		//
		public static Sprite GetCrewFrame(string color, bool tapped) {
			string path = "BoardIcons/CostIcons/ui_crew_frame_";
			path += (tapped) ? "tapped" : color;
			return Resources.Load<Sprite>(path);
		}

		//
		public static Sprite GetPilotFrame(bool tapped, bool hasScramble) {
			string path = "BoardIcons/CostIcons/ui_pilot_frame";
			path += (tapped && !hasScramble) ? "_tapped" : "";
			path += (tapped && hasScramble) ? "_scramble" : "";
			return Resources.Load<Sprite>(path);
		}

		//
		public static Sprite GetUnitFrame(string color, string size, bool hasTaunt, bool tapped) {
			if (tapped) {
				color = "tapped";
			}

			string path = "BoardIcons/UnitIcons/ui_frame";
			path += (size == "L") ? "_L" : "";
			path += (hasTaunt) ? "_taunt" : "_normal";
			path += "_" + color;
			return Resources.Load<Sprite>(path);
		}

		//
		public static Sprite GetUnitHighlight(string size, bool hasTaunt) {
			string path = "BoardIcons/UnitIcons/ui_highlight";
			path += (size == "L") ? "_L" : "";
			path += (hasTaunt) ? "_taunt" : "_normal";
			path += "_a";
			return Resources.Load<Sprite>(path);
		}

		//
		public static Sprite GetUnitNumberFrame(string color, bool isTapped) {
			if (isTapped) {
				color = "tapped";
			}

			string path = "BoardIcons/UnitIcons/ui_number_frame_" + color;
			return Resources.Load<Sprite>(path);
		}

		//
		public static Sprite GetUnitSizeIcon(string color, string size, bool isTapped) {
			if (isTapped) {
				color = "tapped";
			}

			string path = "BoardIcons/UnitIcons/ui_size_" + color + "_" + size;
			return Resources.Load<Sprite>(path);
		}

		//
		public static int UnitGetDisplayType(bool isTapped, bool isTaunt) {
			if (!isTapped && !isTaunt) {
				return (int)LoadAssetBundle.DisplayType.Icon;
			} else if (isTapped && !isTaunt) {
				return (int)LoadAssetBundle.DisplayType.Iconb;
			} else if (!isTapped && isTaunt) {
				return (int)LoadAssetBundle.DisplayType.Icong;
			} else {
				return (int)LoadAssetBundle.DisplayType.Icongb;
			}
		}
	}
}