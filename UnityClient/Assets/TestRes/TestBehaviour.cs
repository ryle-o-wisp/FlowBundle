using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BundleSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        BundleManager.LogMessages = true;
        yield return BundleManager.Initialize();
        
        //get download size from latest bundle manifest
        var manifestReq = BundleManager.GetAllManifests();
        yield return manifestReq;
        if (!manifestReq.Succeeded)
        {
            //handle error
            Debug.LogError(manifestReq.ErrorCode);
        }

        Debug.Log($"Need to download { BundleManager.GetDownloadSize(manifestReq.Result) * 0.000001f } mb");

        var allManifests = manifestReq.Result;
        var manifests = manifestReq.Result.Where(manifest => manifest.DownloadAtInitialTime || BundleManager.IsCached(manifest)).ToArray();

        //start downloading
        var downloadReq = BundleManager.DownloadAssetBundlesInBackground(manifests);
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

        var instance1 = prefab.Instantiate(transform);
        Debug.Assert(instance1 != null);

        var instance2 = prefab.InstantiateAsync(transform);
        Debug.Assert(instance2 != null);
        
        Debug.Assert(sprite.LoadSync().Asset != null);
        var spriteLoad = sprite.LoadAsync();
        yield return spriteLoad;
        Debug.Assert(spriteLoad.Asset != null);
        Debug.Log($@"Loaded Sprite: {spriteLoad.Asset.name}");
        
        {
            var allManagedManifests = BundleManager.AllCachedManifests;
            
            foreach (var manifest in allManagedManifests)
            {
                Debug.Log($@"Manifest {manifest.PackageName}({manifest.PackageGuid}) is {(BundleManager.IsCached(manifest) ? "cached" : "not cached")}");
                if (BundleManager.IsCached(manifest) == false)
                {
                    Debug.Log($@"Started additional download {manifest.PackageName}");
                
                    var additionalDownloadManifests = allManifests.Where(eachManifest => eachManifest.PackageName == manifest.PackageName).ToArray();

                    //start downloading
                    var additionalDownloadReq = BundleManager.DownloadAssetBundlesInBackground(additionalDownloadManifests);
                    while(!additionalDownloadReq.IsDone)
                    {
                        if(additionalDownloadReq.CurrentCount >= 0)
                        {
                            Debug.Log($"Current File {additionalDownloadReq.CurrentCount}/{additionalDownloadReq.TotalCount}, " +
                                      $"Progress : {additionalDownloadReq.Progress * 100}%, " +
                                      $"FromCache {additionalDownloadReq.CurrentlyLoadingFromCache}");
                        }

                        yield return null;
                    }
        
                    if(!additionalDownloadReq.Succeeded)
                    {
                        //handle error
                        Debug.LogError(additionalDownloadReq.ErrorCode);
                    }
                }
            }
        }
        
        BundleManager.LoadScene("Assets/TestRes/testscene2.unity", LoadSceneMode.Single);
    }
}
