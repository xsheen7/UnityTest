﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AssetBundles;
using GameChannel;

public class GameLanch : UnitySingleton<GameLanch> {
    public override void Awake() {
        base.Awake();

        // 初始化游戏框架
        this.gameObject.AddComponent<AssetBundleManager>();
        this.gameObject.AddComponent<SoundMgr>();
        this.gameObject.AddComponent<EventMgr>();
        EventMgr.Instance.init();
        this.gameObject.AddComponent<UI_manager>();

#if RELEASE_BUILD
    #else
        this.gameObject.AddComponent<show_fps>();
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            this.gameObject.AddComponent<DebugManager>();
        }
    #endif
        // end 

        this.gameObject.AddComponent<Game>();
    }

    IEnumerator InitPackageName()
    {
#if UNITY_EDITOR
        if (AssetBundleConfig.IsEditorMode)
        {
            yield break;
        }
#endif
        var packageNameRequest = AssetBundleManager.Instance.RequestAssetFileAsync(BuildUtils.PackageNameFileName);
        yield return packageNameRequest; // 中断当前协程，直到请求结束;
        var packageName = packageNameRequest.text;
        packageNameRequest.Dispose(); // 释放请求;

        AssetBundleManager.ManifestBundleName = packageName; // 包名字;
        ChannelManager.instance.Init(packageName);
        Debug.Log(string.Format("packageName = {0}", packageName));
        yield break;
    }

    IEnumerator CheckAndDownload()
    {
        // 如果已经是最新的，直接返回就可以了;
        // end 

        // 更新的资源包的下载; 直接下载, 最新的ab包;
        // 根据版本，拉取要下载文件列表，然后来一个个下载, 下载完成后直接进入游戏，即可;
        // 检车更新;
        yield break;
    }

    IEnumerator GameStart()
    {

        var start = DateTime.Now;
        yield return InitPackageName();
        Debug.Log(string.Format("InitPackageName use {0}ms", (DateTime.Now - start).Milliseconds));

        // 启动资源管理模块
        start = DateTime.Now;
        yield return AssetBundleManager.Instance.Initialize();
        Debug.Log(string.Format("AssetBundleManager Initialize use {0}ms", (DateTime.Now - start).Milliseconds));

        // 启动检测更新
        // yield return CheckAndDownload();
        // end 

        Game.Instance.EnterGame();

        yield break;
    }

    void Start() {
        this.StartCoroutine(this.GameStart());
    }
}
