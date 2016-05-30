using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class CardPurchaseView : MonoBehaviour {
	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Logger logger { get { return mage.logger("CardPurchaseView"); } }
	
	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	private Steward steward;
	
	// Use this for initialization
	void Start () {
		steward = GameObject.Find ("Steward").GetComponent<Steward> ();
	}
	
	public void OpenMerchandiseWebView() {
		steward.OpenWebView ("http://www.gundam-cw.com/app_merchandise.php");
	}
	
	public void OpenCardInfoWebView () {
		steward.OpenWebView ("http://www.gundam-cw.com/app_cardinfo.php");
	}

	public void OpenMapInfoWebView () {
		steward.OpenWebView ("http://www.gundam-cw.com/app_mapinfo.php");
	}
}
