mergeInto(LibraryManager.library, {
    sendMultiScore: function (roomIdPtr, playerNamePtr, score, difficultyPtr) {

        var roomId = UTF8ToString(roomIdPtr);
        var playerName = UTF8ToString(playerNamePtr);
        var difficulty = UTF8ToString(difficultyPtr);

        fetch("mp_update.php", {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            },
            body: new URLSearchParams({
                room_id: roomId,
                player_name: playerName,
                score: score,
                difficulty: difficulty
            })
        })
        .then(res => res.json())
        .then(data => {
            console.log("✅ WebGL送信成功", data);
        })
        .catch(err => {
            console.error("❌ WebGL送信失敗", err);
        });
    }
});