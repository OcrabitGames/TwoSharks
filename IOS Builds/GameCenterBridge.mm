//
//  GameCenterBridge.mm
//  Unity-iPhone
//
//  Created by Marco Cassar on 3/24/25.
//

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "UnityAppController.h"
#import "UnityInterface.h"
#import <UnityFramework/UnityFramework-Swift.h>

extern "C" {
    void GC_Authenticate() {
        [[GameCenterManager shared] authenticatePlayer];
    }

    void GC_SubmitScore(int64_t score, const char* leaderboardID) {
        NSString* lb = [NSString stringWithUTF8String:leaderboardID];
        [[GameCenterManager shared] submitScore:score leaderboardID:lb];
    }

    void GC_ShowLeaderboard(const char* leaderboardID) {
        NSString* lb = [NSString stringWithUTF8String:leaderboardID];
        [[GameCenterManager shared] showLeaderboardWithLeaderboardID:lb];
    }
}
