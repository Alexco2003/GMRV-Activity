///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Welcome to your first Cloud Script revision!
//
// Cloud Script runs in the PlayFab cloud and has full access to the PlayFab Game Server API 
// (https://api.playfab.com/Documentation/Server), and it runs in the context of a securely
// authenticated player, so you can use it to implement logic for your game that is safe from
// client-side exploits. 
//
// Cloud Script functions can also make web requests to external HTTPS
// endpoints, such as a database or private API for your title, which makes them a flexible
// way to integrate with your existing backend systems.
//
// There are several different options for calling Cloud Script functions:
//
// 1) Your game client calls them directly using the "ExecuteCloudScript" API,
// passing in the function name and arguments in the request and receiving the 
// function return result in the response.
// (https://api.playfab.com/Documentation/Client/method/ExecuteCloudScript)
// 
// 2) You create PlayStream event actions that call them when a particular 
// event occurs, passing in the event and associated player profile data.
// (https://api.playfab.com/playstream/docs)
// 
// 3) For titles using the Photon Add-on (https://playfab.com/marketplace/photon/),
// Photon room events trigger webhooks which call corresponding Cloud Script functions.
// 
// The following examples demonstrate all three options.
//
///////////////////////////////////////////////////////////////////////////////////////////////////////

function sendEmailViaResend(email, subject, bodyHtml) {

    var resendApiKey = "";
    
    if(!email || email === "") {
        log.info("No email address provided.");
        return;
    }

    var headers = {
        "Authorization": "Bearer " + resendApiKey,
        "Content-Type": "application/json"
    };

    var body = {
        from: "onboarding@resend.dev",
        to: email,
        subject: subject,
        html: bodyHtml
    };

    var url = "https://api.resend.com/emails";
    var content = JSON.stringify(body);
    
    var httpResult = http.request(url, "post", content, "application/json", headers);
    log.info("Email sent response: " + JSON.stringify(httpResult));
    return httpResult;
}


handlers.InitializeNewPlayer = function(args, context) {
    var email = args.Email;
    
    server.UpdateUserReadOnlyData({
        PlayFabId: currentPlayerId,
        Data: { "UserEmail": email }
    });

    var statsRequest = {
        PlayFabId: currentPlayerId,
        Statistics: [
            { StatisticName: "Runs", Value: 0 },
            { StatisticName: "TimePlayed", Value: 0 },
            { StatisticName: "Collectibles", Value: 0 },
            { StatisticName: "XP", Value: 0 },
            { StatisticName: "Level", Value: 1 }
        ]
    };
    server.UpdatePlayerStatistics(statsRequest);
    
    log.info("Player initialized: " + currentPlayerId);
    return { initialized: true };
};


handlers.CollectItem = function(args, context) {
    var characterType = args.CharacterType;
    
    var titleData = server.GetTitleData({ Keys: ["GameDifficulty"] });
    var difficultySettings = JSON.parse(titleData.Data["GameDifficulty"]);
    
    var playerStats = server.GetPlayerStatistics({
        PlayFabId: currentPlayerId,
        StatisticNames: ["XP", "Level", "Collectibles"]
    }).Statistics;
    
    var currentXP = 0;
    var currentLevel = 1;
    var currentCollectibles = 0;
    
    playerStats.forEach(function(stat) {
        if (stat.StatisticName === "XP") currentXP = stat.Value;
        if (stat.StatisticName === "Level") currentLevel = stat.Value;
        if (stat.StatisticName === "Collectibles") currentCollectibles = stat.Value;
    });

    var charBonus = (characterType === 1) ? 1.2 : 1.0;
    
    var xpGain = difficultySettings.BaseXP * Math.pow(difficultySettings.DifficultyScaling, currentLevel-1) * charBonus;
    xpGain = Math.round(xpGain);
    
    var newXP = currentXP + xpGain;
    var newCollectibles = currentCollectibles + 1;
    
    var xpNeededForNextLevel = currentLevel * difficultySettings.LevelThresholdBase;
    var leveledUp = false;
    
    if (newXP >= xpNeededForNextLevel) {
        currentLevel++;
        newXP = newXP - xpNeededForNextLevel;
        leveledUp = true;
        
        // var userData = server.GetUserReadOnlyData({
        //     PlayFabId: currentPlayerId,
        //     Keys: ["UserEmail"]
        // });
        
        // if(userData.Data["UserEmail"]) {
        //     sendLevelUpEmail(userData.Data["UserEmail"].Value, currentLevel);
        // }
    }

    server.UpdatePlayerStatistics({
        PlayFabId: currentPlayerId,
        Statistics: [
            { StatisticName: "XP", Value: newXP },
            { StatisticName: "Level", Value: currentLevel },
            { StatisticName: "Collectibles", Value: newCollectibles }
        ]
    });

    return { 
        xpGained: xpGain, 
        totalXP: newXP, 
        newLevel: currentLevel, 
        leveledUp: leveledUp 
    };
};

