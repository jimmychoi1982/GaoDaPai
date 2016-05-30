using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using live2d;
using live2d.framework;

[ExecuteInEditMode]
public class SimpleModel : MonoBehaviour {
	public int randomFreeAction;
	public int randomAngryAction;
	public Text live2dMessage;
	public int timeZone;

    public TextAsset mocFile;
    public TextAsset physicsFile;
    public Texture2D[] textureFiles;
	public TextAsset[] mtnFiles;
	public AudioSource[] voice;

	private float timeCount;
	private float timeleft;
	private Live2DMotion motion;
	private MotionQueueManager motionManager;
	
	private Live2DModelUnity live2DModel;
    private EyeBlinkMotion eyeBlink = new EyeBlinkMotion();
    private L2DTargetPoint dragMgr = new L2DTargetPoint();
    private L2DPhysics physics;
    private Matrix4x4 live2DCanvasPos;

	private bool AnimationStartFlg;
	
	void Start () {
		Live2D.init ();
		load ();

		if (Application.loadedLevelName == "HomeView") {
			switch (this.timeZone) {
			case 0:
				this.Live2DAnimationCall ("おはよう、お帰りなさい。\n今日も頑張りましょう。あなたならできるわ。", 3);
				break;

			case 1:
				this.Live2DAnimationCall ("こんにちは、お帰りなさい。\n今日も頑張りましょう。あなたならできるわ。", 3);
				break;

			case 2:
				this.Live2DAnimationCall ("あなたの帰りを待っていわ。\nお気をつけて。発進!どうぞ!", 4);
				break;
			}
		}

		if (Application.loadedLevelName == "PracticeView") {
			this.Live2DAnimationCall ("ここでは訓練用シミュレータを使ってコンピュータと対戦できるわ。 発進!どうぞ!", 4);
		}
		if (Application.loadedLevelName == "MultiplayView") {
			this.Live2DAnimationCall ("[一ヶ月戦争]では所属戦力に分かれて世界中のライバルと競い合います。戦果に応じてあなたの階級が上がり、報酬を手にすることができるわ。自分の勢力勝利に導きましょう。あなたならできるわ。", 3);
		}
		if (Application.loadedLevelName == "CardPurchaseView") {
			this.Live2DAnimationCall ("はい。ここではリアルカードの購入や販売店の確認ができます。" +
				"手に入れたリアルカードは、[カード登録]をすることで" +
				"アプリでも使うことができるわ。", 7);
		}
		if (Application.loadedLevelName == "CardRegistrationView") {
			this.Live2DAnimationCall ("リアルカードのシリアル登録をすることで、アプリでも同じカードが使用できるわ。不要になったカードは登録解除もできてよ。", 12);
		}

		this.AnimationStartFlg = true;
	}

