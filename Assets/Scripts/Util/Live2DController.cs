using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using live2d;
using live2d.framework;
using DG.Tweening;

using Newtonsoft.Json.Linq;

public class Live2DController : MonoBehaviour {
	public Text live2DMessage;
	public Image messageWindow;

	private GameObject live2DElementsPrefab;
	private TextAsset mocFile;
	private TextAsset physicsFile;
	private Texture2D[] textureFiles;
	private TextAsset[] mtnFiles;
	private AudioSource[] voice;
	private TextAsset idleMtnFile;
	private int randomFreeAction;

	private Dictionary<string, object> operatorTalks;
	private string operatorId;

	private float elapsed = 0f;
	private Live2DMotion motion;
	private MotionQueueManager motionManager;
	
	private Live2DModelUnity live2DModel;
    private EyeBlinkMotion eyeBlink = new EyeBlinkMotion();
    private L2DTargetPoint dragMgr = new L2DTargetPoint();
    private L2DPhysics physics;
    private Matrix4x4 live2DCanvasPos;

	private bool isSetup = false; 
	private bool isEnabled = false; 
	private bool isLoadLevelFirst = true;
	private bool noReply;
	private bool AnimationStartFlg;
	private bool pushMessage;

	private int currentAnimIndex = -1;

	private string timeZone;

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Master master { get { return Master.Instance; }}
	private Logger logger { get { return mage.logger("Live2DController"); } }

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }

	void Awake () {
		if (GameObject.FindGameObjectsWithTag ("Live2D").Length > 1) {
			Destroy (this.transform.parent.parent.parent.gameObject);
		}
		DontDestroyOnLoad (this.transform.parent.parent.parent.gameObject);
		foreach (Transform n in transform) {
			Destroy (n.gameObject);
		}
	}

	public void SetUp (AudioMixer audioMixer) {
		operatorTalks = ConvertJson.ConvertJTokenToDictionary (master.staticData["operatorTalks"]);
		operatorId = user.tUser ["operatorId"].ToString ();
		
		new Task (loadAssetBundle.DownloadLive2DOperator (("Live2D" + operatorId.Substring (8)).ToLower (), transform, (Live2DElements elements) => {
			mocFile = elements.mocFile;
			physicsFile = elements.physicsFile;
			textureFiles = elements.textureFiles;
			mtnFiles = elements.mtnFiles;
			voice = elements.voice;
			foreach (AudioSource so in voice) {
				so.outputAudioMixerGroup = audioMixer.FindMatchingGroups("VOICE")[0];
			}
			idleMtnFile = elements.idleMtnFile;

			Live2D.init ();
			LoadModel ();

			isSetup = true;
		}));
	}

	void OnLevelWasLoaded (int level) {
		if (!isSetup) return;
		StopAnimation ();
	}
	
	void OnApplicationFocus(bool focusStatus) {
		if (focusStatus) {
			OnEnable ();
		} else {
			OnDisable ();
		}
	}

	void OnEnable () {
		if (!isSetup) return;
		isEnabled = true;
		elapsed = 0f;
	}

	void OnDisable () {
		if (!isSetup) return;
		isEnabled = false;
		StopAnimation ();
	}
	
	void OnDestroy () {
		if (!isSetup) return;
		
		DestroyImmediate (live2DElementsPrefab);
	}

    void Update () {
		if (!isSetup) return;
		if (!isEnabled) return;

		elapsed += Time.deltaTime;

		if (elapsed > 10f && noReply == true && pushMessage == true) {
			ShowOperatorTalk (operatorId, "random");
		}

		if (noReply == false) {
			switch (Application.loadedLevelName) {
			case "CardRegistrationView":
			case "PracticeView":
				if (elapsed > 10f) {
					ShowOperatorTalk (operatorId, "still", Application.loadedLevelName);
					noReply = true;
				}
				break;

			case "HomeView":
			case "MultiplayView":
			case "CardPurchaseView":
			default:
				noReply = true;
				break;
				
			}
		}

        var pos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) {
            //
        } else if (Input.GetMouseButton(0)) {
			if (Screen.width / 2 > pos.x) {
				dragMgr.Set(pos.x / Screen.width * 2 - 0.5f, pos.y / Screen.height * 2 - 1);
			}
        } else if (Input.GetMouseButtonUp(0)) {
            dragMgr.Set(0, 0);
        }
    }
	
	void OnRenderObject () {
		if (!isSetup) return;
		
		live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvasPos);

		dragMgr.update();
		live2DModel.setParamFloat("PARAM_ANGLE_X", dragMgr.getX() * 50);
		live2DModel.setParamFloat("PARAM_ANGLE_Y", dragMgr.getY() * 30);

        live2DModel.setParamFloat("PARAM_BODY_ANGLE_X", dragMgr.getX() * 30);

        live2DModel.setParamFloat("PARAM_EYE_BALL_X", -dragMgr.getX());
        live2DModel.setParamFloat("PARAM_EYE_BALL_Y", -dragMgr.getY());

        double timeSec = UtSystem.getUserTimeMSec() / 1000.0;
        double t = timeSec * 2 * Math.PI;
        live2DModel.setParamFloat("PARAM_BREATH", (float)(0.5f + 0.5f * Math.Sin(t / 3.0)));

        eyeBlink.setParam(live2DModel);

        if (physics != null) physics.updateParam(live2DModel);

		live2DModel.update();
		live2DModel.draw();
		
		if (AnimationStartFlg == true) {
			motionManager.updateParam (live2DModel);
		}
	}

	public void ResetLoadLevelFirst () {
		if (!isSetup) return;
		isLoadLevelFirst = true;
		StopAnimation ();
		elapsed = 0f;
		new Task (FirstMessage ());
	}
	
	public void HeadTouch() {
		if (pushMessage == true) {
			ShowOperatorTalk (operatorId, "headTouch");
		}
	}

	public void BodyTouch() {
		if (pushMessage == true) {
			ShowOperatorTalk (operatorId, "bodyTouch");
		}
	}

	public void FaceTouch() {
		if (pushMessage == true) {
			ShowOperatorTalk (operatorId, "faceTouch");
		}
	}

	public void BastTouch() {
		if (pushMessage == true) {
			ShowOperatorTalk (operatorId, "bustTouch");
		}
	}

	private void LoadModel () {
		live2DModel = Live2DModelUnity.loadModel(mocFile.bytes);
		
		live2DModel.setRenderMode(Live2D.L2D_RENDER_DRAW_MESH);
		
		live2DModel.setLayer (8);
		
		for (int i = 0; i < textureFiles.Length; i++) {
			live2DModel.setTexture(i, textureFiles[i]);
		}
		
		float modelWidth = live2DModel.getCanvasWidth();
		live2DCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50.0f, 50.0f);
		
		if (physicsFile != null) physics = L2DPhysics.load(physicsFile.bytes);
	}

	private IEnumerator FirstMessage () {
		isLoadLevelFirst = false;
		pushMessage = false;
		yield return new WaitForSeconds (2.0f);
		elapsed = 0f;
		if (Application.loadedLevelName == "HomeView") {
			DateTime dt = DateTime.Now;
			int currentHour = dt.Hour;
			if (currentHour >= 6 && currentHour < 12) {
				timeZone = "morning";
			} else if (currentHour >= 12 && currentHour < 18) {
				timeZone = "afternoon";
			} else {
				timeZone = "night";
			}

			ShowOperatorTalk (operatorId, "login", "", timeZone);
		} else {
			ShowOperatorTalk (operatorId, "load", Application.loadedLevelName);
		}
	}

	private void Live2DAnimationCall (string say, int animIndex) {
		live2DMessage.text = say;
		motion = Live2DMotion.loadMotion( mtnFiles[ animIndex ].bytes );
		voice [animIndex].Play ();
		
		motionManager = new MotionQueueManager();

		motionManager.startMotion(motion, false);
	}
	
	private IEnumerator StartAnimation () {
		messageWindow.DOFade (1, 0.4f);
		live2DMessage.DOFade (1, 0.4f);
		AnimationStartFlg = true;
		elapsed = 0f;
		yield return new WaitForSeconds (5.0f);
		StopAnimation ();
	}
	
	private void StopAnimation () {
		if (currentAnimIndex > -1) {
			voice [currentAnimIndex].Stop ();
		}
		messageWindow.DOFade (0, 0.4f);
		live2DMessage.DOFade (0, 0.4f);
		pushMessage = true;
		live2DMessage.text = "";
		motion = Live2DMotion.loadMotion( idleMtnFile.bytes );
		motionManager = new MotionQueueManager();
		motionManager.startMotion(motion, false);
	}

	private List<object> GetOperatorTalks (string operatorId, string situation, string sceneName = "", string timeZone = "") {
		List<object> tmpOperatorTalks = new List<object> ();
		foreach (var kv in operatorTalks) {
			var operatorTalk = kv.Value as Dictionary<string, object>;
			if (operatorTalk["operatorId"].ToString () == operatorId && operatorTalk["situation"].ToString () == situation) {
				if (sceneName.Length > 0 && operatorTalk["sceneName"].ToString () != sceneName) continue;
				if (timeZone.Length > 0 && operatorTalk["timeZone"].ToString () != timeZone) continue;
				tmpOperatorTalks.Add (kv.Value);
			}
		}

		return tmpOperatorTalks;
	}
	
	private Dictionary<string, object> GetOperatorTalk (string operatorId, string situation, string sceneName = "", string timeZone = "") {
		List<object> operatorTalks = GetOperatorTalks (operatorId, situation, sceneName, timeZone);
		if (operatorTalks.Count == 0) return null;
		return (Dictionary<string, object>)operatorTalks[UnityEngine.Random.Range(0, operatorTalks.Count)];
	}

	private void ShowOperatorTalk (string operatorId, string situation, string sceneName = "", string timeZone = "") {
		pushMessage = false;
		Dictionary<string, object> tmpOperatorTalk = GetOperatorTalk (operatorId, situation, sceneName, timeZone);
		if (tmpOperatorTalk != null) {
			if (tmpOperatorTalk.ContainsKey ("voiceIndex") && tmpOperatorTalk ["voiceIndex"].ToString ().Length > 0) {
				currentAnimIndex = int.Parse (tmpOperatorTalk ["voiceIndex"].ToString ());
				Live2DAnimationCall (tmpOperatorTalk ["talk"].ToString ().Replace("<br />", "\n"), currentAnimIndex);
			} else {
				currentAnimIndex = -1;
				live2DMessage.text = tmpOperatorTalk ["talk"].ToString ().Replace("<br />", "\n");
			}
			new Task (StartAnimation ());
		}
	}
}
