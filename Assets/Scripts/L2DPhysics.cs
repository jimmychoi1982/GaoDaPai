/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using System.Collections;
using System.Collections.Generic;
using live2d ;

namespace live2d.framework {
    /*
     * 物理演算のアニメーション。
     *
     */
    public class L2DPhysics {

        private List<PhysicsHair> physicsList;
        private long startTimeMSec;


        public L2DPhysics() {
            physicsList = new List<PhysicsHair>();
            startTimeMSec = UtSystem.getUserTimeMSec();
        }


        void addParam(PhysicsHair phisics) {
            physicsList.Add(phisics);
        }


        /*
         * モデルのパラメータを更新。
         * @param model
         */
        public void updateParam(ALive2DModel model) {
            long timeMSec = UtSystem.getUserTimeMSec() - startTimeMSec;
            for (int i = 0; i < physicsList.Count; i++) {
                physicsList[i].update(model, timeMSec);
            }
        }

        public static L2DPhysics load(byte[] buf) {
            return load(System.Text.Encoding.GetEncoding("UTF-8").GetString(buf));
        }


        public static L2DPhysics load(string buf) {
            return load(buf.ToCharArray());
        }

        /*
         * JSONファイルから読み込み
         * 仕様についてはマニュアル参照。JSONスキーマの形式の仕様がある。
         * @param buf
         * @return
         */
        public static L2DPhysics load(char[] buf) {

            L2DPhysics ret = new L2DPhysics();

            Value json = Json.parseFromBytes(buf);

            // 物理演算一覧
            List<Value> params_ = json.get("physics_hair").getVector(null);
            int paramNum = params_.Count;

            for (int i = 0; i < paramNum; i++) {
                Value param = params_[i];

                PhysicsHair physics = new PhysicsHair();
                // 計算の設定
                Value setup = param.get("setup");
                // 長さ
                float length = setup.get("length").toFloat();
                // 空気抵抗
                float resist = setup.get("regist").toFloat();
                // 質量
                float mass = setup.get("mass").toFloat();
                physics.setup(length, resist, mass);

                // 元パラメータの設定
                List<Value> srcList = param.get("src").getVector(null);
                int srcNum = srcList.Count;
                for (int j = 0; j < srcNum; j++) {
                    Value src = srcList[j];
                    string id = src.get("id").toString();//param ID
                    PhysicsHair.Src type = PhysicsHair.Src.SRC_TO_X;
                    string typeStr = src.get("ptype").toString();
                    if (typeStr == "x") {
                        type = PhysicsHair.Src.SRC_TO_X;
                    } else if (typeStr == "y") {
                        type = PhysicsHair.Src.SRC_TO_Y;
                    } else if (typeStr == "angle") {
                        type = PhysicsHair.Src.SRC_TO_G_ANGLE;
                    } else {
                        UtDebug.error("live2d", "Invalid parameter:hysicsHair.Src");
                    }

                    float scale = src.get("scale").toFloat();
                    float weight = src.get("weight").toFloat();
                    physics.addSrcParam(type, id, scale, weight);
                }

                // 対象パラメータの設定
                List<Value> targetList = param.get("targets").getVector(null);
                int targetNum = targetList.Count;
                for (int j = 0; j < targetNum; j++) {
                    Value target = targetList[j];
                    string id = target.get("id").toString();//param ID
                    PhysicsHair.Target type = PhysicsHair.Target.TARGET_FROM_ANGLE;
                    string typeStr = target.get("ptype").toString();
                    if (typeStr == "angle") {
                        type = PhysicsHair.Target.TARGET_FROM_ANGLE;
                    } else if (typeStr == "angle_v") {
                        type = PhysicsHair.Target.TARGET_FROM_ANGLE_V;
                    } else {
                        UtDebug.error("live2d", "Invalid parameter:PhysicsHair.Target");
                    }

                    float scale = target.get("scale").toFloat();
                    float weight = target.get("weight").toFloat();
                    physics.addTargetParam(type, id, scale, weight);

                }
                ret.addParam(physics);
            }

            return ret;
        }
    }
}