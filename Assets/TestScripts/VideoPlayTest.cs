using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Enhance.Unity;
using Cysharp.Threading.Tasks;
using System;
using FFmpeg.AutoGen;

public class VideoPlayTest : MonoBehaviour
{
    public VideoPlayerPlus player;
    public string urlPath;
    // Start is called before the first frame update
    private void Start()
    {
		player.Load(urlPath);
        UniTask.Create(async () =>
        {
            await UniTask.DelayFrame(10);
            await player.PlayPlusAsync();
		});
    }
}
