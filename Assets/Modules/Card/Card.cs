using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class Card : Module<Card> {
	//
	protected override List<string> staticTopics {
		get {
			return new List<string>() {
				"characteristics",
				"cardEffects",
				"cardsPilot",
				"cardsCrew",
				"cardsUnit",
				"cardsEvent",
				"cardsCounter",
			};
		}
	}


	//
	public TomeObject GetCrewCard(string cardId) {
		TomeObject card = staticData["cardsCrew"][cardId] as TomeObject;
		if (card == null) {
			card = staticData["cardsPilot"][cardId] as TomeObject;
		}

		return card;
	}

	//
	public TomeObject GetUnitCard(string cardId) {
		TomeObject card = staticData["cardsUnit"][cardId] as TomeObject;
		if (card == null) {
			card = staticData["cardsEvent"][cardId] as TomeObject;
		}
		if (card == null) {
			card = staticData["cardsCounter"][cardId] as TomeObject;
		}

		return card;
	}

	//
	public TomeObject GetCard(string cardId) {
		TomeObject card = GetCrewCard(cardId) as TomeObject;
		if (card == null) {
			card = GetUnitCard(cardId) as TomeObject;
		}

		return card;
	}

	public TomeObject GetEffect(string effectId) {
		return staticData ["cardEffects"] [effectId] as TomeObject;
	}

	//
	public bool IsPilotCard(string cardId) {
		return (staticData["cardsPilot"] as TomeObject)[cardId] != null;
	}

	//
	public bool IsUnitCard(string cardId) {
		return (staticData["cardsUnit"] as TomeObject)[cardId] != null;
	}

	//
	public bool IsCounterCard(string cardId) {
		return (staticData["cardsCounter"] as TomeObject)[cardId] != null;
	}

	//
	public bool IsEventCard(string cardId) {
		return (staticData["cardsEvent"] as TomeObject)[cardId] != null;
	}

	//
	public bool IsUsoEwin(string cardId) {
		return cardId == "cw12001156";
	}

	//
	public bool IsRaru(string cardId) {
		return cardId == "cw12002151";
	}

	//
	public bool IsAmada(string cardId) {
		return cardId == "cw12002153";
	}

}