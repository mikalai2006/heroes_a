using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class AudioManager : StaticInstance<AudioManager>
{
    private AudioSource _source;
    public AudioSource Source => _source;
    protected override void Awake()
    {
        base.Awake();
        _source = GetComponent<AudioSource>();
    }

    public async UniTask PlayClip(AssetReferenceT<AudioClip> clip)
    {
        await clip.LoadAssetAsync();
        await UniTask.Yield();
        AudioManager.Instance.Source.PlayOneShot((AudioClip)clip.Asset);
        clip.ReleaseAsset();
    }

    public async UniTask Click()
    {
        await AudioManager.Instance.PlayClip(LevelManager.Instance.ConfigGameSettings.AudioGeneral.buttonClick);
    }
}
