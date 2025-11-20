using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoSceneLoader : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;         // Assign in Inspector
    public string nextSceneName;            // Name of scene to load after video ends

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer not assigned in the Inspector!");
            return;
        }

        // Play the video
        videoPlayer.Play();

        // Subscribe to event when video finishes
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Load the next scene after the video ends
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoEnd;
    }
}