handlers.SendLevelUpEmail = function(args, context) {
    var userData = server.GetUserReadOnlyData({
        PlayFabId: currentPlayerId,
        Keys: ["UserEmail"]
    });
    
    var userEmail = null;
    if(userData.Data["UserEmail"]) {
        userEmail = userData.Data["UserEmail"].Value;
    }

    if (userEmail) {

        var stats = server.GetPlayerStatistics({ PlayFabId: currentPlayerId, StatisticNames: ["Level"] }).Statistics;
        var lvl = 1;
        if(stats.length > 0) lvl = stats[0].Value;

        sendEmailViaResend(
            userEmail, 
            "Level Up Complete!", 
            "<p>Felicitari! Ai asteptat 1 minut si acum ai primit confirmarea pentru <strong>Level " + lvl + "</strong>!</p>"
        );
        return { emailSent: true };
    } else {
        return { emailSent: false, error: "No email found" };
    }
};

handlers.UpdateSessionStats = function(args, context) {
    var timeToAdd = args.TimeSeconds;
    
    var playerStats = server.GetPlayerStatistics({
        PlayFabId: currentPlayerId,
        StatisticNames: ["Runs", "TimePlayed"]
    }).Statistics;
    
    var runs = 0;
    var time = 0;
    playerStats.forEach(function(s){
        if(s.StatisticName === "Runs") runs = s.Value;
        if(s.StatisticName === "TimePlayed") time = s.Value;
    });
    
    server.UpdatePlayerStatistics({
        PlayFabId: currentPlayerId,
        Statistics: [
            { StatisticName: "Runs", Value: runs + 1 },
            { StatisticName: "TimePlayed", Value: time + timeToAdd }
        ]
    });
};




///

handlers.SendLevelUpEmail2 = function(args, context) {
    var playerEmail = args.playerEmail;
 
    var headers = {
        "Authorization": "Bearer ",
        "Content-Type": "application/json"
    };
 
    var body = {
        from: "onboarding@resend.dev",
        to: playerEmail,
        subject: "Level Up Complete!",
        html: "<p>Your training is complete!</p>"
    };
 
    var url = "https://api.resend.com/emails";
    var response = http.request(url, "post", JSON.stringify(body), "application/json", headers);
 
    return { emailSent: true };
};

handlers.incrementCounter = function(args, context) {
    var res = server.GetUserReadOnlyData({
        PlayFabId: currentPlayerId,
        Keys: ["Counter"]
    });
 
    var count = 0;
    if (res.Data["Counter"] !== undefined) {
        count = JSON.parse(res.Data["Counter"].Value);
    }
    count++;
 
    server.UpdateUserReadOnlyData({
        PlayFabId: currentPlayerId,
        Data: { "Counter": count }
    });
 
    // return count;
    return { "Counter": count };
};

// This is a Cloud Script function. "args" is set to the value of the "FunctionParameter" 
// parameter of the ExecuteCloudScript API.
// (https://api.playfab.com/Documentation/Client/method/ExecuteCloudScript)
// "context" contains additional information when the Cloud Script function is called from a PlayStream action.
handlers.helloWorld = function (args, context) {
    
    // The pre-defined "currentPlayerId" variable is initialized to the PlayFab ID of the player logged-in on the game client. 
    // Cloud Script handles authenticating the player automatically.
    var message = "Hello " + currentPlayerId + "!";

    // You can use the "log" object to write out debugging statements. It has
    // three functions corresponding to logging level: debug, info, and error. These functions
    // take a message string and an optional object.
    log.info(message);
    var inputValue = null;
    if (args && args.inputValue)
        inputValue = args.inputValue;
    log.debug("helloWorld:", { input: args.inputValue });

    // The value you return from a Cloud Script function is passed back 
    // to the game client in the ExecuteCloudScript API response, along with any log statements
    // and additional diagnostic information, such as any errors returned by API calls or external HTTP
    // requests. They are also included in the optional player_executed_cloudscript PlayStream event 
    // generated by the function execution.
    // (https://api.playfab.com/playstream/docs/PlayStreamEventModels/player/player_executed_cloudscript)
    return { messageValue: message };
};

