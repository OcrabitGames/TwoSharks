// using UnityEngine;
//
// public class GameCenterManager : MonoBehaviour
// {
//     // Replace with your actual leaderboard ID from App Store Connect.
//     public string leaderboardID = "7cWF3D5z2UjX269WM4av";
//
//     // Called when the script instance is being loaded.
//     void Start()
//     {
//         AuthenticateUser();
//     }
//
//     /// <summary>
//     /// Authenticates the local user with Game Center.
//     /// </summary>
//     public void AuthenticateUser()
//     {
//         // Check if already authenticated
//         if (!GameCenterPlatform.localUser.authenticated)
//         {
//             Social.localUser.Authenticate(success =>
//             {
//                 if (success)
//                 {
//                     Debug.Log("Game Center authentication successful");
//                 }
//                 else
//                 {
//                     Debug.LogError("Game Center authentication failed");
//                 }
//             });
//         }
//         else
//         {
//             Debug.Log("User already authenticated with Game Center");
//         }
//     }
//
//     /// <summary>
//     /// Reports a score to the specified leaderboard.
//     /// </summary>
//     /// <param name="score">The score to report</param>
//     public void ReportScore(long score)
//     {
//         if (Social.localUser.authenticated)
//         {
//             Social.ReportScore(score, leaderboardID, success =>
//             {
//                 if (success)
//                 {
//                     Debug.Log("Score reported successfully");
//                 }
//                 else
//                 {
//                     Debug.LogError("Failed to report score");
//                 }
//             });
//         }
//         else
//         {
//             Debug.LogWarning("User is not authenticated. Cannot report score.");
//         }
//     }
//
//     /// <summary>
//     /// Displays the native Game Center leaderboard UI.
//     /// </summary>
//     public void ShowLeaderboard()
//     {
//         if (Social.localUser.authenticated)
//         {
//             Social.ShowLeaderboardUI();
//         }
//         else
//         {
//             Debug.LogWarning("User is not authenticated. Cannot display leaderboard.");
//         }
//     }
// }
