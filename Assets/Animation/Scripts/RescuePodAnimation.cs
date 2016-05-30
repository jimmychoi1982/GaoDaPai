using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGU

public class RescuePodAnimation : MonoBehaviour {

	public UIElement_CostOverWindow costOverWindow;
	public Image bk;
	private GameObject cautionWindow;
	private Transform cautionWindowTra;
	private Text cautionText;
	private GameObject pod;
	private Transform podTra;
	private Image podLight;
	private Animator podAnimator;
	private AudioSource SE_podMove;
	private GameObject EF_LightBomb;

	private Vector3[] path;

	public bool rescuePodIn, rescuePodOut;


	// Use this for initialization
	void Start () {

		cautionWindow = costOverWindow.cautionWindow;
		cautionWindowTra = costOverWindow.cautionWindowTra;
		cautionText = costOverWindow.cautionText;
		pod = costOverWindow.pod;
		podTra = costOverWindow.podTra;
		podLight = costOverWindow.podLight;
		podAnimator = costOverWindow.podAnimator;
		SE_podMove = costOverWindow.SE_podMove;
		EF_LightBomb = costOverWindow.EF_LightBomb;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (rescuePodIn == true) {
			StartCoroutine(rescuePodInAnimation());
			rescuePodIn = false;
		}
		if (rescuePodOut == true) {
			StartCoroutine (rescuePodOutAnimation ());
			rescuePodOut = false;
		}
	
	}

	//ポッドIn---------------------------------------------------------------------------

	IEnumerator rescuePodInAnimation(){

		bk.DOFade (0.7f, 0.5f);
		yield return new WaitForSeconds(0.3f);

		StartCoroutine (WindowOpen ());

		StartCoroutine (PodIn ());
		yield return new WaitForSeconds(0.5f);

		//ポッドの蓋を開ける
		podAnimator.SetBool("podOpen", true);
		yield return new WaitForSeconds(0.1f);
	}

	//ウィンドゥオープン＆テキストフェードイン
	IEnumerator WindowOpen(){
		cautionWindowTra.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
		cautionWindow.SetActive (true);
		yield return new WaitForSeconds(0.01f);

		cautionWindowTra.DOScale (1, 0.3f).SetEase (Ease.OutBack);
		yield return new WaitForSeconds(0.2f);
		cautionText.DOFade (1, 0.3f);
		yield return new WaitForSeconds(0.4f);
	}

	//ポッドがやってくる
	IEnumerator PodIn(){
		//Reset
		pod.SetActive (true);
		podTra.localScale = new Vector3 (1, 1, 1);
		podTra.localPosition = new Vector3 (-655, 743, 0);
		yield return new WaitForSeconds(0.01f);

		SE_podMove.Play ();
		podTra.DOLocalMove(new Vector3(-655, 238, 0), 0.7f).SetEase(Ease.OutBack);
		yield return new WaitForSeconds(0.3f);
		podLight.DOFade (0.5f, 0.5f).SetLoops (-1, LoopType.Yoyo);
	}

	//ポッドOut---------------------------------------------------------------------------

	IEnumerator rescuePodOutAnimation(){
		//ポッドの蓋を閉じる
		podAnimator.SetBool("podOpen", false);
		yield return new WaitForSeconds(0.1f);

		StartCoroutine (WindowClose ());
		yield return new WaitForSeconds(0.2f);
		StartCoroutine (PodOut ());
		yield return new WaitForSeconds(0.8f);

		bk.DOFade (0, 0.5f);
	}

	//ウィンドゥオクローズ＆テキストフェードアウト
	IEnumerator WindowClose(){
		cautionText.DOFade (0, 0.2f);
		cautionWindowTra.DOScale (0.2f, 0.2f).SetEase (Ease.InCubic);
		yield return new WaitForSeconds(0.2f);
		cautionWindow.SetActive (false);
	}

	//ポッドが母艦に吸い込まれる動き
	IEnumerator PodOut(){
		path = new[] {
			new Vector3(-16, 198, 0),
			new Vector3(192, -155, 0),
			new Vector3(0, -414, 0),
		};
		podTra.DOLocalPath(path, 0.6f, PathType.CatmullRom).SetEase(Ease.InSine);
		podTra.DOScale (0.2f, 0.6f).SetEase(Ease.InSine);
		yield return new WaitForSeconds(0.6f);
		EF_LightBomb.GetComponent<Effect_Play_Limit> ().Effect_Start = true;
		pod.SetActive (false);
	}

}
