using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class Deck : Module<Deck> {
	public JArray tDecks = null;

	public const int unitDeckLimit = 40;
	public const int crewDeckLimit = 20;

	//
	protected override List<string> staticTopics {
		get {
			return new List<string>() {
				"cardTypes",
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
	protected override string commandPrefix { get { return "deck"; } }
	protected override List<string> commands {
		get {
			return new List<string>() {
				"setAvatar",
				"setCards",
				"setMain",
				"setMotherShip",
				"setName",
			};
		}
	}

	
	//
	public void Setup(Action<Exception> cb) {
		mage.eventManager.on("session.set", (object sender, JToken session) => {
			JArray queries = new JArray();
			
			//
			for (int deckId = 0; deckId < 10; deckId += 1) {
				JObject deckQuery = new JObject();
				queries.Add(deckQuery);
				
				deckQuery.Add("topic", new JValue("userDecks"));
				
				JObject deckIndex = new JObject();
				deckIndex.Add("userId", new JValue(mage.session.GetActorId()));
				deckIndex.Add("deckId", new JValue(deckId));
				deckQuery.Add("index", deckIndex);
			}
			
			JObject options = new JObject();
			options.Add("optional", new JValue(true));
			
			mage.archivist.mget(queries, options, (Exception error, JToken data) => {
				if (error != null) {
					logger.data(error).error("Could not retrieve user deck data:");
					return;
				}
				
				tDecks = (JArray)data;
				logger.debug("Retrieved user deck data");
			});
		});

		cb(null);
	}
	
	
	//
	public IEnumerator setCards(Action<Exception> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("setCards", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
	
	//
	public IEnumerator setAvatar(Action<Exception> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("setAvatar", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
	
	//
	public IEnumerator setMotherShip(Action<Exception> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("setMotherShip", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
	
	//
	public IEnumerator setName(Action<Exception> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("setName", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
	
	//
	public IEnumerator setMain(Action<Exception> cb) {
		JObject arguments = new JObject();
		
		UsercommandStatus status = this.command("setMain", arguments);
		while (!status.done) {
			yield return null;
		}
		
		cb (status.error);
	}
}