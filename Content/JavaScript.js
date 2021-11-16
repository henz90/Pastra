let clickCount = 0;

let selected;

function handClick(cardValue, cardID) {
    console.log(clickCount);
    var board = Array.from(document.getElementsByClassName("inplay"));
    unhighlight();

    //  Highlight selected card
    var playedCard = document.getElementById(cardID);
    highlightThis(playedCard);

    //  Played card is a Jack, highlight all cards to collect
    if (cardValue == 11) {
        for (var i = 0; i < board.length; i++) {
            highlightThis(board[i]);
        }
    }
    //  Played card can combine certain cards on the board to equal them
    else if (cardValue <= 10) {
        var highlight = [];
        //  Obtain combinations
        var combinations = getCombinations(board);
        //  Obtain IDs
        var ids = getIds(board);
        //  Combine combinations and IDs    //  FIXME:  This step can be done by combining getCombinations and GetIds... Find out how
        for (var i = 0; i < combinations.length; i++) {
            highlight.push([ids[i], combinations[i]]);
        }
        highlight.forEach(combo => {
            if (cardValue == combo[1]) {
                for (var i = 0; i < combo[0].length; i++) {
                    //  Highlight those cards
                    highlightThis(combo[0][i]);
                }
            }
        });
    }
    else {
        //  For Face cards 
        for (var i = 0; i < board.length; i++) {
            var value = parseInt(board[i].className.replace("inplay card ", ""));
            if (value == cardValue) {
                highlightThis(board[i]);
            }
        }
    }
    if (clickCount % 2 != 0 && playedCard == selected) {
        playedCard.parentElement.onclick = function () { return true; };
    }
    else {
        selected = playedCard;
        clickCount++;
    }
}

function highlightThis(img) {
    img.style.backgroundColor = "goldenrod";
    img.style.borderColor = "goldenrod";
}

function unhighlight() {
    var cards = document.getElementsByClassName("card");

    for (var i = 0; i < cards.length; i++) {
        cards[i].style.backgroundColor = "white";
        cards[i].style.borderColor = "white";
    }
}

//  https://codereview.stackexchange.com/questions/7001/generating-all-combinations-of-an-array
function getCombinations(board) {
    var combinations = [];
    var f = function (prefix, subBoard) {
        for (var i = 0; i < subBoard.length; i++) {
            var value = parseInt(subBoard[i].className.replace("inplay card ", ""));
            var newPrefix = prefix + value;
            combinations.push(newPrefix);
            f(newPrefix, subBoard.slice(i + 1));
        }
    }
    f(0, board);
    return combinations;
}

//  https://codereview.stackexchange.com/questions/7001/generating-all-combinations-of-an-array
function getIds(board) {
    var ids = [];
    var prefix = [];
    var f = function (previous, subBoard) {
        for (var i = 0; i < subBoard.length; i++) {
            var id = subBoard[i];
            var newPrevious = previous.slice();
            newPrevious.push(id);
            ids.push(newPrevious);
            f(newPrevious, subBoard.slice(i + 1));
        }
    }
    f(prefix, board);
    return ids;
}