// This is a simple example of making a PlayFab server API call
handlers.makeAPICall = function (args, context) {
    var request = {
        PlayFabId: currentPlayerId, Statistics: [{
                StatisticName: "Level",
                Value: 2
            }]
    };
    // The pre-defined "server" object has functions corresponding to each PlayFab server API 
    // (https://api.playfab.com/Documentation/Server). It is automatically 
    // authenticated as your title and handles all communication with 
    // the PlayFab API, so you don't have to write extra code to issue HTTP requests. 
    var playerStatResult = server.UpdatePlayerStatistics(request);
};

// This an example of a function that calls a PlayFab Entity API. The function is called using the 
// 'ExecuteEntityCloudScript' API (https://api.playfab.com/documentation/CloudScript/method/ExecuteEntityCloudScript).
handlers.makeEntityAPICall = function (args, context) {

    // The profile of the entity specified in the 'ExecuteEntityCloudScript' request.
    // Defaults to the authenticated entity in the X-EntityToken header.
    var entityProfile = context.currentEntity;

    // The pre-defined 'entity' object has functions corresponding to each PlayFab Entity API,
    // including 'SetObjects' (https://api.playfab.com/documentation/Data/method/SetObjects).
    var apiResult = entity.SetObjects({
        Entity: entityProfile.Entity,
        Objects: [
            {
                ObjectName: "obj1",
                DataObject: {
                    foo: "some server computed value",
                    prop1: args.prop1
                }
            }
        ]
    });

    return {
        profile: entityProfile,
        setResult: apiResult.SetResults[0].SetResult
    };
};

// This is a simple example of making a web request to an external HTTP API.
handlers.makeHTTPRequest = function (args, context) {
    var headers = {
        "X-MyCustomHeader": "Some Value"
    };
    
    var body = {
        input: args,
        userId: currentPlayerId,
        mode: "foobar"
    };

    var url = "http://httpbin.org/status/200";
    var content = JSON.stringify(body);
    var httpMethod = "post";
    var contentType = "application/json";

    // The pre-defined http object makes synchronous HTTP requests
    var response = http.request(url, httpMethod, content, contentType, headers);
    return { responseContent: response };
};

// This is a simple example of a function that is called from a
// PlayStream event action. (https://playfab.com/introducing-playstream/)
handlers.handlePlayStreamEventAndProfile = function (args, context) {
    
    // The event that triggered the action 
    // (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
    var psEvent = context.playStreamEvent;
    
    // The profile data of the player associated with the event
    // (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)
    var profile = context.playerProfile;
    
    // Post data about the event to an external API
    var content = JSON.stringify({ user: profile.PlayerId, event: psEvent.EventName });
    var response = http.request('https://httpbin.org/status/200', 'post', content, 'application/json', null);

    return { externalAPIResponse: response };
};


// Below are some examples of using Cloud Script in slightly more realistic scenarios

// This is a function that the game client would call whenever a player completes
// a level. It updates a setting in the player's data that only game server
// code can write - it is read-only on the client - and it updates a player
// statistic that can be used for leaderboards. 
//
// A funtion like this could be extended to perform validation on the 
// level completion data to detect cheating. It could also do things like 
// award the player items from the game catalog based on their performance.
handlers.completedLevel = function (args, context) {
    var level = args.levelName;
    var monstersKilled = args.monstersKilled;
    
    var updateUserDataResult = server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            lastLevelCompleted: level
        }
    });

    log.debug("Set lastLevelCompleted for player " + currentPlayerId + " to " + level);
    var request = {
        PlayFabId: currentPlayerId, Statistics: [{
                StatisticName: "level_monster_kills",
                Value: monstersKilled
            }]
    };
    server.UpdatePlayerStatistics(request);
    log.debug("Updated level_monster_kills stat for player " + currentPlayerId + " to " + monstersKilled);
};


// In addition to the Cloud Script handlers, you can define your own functions and call them from your handlers. 
// This makes it possible to share code between multiple handlers and to improve code organization.
handlers.updatePlayerMove = function (args) {
    var validMove = processPlayerMove(args);
    return { validMove: validMove };
};


