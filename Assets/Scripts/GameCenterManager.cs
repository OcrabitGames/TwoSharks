using System.Runtime.InteropServices;
using UnityEngine;

public class GameCenterManager : MonoBehaviour
{
    private static string leaderboardID = "7cWF3D5z2UjX269WM4av";
    
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void GC_Authenticate();

    [DllImport("__Internal")]
    private static extern void GC_SubmitScore(long score, string leaderboardID);

    [DllImport("__Internal")]
    private static extern void GC_ShowLeaderboard(string leaderboardID);
#endif

    public static void Authenticate()
    {
#if UNITY_IOS && !UNITY_EDITOR
        GC_Authenticate();
#else
        Debug.Log("Game Center authenticate only works on iOS device.");
#endif
    }

    public static void SubmitScore(long score)
    {
#if UNITY_IOS && !UNITY_EDITOR
        GC_SubmitScore(score, leaderboardID);
#else
        Debug.Log("Score submission only works on iOS device.");
#endif
    }

    public static void ShowLeaderboard()
    {
#if UNITY_IOS && !UNITY_EDITOR
        GC_ShowLeaderboard(leaderboardID);
#else
        Debug.Log("Leaderboard display only works on iOS device.");
#endif
    }
}