using UnityEngine;
using System.Collections;
using UnityEngine.UI;//uGUI

public class UIElement_AttackTargetLazer : MonoBehaviour {

	public GameObject scrollLazer;
	public Transform getAttackPostion;
	public Transform getTargetPositon;

	//SE
	public AudioSource lazerSE;
	public AudioSource lockOnSE;//targetLockOnのとき再生

	//LockOn
	public ParticleSystem redLightEffect;
	public CanvasGroup lockonCanvasGroup;
	public Transform lockOnTra;
	public GameObject sightsOK, sightsBan;
}
