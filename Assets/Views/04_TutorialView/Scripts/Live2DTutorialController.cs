using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using live2d;
using live2d.framework;
using DG.Tweening;

[ExecuteInEditMode]
public class Live2DTutorialController : MonoBehaviour {
	public Text live2dMessage;

	public GameObject live2DModelObj;
	public GameObject title, loadingText;
	public Text titleText, pageText;
	public GameObject arrow;

	private String userName;

	public TextAsset mocFile;
	public TextAsset physicsFile;
	public Texture2D[] textureFiles;
	public TextAsset[] mtnFiles = new TextAsset[4];
	public AudioSource[] voice = new AudioSource[4];

	public Image messageWindow;

	private Live2DMotion motion;
	private MotionQueueManager motionManager;
	
	private Live2DModelUnity live2DModel;
    private EyeBlinkMotion eyeBlink = new EyeBlinkMotion();
    private L2DPhysics physics;
    private Matrix4x4 live2DCanvasPos;

	private bool isSetup = false; 
	private bool noReply;
	private bool AnimationStartFlg;

	private bool pushMessage;


	//-----------------------------------------------------------------------
	[SerializeField] Text soundNovelText;
	
	[SerializeField][Range(0.001f, 0.3f)]

	private int currentLine = 0;
	private string currentText = string.Empty;	// 現在の文字列
	private float timeUntilDisplay = 0;		// 表示にかかる時間
	private float timeElapsed = 1;			// 文字列の表示を開始した時間
	private int lastUpdateCharacter = -1;		// 表示中の文字数
	//------------------------------------------------------------------------

	private Mage mage { get { return Mage.Instance; }}
	private User user { get { return User.Instance; }}
	private Logger logger { get { return mage.logger("TitleView"); } }

	private LoadAssetBundle loadAssetBundle { get { return LoadAssetBundle.Instance; } }
	
	void Start () {
		userName = user.tUser ["userName"].ToString ();

		Live2D.init ();
		load ();

		isSetup = true;

		SetNextLine ();
		StartCoroutine ("FirstMessage");
	}

    void load () {
        live2DModel = Live2DModelUnity.loadModel(mocFile.bytes);

		live2DModel.setRenderMode(Live2D.L2D_RENDER_DRAW_MESH);

		live2DModel.setLayer (8);

        for (int i = 0; i < textureFiles.Length; i++)
        {
            live2DModel.setTexture(i, textureFiles[i]);
        }

        float modelWidth = live2DModel.getCanvasWidth();
        live2DCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50.0f, 50.0f);

