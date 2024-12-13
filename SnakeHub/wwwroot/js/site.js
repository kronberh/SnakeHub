const loadingGif = '<img class="img-fluid w-25" src="/img/loading.svg" alt="Loading..." />';

function onRoute() {
    $(`.nav-link[href="${window.location.pathname.split('?')[0]}"]`).addClass('active');
    $.ajax({
        url: window.location.hash,
        type: 'GET',
        headers: {
            'IncludeLayout': false
        },
        success: function (data) {
            $('main')[0].innerHTML = data;
        }
    });
}

function onAuthFormSubmit(formQuery, buttonQuery, responseContainerQuery) {
    $(buttonQuery)[0].disabled = true;
    $(responseContainerQuery)[0].innerHTML = loadingGif;
    $.ajax({
        url: $(formQuery).attr('action'),
        type: $(formQuery).attr('method'),
        data: $(formQuery).serialize(),
        success: function (data) {
            if (data.value.trim() === '') {
                location.reload();
            } else {
                $(responseContainerQuery)[0].innerHTML = `<p class="text-danger">${data.value}</p>`;
            }
        },
        error: function (xhr, status, error) {
            $(responseContainerQuery)[0].innerHTML = `<p class="text-danger">${status}</p>`;
        },
        complete: function () {
            $(buttonQuery)[0].disabled = false;
        }
    });
}

window.onload = () => {
    $('#register-form').on('submit', function (event) {
        event.preventDefault();
        onAuthFormSubmit(this, 'button[form="register-form"]', '#register-response-container');
    });
    $('#login-form').on('submit', function (event) {
        event.preventDefault();
        onAuthFormSubmit(this, 'button[form="login-form"]', '#login-response-container');
    });
    $('#logout-form').on('submit', function (event) {
        event.preventDefault();
        onAuthFormSubmit(this, 'button[form="logout-form"]', '#logout-response-container');
    });
    onRoute();
};

let intervalId = null;
let gameId = null;
let lost = false;

async function startGame(newGameId) {
    document.getElementById('start-game-btn').style.display = 'none';
    const response = await fetch(`/Games/StartGame?gameId=${newGameId}`, { method: "POST" });
    if (response.ok) {
        gameId = newGameId;
        startGameUpdates();
    }
}

async function joinGame(newGameId) {
    document.getElementById('join-game-btn').style.display = 'none';
    gameId = newGameId;
    startGameUpdates();
}

document.addEventListener("keydown", async (event) => {
    let action;
    switch (event.key) {
        case "w":
        case "ArrowUp":
            action = "UP";
            break;
        case "s":
        case "ArrowDown":
            action = "DOWN";
            break;
        case "a":
        case "ArrowLeft":
            action = "LEFT";
            break;
        case "d":
        case "ArrowRight":
            action = "RIGHT";
            break;
        default:
            return;
    }
    await fetch(`/Games/PlayerAction?gameId=${gameId}`,
        {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ action: action })
        });
});

function startGameUpdates() {
    if (intervalId) {
        clearInterval(intervalId);
    }
    intervalId = setInterval(async () => {
        let response = await fetch(`/Games/GetGameState?gameId=${gameId}`);
        if (response.ok) {
            const gameState = await response.json();
            redrawCanvas(gameState);
        } else {
            clearInterval(intervalId);
        }
        if (!lost) {
            response = await fetch(`/Games/GetPlayerScore?gameId=${gameId}`);
            if (response.ok) {
                const score = await response.json();
                document.getElementById('score-span').innerText = score;
            } else {
                lost = true;
            }
        }
    }, 1000 / 60);
}

function redrawCanvas(gameState) {
    const canvas = document.getElementById("game-canvas");
    const ctx = canvas.getContext("2d");
    const cellWidth = canvas.width / gameState[0].length;
    const cellHeight = canvas.height / gameState.length;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    for (let x = 0; x < gameState.length; x++) {
        for (let y = 0; y < gameState[x].length; y++) {
            const cell = gameState[x][y];
            if (cell && cell.color) {
                ctx.fillStyle = `rgba(${cell.color.r}, ${cell.color.g}, ${cell.color.b}, ${cell.color.a})`;
                ctx.fillRect(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
            }
        }
    }
}