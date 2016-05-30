using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;//uGUI

public class Arch_Ray : MonoBehaviour 
{
	public UIElement_AttackTargetLazer lazer;
	private GameObject scrollLazer;
	private Transform getAttackPostion;
	private Transform getTargetPositon;
	private AudioSource lazerSE;
	private AudioSource lockOnSE;
	private ParticleSystem redLightEffect;
	private CanvasGroup lockonCanvasGroup;
	private Transform lockOnTra;
	private GameObject sightsOK, sightsBan;

	//2点間の距離を求める用
	private Vector3 atkPos, targetPos;
	private float distance;

	private LineRenderer linerenderer;
	private Renderer rend;

	private bool punchTrigger;

	//アニメ再生フラグ
	public bool attackActive;//ユニットをドラッグして攻撃しようとしたらON
	public bool attackActiveSE;//↑の状態のとき、同時にSEを再生
	public bool targetLockOn;//attackActiveの状態で、あるユニットに攻撃座標をあわせたとき
	public bool targetLockOff;//attackActiveの状態で、どのユニットにも攻撃座標を合わせていないとき
	public bool targetBanOn;//キーワード効果等で、攻撃座標を合わせていても攻撃できないとき


	// Use this for initialization
	void Start () 
	{

		scrollLazer = lazer.scrollLazer;
		getAttackPostion = lazer.getAttackPostion;
		getTargetPositon = lazer.getTargetPositon;
		lazerSE = lazer.lazerSE;
		lockOnSE = lazer.lockOnSE;
		redLightEffect = lazer.redLightEffect;
		lockonCanvasGroup = lazer.lockonCanvasGroup;
		lockOnTra = lazer.lockOnTra;
		sightsOK = lazer.sightsOK;
		sightsBan = lazer.sightsBan;

		//攻撃開始地点格納
		linerenderer = GameObject.Find ("Attack_Position").GetComponent<LineRenderer> ();

		//レイヤー管理に必要なのでコンポーネント取得
		rend = scrollLazer.GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//レイヤーを上昇させる
		rend.sortingOrder = 3;
	
		//攻撃するフラグがオンの時
		if (this.attackActive == true) 
		{
			//線描画を有効化
			linerenderer.enabled = true;

			//攻撃開始地点をセット
			linerenderer.SetPosition (0, getAttackPostion.position);	
			
			//標的場所をセット
			linerenderer.SetPosition (1, getTargetPositon.position);

			//テクスチャをスクロールさせる
			rend.material.mainTextureOffset = new Vector2 (rend.material.mainTextureOffset.x - Time.deltaTime * 6, 0);

			//タイリングを伸縮させる
			atkPos = getAttackPostion.transform.localPosition;
			targetPos = getTargetPositon.transform.localPosition;
			distance = (float)Vector3.Distance (atkPos, targetPos);
			rend.material.mainTextureScale = new Vector2(distance * 0.04f , 1);
		}
		//攻撃はしない
		else 
		{	
			//全ての描画オフ
			linerenderer.enabled = false;
			this.redLightEffect.Stop();
			this.targetLockOff = true;
			this.targetLockOn = false;
			this.targetBanOn = false;
			lazerSE.Stop ();
		}

		//attackActiveのときSEを鳴らす
		if(this.attackActiveSE == true)
		{
			lazerSE.Play ();
			this.attackActiveSE = false;
		}

		//ターゲットロックオン
		if(targetLockOn == true)
		{
			lockOnSE.Play ();
			StartCoroutine(lockOnAnimation());
			this.redLightEffect.Play();
			this.targetLockOn = false;
		}
		
		//ターゲットロックオフ
		if(targetLockOff == true)
		{
			linerenderer.SetColors(new Color(0, 0.9f, 0.4f, 1), new Color(0, 0.9f, 0.4f, 1));
			lockonCanvasGroup.DOFade (0f, 0.1f);
			lockOnTra.DOScale (1.1f, 0.1f).SetEase (Ease.OutSine);
			this.redLightEffect.Stop();
			this.targetLockOff = false;
		}
		
		//ターゲットに照準を当てられないとき
		if (targetBanOn == true)
		{
			StartCoroutine(lockOnAnimation());
			this.redLightEffect.Stop();
			this.targetBanOn = false;
		}

	}

	IEnumerator lockOnAnimation(){
		if (targetBanOn == true) {
			//エフェクトの色を赤にする
			linerenderer.SetColors (new Color (1, 0, 0, 0.6f), new Color (1, 0, 0, 0.6f));
			sightsOK.SetActive (false);
			sightsBan.SetActive (true);
		} else if (targetBanOn == false) {
			//エフェクトの色を緑にする
			linerenderer.SetColors(new Color(0, 0.9f, 0.4f, 1), new Color(0, 0.9f, 0.4f, 1));
			sightsBan.SetActive (false);
			sightsOK.SetActive (true);
		}
		lockonCanvasGroup.alpha = 0;
		lockOnTra.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
		yield return new WaitForSeconds (0.05f);
		lockonCanvasGroup.DOFade (0.7f, 0.2f);
		lockOnTra.DOScale (1, 0.2f).SetEase (Ease.OutBack);
	}

	public void SetAttackerPosition ( Vector3 atkPosition ) {
		getAttackPostion.position = atkPosition;
	}
	
	public void SetTargetPosition ( Vector3 tgtPosition ) {
		getTargetPositon.position = tgtPosition;
	}
}