// This is a helper function that verifies that the player's move wasn't made
// too quickly following their previous move, according to the rules of the game.
// If the move is valid, then it updates the player's statistics and profile data.
// This function is called from the "UpdatePlayerMove" handler above and also is 
// triggered by the "RoomEventRaised" Photon room event in the Webhook handler
// below. 
//
// For this example, the script defines the cooldown period (playerMoveCooldownInSeconds)
// as 15 seconds. A recommended approach for values like this would be to create them in Title
// Data, so that they can be queries in the script with a call to GetTitleData
// (https://api.playfab.com/Documentation/Server/method/GetTitleData). This would allow you to
// make adjustments to these values over time, without having to edit, test, and roll out an
// updated script.
function processPlayerMove(playerMove) {
    var now = Date.now();
    var playerMoveCooldownInSeconds = 15;

    var playerData = server.GetUserInternalData({
        PlayFabId: currentPlayerId,
        Keys: ["last_move_timestamp"]
    });

    var lastMoveTimestampSetting = playerData.Data["last_move_timestamp"];

    if (lastMoveTimestampSetting) {
        var lastMoveTime = Date.parse(lastMoveTimestampSetting.Value);
        var timeSinceLastMoveInSeconds = (now - lastMoveTime) / 1000;
        log.debug("lastMoveTime: " + lastMoveTime + " now: " + now + " timeSinceLastMoveInSeconds: " + timeSinceLastMoveInSeconds);

        if (timeSinceLastMoveInSeconds < playerMoveCooldownInSeconds) {
            log.error("Invalid move - time since last move: " + timeSinceLastMoveInSeconds + "s less than minimum of " + playerMoveCooldownInSeconds + "s.");
            return false;
        }
    }

    var playerStats = server.GetPlayerStatistics({
        PlayFabId: currentPlayerId
    }).Statistics;
    var movesMade = 0;
    for (var i = 0; i < playerStats.length; i++)
        if (playerStats[i].StatisticName === "")
            movesMade = playerStats[i].Value;
    movesMade += 1;
    var request = {
        PlayFabId: currentPlayerId, Statistics: [{
                StatisticName: "movesMade",
                Value: movesMade
            }]
    };
    server.UpdatePlayerStatistics(request);
    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            last_move_timestamp: new Date(now).toUTCString(),
            last_move: JSON.stringify(playerMove)
        }
    });

    return true;
}

// This is an example of using PlayStream real-time segmentation to trigger
// game logic based on player behavior. (https://playfab.com/introducing-playstream/)
// The function is called when a player_statistic_changed PlayStream event causes a player 
// to enter a segment defined for high skill players. It sets a key value in
// the player's internal data which unlocks some new content for the player.
handlers.unlockHighSkillContent = function (args, context) {
    var playerStatUpdatedEvent = context.playStreamEvent;
    var request = {
        PlayFabId: currentPlayerId,
        Data: {
            "HighSkillContent": "true",
            "XPAtHighSkillUnlock": playerStatUpdatedEvent.StatisticValue.toString()
        }
    };
    var playerInternalData = server.UpdateUserInternalData(request);
    log.info('Unlocked HighSkillContent for ' + context.playerProfile.DisplayName);
    return { profile: context.playerProfile };
};

// Photon Webhooks Integration
//
// The following functions are examples of Photon Cloud Webhook handlers. 
// When you enable the Photon Add-on (https://playfab.com/marketplace/photon/)
// in the Game Manager, your Photon applications are automatically configured
// to authenticate players using their PlayFab accounts and to fire events that 
// trigger your Cloud Script Webhook handlers, if defined. 
// This makes it easier than ever to incorporate multiplayer server logic into your game.


// Triggered automatically when a Photon room is first created
handlers.RoomCreated = function (args) {
    log.debug("Room Created - Game: " + args.GameId + " MaxPlayers: " + args.CreateOptions.MaxPlayers);
};

// Triggered automatically when a player joins a Photon room
handlers.RoomJoined = function (args) {
    log.debug("Room Joined - Game: " + args.GameId + " PlayFabId: " + args.UserId);
};

// Triggered automatically when a player leaves a Photon room
handlers.RoomLeft = function (args) {
    log.debug("Room Left - Game: " + args.GameId + " PlayFabId: " + args.UserId);
};

// Triggered automatically when a Photon room closes
// Note: currentPlayerId is undefined in this function
handlers.RoomClosed = function (args) {
    log.debug("Room Closed - Game: " + args.GameId);
};

// Triggered automatically when a Photon room game property is updated.
// Note: currentPlayerId is undefined in this function
handlers.RoomPropertyUpdated = function (args) {
    log.debug("Room Property Updated - Game: " + args.GameId);
};

// Triggered by calling "OpRaiseEvent" on the Photon client. The "args.Data" property is 
// set to the value of the "customEventContent" HashTable parameter, so you can use
// it to pass in arbitrary data.
handlers.RoomEventRaised = function (args) {
    var eventData = args.Data;
    log.debug("Event Raised - Game: " + args.GameId + " Event Type: " + eventData.eventType);

    switch (eventData.eventType) {
        case "playerMove":
            processPlayerMove(eventData);
            break;

        default:
            break;
    }
};
