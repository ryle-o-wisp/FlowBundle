using System;
using System.Collections;
using System.Collections.Generic;
using BundleSystem;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    [Header("Test"), Tooltip("Test Tooltip")]
    public BundledGameObjectReference prefab;

    [Serializable]
    public class ListEntry
    {
        [Header("Test"), Tooltip("Test Tooltip")]
        public BundledGameObjectReference prefab;

        public GameObject directRef;
    }

    public ListEntry[] prefabs;
    
    public ListEntry t;

    public BundledSpriteReference sprite;
    
    
    [Serializable]
    public class SpriteListEntry
    {
        public BundledSpriteReference sprite;

        public Sprite directRef;
    }

    public SpriteListEntry[] spriteListEntry;
    public SpriteListEntry singleSpriteEntry;

    IEnumerator Start()
    {
        yield return BundleManager.Initialize();
        
        //get download size from latest bundle manifest
        var manifestReq = BundleManager.GetManifest();
        yield return manifestReq;
        if (!manifestReq.Succeeded)
        {
            //handle error
            Debug.LogError(manifestReq.ErrorCode);
        }

        Debug.Log($"Need to download { BundleManager.GetDownloadSize(manifestReq.Result) * 0.000001f } mb");

        //start downloading
        var downloadReq = BundleManager.DownloadAssetBundles(manifestReq.Result);
        while(!downloadReq.IsDone)
        {
            if(downloadReq.CurrentCount >= 0)
            {
                Debug.Log($"Current File {downloadReq.CurrentCount}/{downloadReq.TotalCount}, " +
                          $"Progress : {downloadReq.Progress * 100}%, " +
                          $"FromCache {downloadReq.CurrentlyLoadingFromCache}");
            }

            yield return null;
        }
        
        if(!downloadReq.Succeeded)
        {
            //handle error
            Debug.LogError(downloadReq.ErrorCode);
        }
        
        Debug.Assert(prefab.LoadSync().Asset != null);
        var prefabLoad = prefab.LoadAsync();
        yield return prefabLoad;
        Debug.Assert(prefabLoad.Asset != null);
        Debug.Log($@"Loaded Prefab: {prefabLoad.Asset.name}");
        
        Debug.Assert(sprite.LoadSync().Asset != null);
        var spriteLoad = sprite.LoadAsync();
        yield return spriteLoad;
        Debug.Assert(spriteLoad.Asset != null);
        Debug.Log($@"Loaded Sprite: {spriteLoad.Asset.name}");
    }
}
