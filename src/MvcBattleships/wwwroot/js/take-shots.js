var lastClickedRow;
var lastClickedCol;
var lastClicked;
$(document).ready(function () {
    $(".opponent-field td").click(function () {
        if (lastClicked) {
            lastClicked.removeClass("clicked");
        }
        lastClicked = $(this);
        lastClicked.addClass("clicked");
        lastClickedRow = $(this).parent().index()+1;
        lastClickedCol = $(this).index()+1;
    });
    $("#fire").click(function () {
        if (!lastClicked) {
            //show error
            console.log("Nothing has been clicked.");
            alert("Nothing has been clicked.");
        }
        else if ($(lastClicked).html() !== "-")
        {
            console.log("This tile has already been fired upon. Pick a new location.");
            alert("This tile has already been fired upon. Pick a new location.");
        }
        else
        {
            var data = {
                row: lastClickedRow,
                col: lastClickedCol
            };
            $.ajax({
                //url: '/Game/TakeShot',
                url: "TakeShot",
                type: 'POST',
                data: JSON.stringify({ 
                    row: lastClickedRow,
                    col: lastClickedCol
                }),
                contentType: 'application/json; charset=utf-8',
                error: function () {
                    alert("Something went wrong when taking the shot.");
                }
                }).done(function (response) {
                    
                    if (response.modelResponse) 
                    {
                        $(lastClicked).html("X");
                    }
                    else if (response.modelResponse===false)
                    {
                        $(lastClicked).html("M");
                    }
                    else
                    {
                        console.log(response);
                        alert(response.error);
                    }
                    if (response.incomingShot.playerHit)
                    {
                        document.getElementById("player-board").rows[response.incomingShot.row-1].cells[response.incomingShot.col-1].innerHTML = "X";
                    }
                    else if (response.incomingShot.playerHit===false)
                    {
                        document.getElementById("player-board").rows[response.incomingShot.row-1].cells[response.incomingShot.col-1].innerHTML = "M";
                    }
                    if (response.win.playerWin === true) {
                        alert("You win!");
                    } 
                    else if (response.win.opponentWin === true)
                    {
                        alert("You lose!");
                    }
                });
       
               

        }
    });
});