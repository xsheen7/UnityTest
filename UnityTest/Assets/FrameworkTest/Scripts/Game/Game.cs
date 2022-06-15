using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;

public class Game : UnitySingleton<Game> {

    public override void Awake() {
        base.Awake();
    }

    public void EnterGame() {
        this.StartCoroutine(this.enter_login_scene());
    }

    IEnumerator enter_login_scene() {
        var loader1 = AssetBundleManager.Instance.LoadAssetBundleAsync("gui.assetbundle");
        var loader2 = AssetBundleManager.Instance.LoadAssetBundleAsync("sounds.assetbundle");
        var loader3 = AssetBundleManager.Instance.LoadAssetBundleAsync("excels.assetbundle");
        var loader4 = AssetBundleManager.Instance.LoadAssetBundleAsync("maps.assetbundle");
        yield return loader1;
        yield return loader2;
        yield return loader3;
        yield return loader4;

        loader1.Dispose();
        loader2.Dispose();
        loader3.Dispose();
        loader4.Dispose();

        UI_manager.Instance.ShowUIView("ChooseUI");
        SoundMgr.Instance.play_music("Sounds/bgm_scene1.ogg");

        BulletConfig config = GameConfigDataBase.GetConfigData<BulletConfig>("1", "BulletConfig");
        Debug.Log(config.bulletName + " " + config.coinCost);

        GameObject prefab = AssetBundleManager.Instance.GetAssetCache("Maps/Level1/Race.prefab") as GameObject;
        GameObject.Instantiate(prefab);
        yield break;
    }

	void Update () {
		
	}
}