        if (physicsFile != null) physics = L2DPhysics.load(physicsFile.bytes);
    }
    

	void Update() {
		if (!isSetup) return;

		if(currentLine < live2dMessage.text.Length && this.pushMessage == false) {
			SetNextLine();
		}
		
		int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * currentText.Length);
		
		// 表示文字数が前回の表示文字数と異なるならテキストを更新する
		if( displayCharacterCount != lastUpdateCharacter ) {
			this.soundNovelText.text = this.live2dMessage.text;
			soundNovelText.text = currentText.Substring(0, displayCharacterCount);
			lastUpdateCharacter = displayCharacterCount;
		}
    }
	
	void OnRenderObject() {
		if (!isSetup) return;

		if (live2DModel == null) {
            load();
        }

        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvasPos);


        if ( ! Application.isPlaying) {
            live2DModel.update();
            live2DModel.draw();
            return;
        }

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

	public void Live2DAnimationCall(string say, int animIndex) {
		this.live2dMessage.text = say;
		motion = Live2DMotion.loadMotion( mtnFiles[ animIndex ].bytes );
		this.voice [animIndex].Play ();
		// モーション管理クラスのインスタンスの作成
		motionManager = new MotionQueueManager();
		// モーションの再生
		motionManager.startMotion( motion, false );
	}

	IEnumerator FirstMessage() {
		yield return new WaitForSeconds (0.5f);
		this.messageWindow.DOFade (1, 0.4f);
		this.soundNovelText.DOFade (1, 0.4f);
		this.live2dMessage.text = "ようこそ。ガンダムクロスウォーへ。";
		this.loadingText.GetComponent<CanvasGroup> ().DOFade (1, 0.4f);
		motionManager = new MotionQueueManager();

		this.TitleIn();
		this.titleText.text = "ようこそガンダムクロスウォーへ";

		arrow.SetActive (true);

		this.AnimationStartFlg = true;
	}

	void WindowAlphaFade(float fade) {
		this.messageWindow.DOFade (fade, 0.4f);
		this.soundNovelText.DOFade (fade, 0.4f);
		this.pushMessage = true;
		this.currentLine = 0;
	}

	void SetNextLine() {
		currentText = live2dMessage.text;
		currentLine ++;
		
		// 想定表示時間と現在の時刻をキャッシュ
//		timeUntilDisplay = currentText.Length * intervalForCharacterDisplay;
//		timeElapsed = Time.time;
		
		// 文字カウントを初期化
		lastUpdateCharacter = -1;
	}

	//Tutorial---------------------------------------------------------------------------------------

	// Tutorial End
	public void SaylaTalkSwitch2(int saylaTalk) {
		this.AnimationStartFlg = true;
		this.pushMessage = false;
		
		//reset
		currentLine = 0;
		this.soundNovelText.text = "";
		
		switch (saylaTalk) {
		case 0:
			this.live2dMessage.text = "おつかれさまでした。";
			break;
		case 1:
			this.live2dMessage.text = "以上が、ガンダムクロスウォーの基本的なルールとカード説明です。";
			break;
		case 2:
			this.live2dMessage.text = "実際の戦闘では、開始前にコイントスをして先攻・後攻を決めます。";
			break;
		case 3:
			this.live2dMessage.text = "先攻はカードを3枚、後攻は5枚ドローして開始よ。\nそのとき、一度だけ手札からいらないカードを選んで引き直すことができるわ。";
			break;
		case 4:
			this.live2dMessage.text = "チュートリアル報酬として、デジタルカードを入手できる「コイン」と、\n対戦するために必要な「BP」を回復する「補給物資」をプレゼントするわ。";
			break;
		case 5:
			this.live2dMessage.text = "「BP」は「補給物資」がなくても毎日回復するから安心して。";
			break;
		case 6:
			this.live2dMessage.text = "すでにリアルカードを持っていたら、すぐにアプリに登録しましょう。\n登録したカードは、アプリ内で使えるようになるわ。";
			break;
		case 7:
			this.live2dMessage.text = "リアルカードは、全国の家電量販店、コンビニ、カードショップ、ECサイト、カードダス自販機で販売中よ。";
			break;
		case 8:
			this.live2dMessage.text = "それでは、最後にファーストデッキを選択してください。";
			break;
		case 9:
			this.live2dMessage.text = "5つの勢力から好みのデッキを選んでよくてよ。";
			break;
		case 10:
			this.live2dMessage.text = "デッキは変更できませんが、勢力は後から変更ができます。";
			break;
		case 11:
			this.live2dMessage.text = "デッキ戦闘力の確認や練習は、訓練モードを活用するといいわ。";
			break;
		case 12:
			this.Live2DAnimationCall ("では、"+userName+"さん。頑張ってください。", 2);
			this.TitleOut();
			break;
		}
	}

	// Tutorial Intro
	public void SaylaTalkSwitch(int saylaTalk) {
		this.AnimationStartFlg = true;
		this.pushMessage = false;

		//reset
		currentLine = 0;
		this.soundNovelText.text = "";
		
		switch (saylaTalk) {
		case 0:
			this.live2dMessage.text = "ようこそ。ガンダムクロスウォーへ。";
			break;
			
		case 1:
			this.Live2DAnimationCall ("担当のセイラ・マスです。", 0);
			break;
			
		case 2:
			this.live2dMessage.text = "このチュートリアルでは、基本的なゲームの操作やルール、カード効果などを解説します。";
			break;
			
		case 3:
			this.Live2DAnimationCall ("大丈夫、あなたならできるわ。", 2);
			break;
			
		case 4:
			this.live2dMessage.text = "ガンダムクロスウォーは2人で対戦するカードゲームよ。";
			break;
			
		case 5:
			this.live2dMessage.text = "あなたには艦長としてモビルスーツ部隊を指揮してもらうわ。";
			break;
			
		case 6:
			this.live2dMessage.text = "対戦でつかうカードは全部で60枚。40枚のMSデッキと20枚のキャラデッキに分かれているの。デッキというのはカードの束のことね。ご存知かしら。";
			break;

		case 7:
			this.live2dMessage.text = "それではさっそく、チュートリアルをはじめましょう。";
			break;
			
		case 8:
			this.live2dMessage.text = "先攻は手札を3枚、後攻は手札を5枚、\n「MSデッキ」からドローしてゲーム開始よ。";
			break;
			
		case 9:
			this.live2dMessage.text = "一度だけ、手札からいらないカードを選んで引き直すことができるわ。";
			break;

		case 10:
			this.live2dMessage.text = "ここまでは大丈夫かしら？";
			break;

		case 11:
			this.Live2DAnimationCall ("ありがと。", 4);
			break;

		case 12:
			this.live2dMessage.text = "次にこのゲームの勝利条件を説明するわ。";
			this.TitleIn();
			this.titleText.text = "勝利条件について";
			this.pageText.text = "2/9";
			break;

		case 13:
			this.Live2DAnimationCall ("あちらをご覧になって。", 5);
			break;
			
		case 14:
			this.live2dMessage.text = "相手の母艦が見えるかしら？\n母艦には耐久値が30あるわ。";
			break;

		case 15:
			this.live2dMessage.text = "手札から「モビルスーツ」や「モビルアーマー」などのユニットを出撃させて、\n相手の母艦を攻撃すると、耐久値にダメージを与えられるわ。";
			break;

		case 16:
			this.live2dMessage.text = "相手の母艦の耐久値を先に０にしたプレイヤーの勝利よ。";
			break;

		case 17:
			this.Live2DAnimationCall ("あなたならできるわ。", 2);
			break;
			
		case 18:
			this.live2dMessage.text = "ここからは操作方法を自分のターンの流れにそって簡単に説明するわね。\nよろしいかしら？";
			this.TitleIn();
			this.titleText.text = "操作方法について";
			this.pageText.text = "3/9";
			break;

		case 19:
			this.Live2DAnimationCall ("ありがと。", 4);
			break;
			
		case 20:
			this.Live2DAnimationCall ("自分のターンは、４つのフェイズで進むわ。", 5);
			break;
			
		case 21:
			this.live2dMessage.text = "まずはドローフェイズ。\n「MSデッキ」の上からカードを１枚、手札にくわえるわ。\nカードゲームでは手札を引くことを「ドロー」って呼ぶわ。";
			this.TitleIn();
			this.titleText.text = "ドローフェイズ";
			this.pageText.text = "4/9";
			break;
			
		case 22:
			this.live2dMessage.text = "次にコストフェイズ。\n「キャラデッキ」の上からカードを1枚、オープンしてブリッジに登場させるわ。\nドローフェイズ、コストフェイズは自動で進行するのよ。";
			this.TitleIn();
			this.titleText.text = "コストフェイズ";
			this.pageText.text = "5/9";
			break;
			
		case 23:
			this.Live2DAnimationCall ("そしてメインフェイズ。\nここからは、あなたが画面を操作して、\n手札からカードを使用したり、出撃しているユニットで攻撃を行うのよ。", 1);
			this.TitleIn();
			this.titleText.text = "メインフェイズ";
			this.pageText.text = "6/9";
			break;
			
		case 24:
			this.live2dMessage.text = "手札からカードを使用するときは、\n画面右下に表示されている手札をタッチしましょう。";
			break;
			
		case 25:
			this.Live2DAnimationCall ("さらにカードをタップすると、そのカードの詳細をみることができるわ。", 5);
			break;
			
		case 26:
			this.live2dMessage.text = "カードを使用するには、手札からカードを上にフリックしましょう。";
			break;
			
		case 27:
			this.live2dMessage.text = "次に、カードを使用するために必要なコストの選択よ。";
			break;
			
		case 28:
			this.Live2DAnimationCall ("カードにはそれぞれ使用するために必要なコストが存在します。", 5);
			break;
			
		case 29:
			this.Live2DAnimationCall ("必要なコスト分、ブリッジのキャラクターをタッチしてコストを選択しましょう。", 1);
			break;
			
		case 30:
			this.live2dMessage.text = "コストが満たされたらカードを使用できるわ。\nカードを上にフリックしましょう。";
			break;
			
		case 31:
			this.live2dMessage.text = "使用するカードが「ユニットカード」の場合は出撃させる位置を選べます。\n出撃させたい場所まで「ユニットアイコン」をフリックさせましょう。";
			break;

		case 32:
			this.Live2DAnimationCall ("あなたならできるわ。", 2);
			break;
			
		case 33:
			this.live2dMessage.text = "ブリッジに未行動のパイロットがいる場合は、\nユニットの出撃時にそのパイロットを搭乗させることができます。";
			this.TitleIn();
			this.titleText.text = "メインフェイズ　パイロットの搭乗について";
			this.pageText.text = "7/9";
			break;
			
		case 34:
			this.live2dMessage.text = "ブリッジからパイロットをフリックして搭乗させましょう。";
			break;
			
		case 35:
			this.live2dMessage.text = "では最後に、ユニットの攻撃について説明するわね。\nあと少しよ。頑張りましょう。";
			this.TitleIn();
			this.titleText.text = "メインフェイズ　攻撃について";
			this.pageText.text = "8/9";
			break;

		case 36:
			this.Live2DAnimationCall ("ありがと。", 4);
			break;

		case 37:
			this.Live2DAnimationCall ("あなたならできるわ。", 2);
			break;

		case 38:
			this.live2dMessage.text = "出撃させたユニットは、ユニットアイコンとして表示されるわ。\n母艦を守る効果【防衛】を持つユニットアイコンは形が変わります。";
			break;
			
		case 39:
			this.Live2DAnimationCall ("出撃させたユニットは、すぐに攻撃ができるわ。\nユニットをフリックして、攻撃対象を選択しましょう。\nただし、先攻の最初のターンだけは攻撃ができないの。覚えておいてね。", 1);
			break;
			
		case 40:
			this.live2dMessage.text = "一部のユニットは特別な効果を所有しているわ。\nこれらの効果を使用する際は、ユニットにタッチしたまま\n表示された効果をフリックで選択しましょう。";
			break;
			
		case 41:
			this.live2dMessage.text = "メインフェイズでこれ以上やることがなくなったり、\n自分のターンを終わらせたいときは「END TURN」をタッチよ。";
			this.TitleIn();
			this.titleText.text = "エンドフェイズについて";
			this.pageText.text = "9/9";
			break;
			
		case 42:
			this.Live2DAnimationCall ("これでチュートリアルは終了よ。\n"+userName+"さんには、「初期デッキ」をプレゼントするわ。", 1);
			this.TitleIn();
			this.titleText.text = "「初期デッキ」プレゼント！";
			this.pageText.color = new Color(1,1,1,0);
			break;
			
		case 43:
			this.live2dMessage.text = "デジタルカードを入手できる「コイン」と\n対戦するために必要な「BP」を回復する「補給物資」もプレゼントするわ。";
			this.TitleIn();
			this.titleText.text = "「コイン」「補給物資」プレゼント！";
			break;
			
		case 44:
			this.live2dMessage.text = "「BP」は「補給物資」がなくても毎日回復するから安心してね。";
			this.TitleIn();
			this.titleText.text = "「BP」の回復について";
			break;
			
		case 45:
			this.live2dMessage.text = "すでにリアルカードを持っていたら、すぐにアプリに登録しましょう。\nそのカードがアプリ内でも使用できるようになるわ。\nさらに「補給物資」も入手できるわ。";
			this.TitleIn();
			this.titleText.text = "リアルカードの登録について";
			break;
			
		case 46:
			this.live2dMessage.text = "リアルカードは全国の家電量販店、コンビニ、カードショップ、ECサイト、\nカードダス自販機で販売中よ。";
			this.TitleIn();
			this.titleText.text = "リアルカードの販売について";
			break;
			
		case 47:
			this.live2dMessage.text = "カードゲームの詳細ルールは公式WEBをチェックしてね！\nバトルガイド動画がオススメよ。";
			this.TitleIn();
			this.titleText.text = "カードゲームのルールは公式WEBをチェック！";
			break;
			
		case 48:
			this.live2dMessage.text = "最後に、あなたの所属する「勢力」を選択しましょう。\n「一ヶ月戦争」モードでは、あなたが選んだ勢力で参戦することになるわ。\n選んだ勢力は、「入隊証」からいつでも変更可能よ。";
			this.TitleIn();
			this.titleText.text = "所属勢力の選択";
			break;

		case 49:
			this.live2dMessage.text = "では、"+userName+"さん。";
			this.TitleOut();
			break;
			
		case 50:
			this.Live2DAnimationCall ("頑張ってね。", 3);
			this.TitleOut();
			break;
		}

	}

	public void live2DPositionLeft() {
		live2DModelObj.transform.DOLocalMove(new Vector3(-244, -9, 0), 0.4f).SetEase (Ease.OutCubic);
	}

	public void live2DPositionCenter() {
		live2DModelObj.transform.DOLocalMove(new Vector3(0, -9, 0), 0.4f).SetEase (Ease.OutCubic);
	}
		
	public void live2DIn() {
		live2DPositionLeft ();
		title.GetComponent<CanvasGroup>().DOFade(1, 0.4f);
		WindowAlphaFade (1);
		arrow.SetActive (true);
	}
		
	public void live2DOut () {
		live2DModelObj.transform.DOLocalMove(new Vector3(-610, -9, 0), 0.5f).SetEase (Ease.OutCubic);
		title.GetComponent<CanvasGroup>().DOFade(0, 0.4f);
		WindowAlphaFade (0);
		arrow.SetActive (false);
	}

	public void arrowOFF() {
		arrow.SetActive(false);
	}


	//タイトル関係アニメ------------------------------------------------

	void TitleIn() {
		title.transform.localPosition = new Vector3 (-550, 205, 0);
		title.GetComponent<CanvasGroup> ().alpha = 0;
		title.transform.DOLocalMove (new Vector3 (-364, 205, 0), 0.4f).SetEase (Ease.OutCubic);
		title.GetComponent<CanvasGroup> ().DOFade (1, 0.4f).SetEase (Ease.InCubic);
	}

	void TitleOut() {
		title.transform.DOLocalMove (new Vector3 (-550, 205, 0), 0.3f);
		title.GetComponent<CanvasGroup> ().DOFade (0, 0.3f);
	}

}
