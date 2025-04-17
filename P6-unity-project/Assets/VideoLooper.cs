using UnityEngine;
using UnityEngine.Video;

public class VideoLooper : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public double loopStartTime = 5.0; // Start looping from here once video ends

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayer.time = loopStartTime;
        videoPlayer.Play();
    }
}
