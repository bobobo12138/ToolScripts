using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;//记得启用Video包

public class UIVideoPlayer : MonoBehaviour
{

    public bool isAutoStop = false;


    VideoPlayer videoPlayer;
    RawImage rawImage;
    RenderTexture renderTexture;


    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GetComponent<RawImage>();
        renderTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 0);
        videoPlayer.targetTexture = renderTexture;
        rawImage.texture = renderTexture;
        rawImage.SetNativeSize();


        //videoPlayer.Play();
        //videoPlayer.frame = 1;
        //videoPlayer.Pause();


    }

    void Update()
    {
        //if (isAutoStop) videoPlayer.Stop();

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            videoPlayer.Play();
            videoPlayer.frame = 5;
            videoPlayer.Pause();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            videoPlayer.Play();
            videoPlayer.frame = 29;
            videoPlayer.Pause();
        }
    }
}