    void load() {
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
    

    void Update() {
		timeleft -= Time.deltaTime;
		
		if (timeleft <= 0.0) {
			timeleft = 1.0f;
			this.timeCount++;
		}

		if (this.timeCount > 10f && Application.loadedLevelName == "HomeView") {
			this.randomFreeAction = UnityEngine.Random.Range (0, 21);
			this.AnimationStartFlg=true;

			switch(this.randomFreeAction) {
			case 0:
				this.Live2DAnimationCall("あなたならできるわ。", 3);
				break;

			case 1:
				this.Live2DAnimationCall("何？", 5);
				break;

			case 2:
				this.Live2DAnimationCall("ありがと。", 9);
				break;

			case 3:
				this.Live2DAnimationCall("かまいません。", 8);
				break;

			case 4:
				this.Live2DAnimationCall("事情は…　いろいろとね…。", 12);
				break;

			case 5:
				this.Live2DAnimationCall("セイラ・マスです。", 10);
				break;

			case 6:
				this.live2dMessage.text = "「砲撃」なら相手のユニットが「防衛」を持っていても、相手の母艦にダメージを与えられるわ。";
				break;
			
			case 7:
				this.live2dMessage.text = "「防衛」を持ったユニットが場にいれば、母艦が直接攻撃を受けることはなくなるわ。有効に活用しましょう。";
				break;

			case 8:
				this.live2dMessage.text = "ユニットが破壊されても、搭乗していたパイロットはブリッジに戻るわ。ブリッジの人数は10人までよ。";
				break;

			case 9:
				this.live2dMessage.text = "デッキは複数保存できるわ。\n色々なデッキを試してもかまわなくてよ。";
				break;

			case 10:
				this.live2dMessage.text = "所属勢力はいつでも変更できるわ。\n変更前の階級は保持されるから、お気になさらず。";
				break;

			case 11:
				this.live2dMessage.text = "キーワード効果の中には、相手のユニットの\nサイズや位置が影響するものもあるわ。";
				break;

			case 12:
				this.live2dMessage.text = "行動済のカードは、自分のターンのエンドフェイズに未行動状態に戻るわ。";
				break;

			case 13:
				this.live2dMessage.text = "キーワード効果は、出撃時、攻撃時など、\n発動タイミングが限定されているものもあるわ。";
				break;

			case 14:
				this.live2dMessage.text = "NCを貯めれば、デジタルカードを手に入れることができるわ。";
				break;

			case 15:
				this.live2dMessage.text = "カウンターカードは何枚でもセットすることができるわ。先に発動するのは、最後にセットしたカードよ。";
				break;

			case 16:
				this.live2dMessage.text = "デッキは色やコストに気をつけて編成しましょう。\n特定の色コストがないと、なかなかユニットが\n出撃できない……、なんてことも……。";
				break;

			case 17:
				this.live2dMessage.text = "Lサイズのユニットカードは場に1体しか\n出撃させることはできないわ。";
				break;

			case 18:
				this.live2dMessage.text = "母艦の耐久値が残っていても、\nMSデッキが0枚になり、ドローが出来なければ、\n負けとなるので注意が必要よ。";
				break;

			case 19:
				this.live2dMessage.text = "手に入れた母艦はデッキ編成画面で\n変更することができるわ。";
				break;

			case 20:
				this.live2dMessage.text = "「装甲」を持っているユニットは、\n効果やダメージで「行動済」にしないと\n破壊できないわ……、かなり強力ね。";
				break;
			}

			this.timeCount=0;
		}

		if (Application.loadedLevelName == "CardRegistrationView") {
			if(this.timeCount > 10f) {
				this.Live2DAnimationCall("事情は…　いろいろとね…。", 12);
				this.timeCount = 0;
			}

			if (Input.GetMouseButtonDown (0) || Input.touchCount == 1) {
				this.timeCount = 0;
			}
		}

		if (Application.loadedLevelName == "PracticeView") {
			if(this.timeCount > 10f) {
				this.Live2DAnimationCall("返事が聞こえないわ。", 6);
				this.timeCount = 0;
			}

			if (Input.GetMouseButtonDown (0) || Input.touchCount == 1) {
				this.timeCount = 0;
			}
		}


        var pos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) {
            //
        }  else if (Input.GetMouseButton(0)) {
            dragMgr.Set(pos.x / Screen.width*2-1, pos.y/Screen.height*2-1);
        } else if (Input.GetMouseButtonUp(0)) {
            dragMgr.Set(0, 0);
        }
    }

	
	void OnRenderObject() {
        if (live2DModel == null) {
            load();
        }

        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvasPos);


        if ( ! Application.isPlaying) {
            live2DModel.update();
            live2DModel.draw();
            return;
        }

        dragMgr.update();
		live2DModel.setParamFloat( "PARAM_ANGLE_X" , dragMgr.getX()*30 ) ;
        live2DModel.setParamFloat("PARAM_ANGLE_Y", dragMgr.getY() * 30);

        live2DModel.setParamFloat("PARAM_BODY_ANGLE_X", dragMgr.getX() * 10);

        live2DModel.setParamFloat("PARAM_EYE_BALL_X", -dragMgr.getX());
        live2DModel.setParamFloat("PARAM_EYE_BALL_Y", -dragMgr.getY());

        double timeSec = UtSystem.getUserTimeMSec() / 1000.0;
        double t = timeSec * 2 * Math.PI;
        live2DModel.setParamFloat("PARAM_BREATH", (float)(0.5f + 0.5f * Math.Sin(t / 3.0)));

        eyeBlink.setParam(live2DModel);

        if (physics != null) physics.updateParam(live2DModel);

		live2DModel.update();
		live2DModel.draw();

		if (this.AnimationStartFlg == true) {
			motionManager.updateParam (live2DModel);
		}
	}

	public void BodyTouch() {
		this.randomAngryAction = UnityEngine.Random.Range (0, 4);

		switch(this.randomAngryAction) {
		case 0:
			this.Live2DAnimationCall("それでも男ですか! 軟弱者!!", 0);
			break;

		case 1:
			this.Live2DAnimationCall("おやめなさい!", 1);
			break;

		case 2:
			this.Live2DAnimationCall("およしなさい!", 2);
			break;

		case 3:
			this.Live2DAnimationCall("あなたは何を考えているの?", 11);
			break;
		}

		this.AnimationStartFlg = true;
	}

	public void Live2DAnimationCall(string say, int animIndex) {
		this.live2dMessage.text = say;
		motion = Live2DMotion.loadMotion( mtnFiles[ animIndex ].bytes );
		this.voice [animIndex].Play ();

		// モーション管理クラスのインスタンスの作成
		motionManager = new MotionQueueManager();
		// モーションの再生
		motionManager.startMotion( motion, false );
	}
}
