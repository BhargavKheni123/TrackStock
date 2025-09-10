$(document).ready(function () {
    var eTurnsHub = $.connection.eTurnsHub;
    // Create a function that the hub can call back to display messages.
    eTurnsHub.client.UpdateRedCircleCountInClients = function () {
        //FillRedCount();
        SetReplenishRedCount();
        SetConsumeRedCount();
    };


    eTurnsHub.client.updateUsersOnlineCount = function (count, userList) {
        // Add the message to the page. 
        
        $('#eTurnsUsersRightNow').text(count);
        var UserList = '';
       
        
        for (var i = 0; i < userList.length; i++)
        {
            if(UserList=='')
            {
                UserList = userList[i];
            }
            else {
                UserList =UserList+','+ userList[i];
            }
           

        }
        var postData = { "userList": JSON.stringify(userList) };
        $.ajax({
            url: "/Master/SaveUserListInSession",
            data: (postData),
            type:"POST",
            async: false,
            cache: false,
            success:function(){

            }
        });
        $("#UserNameList").val(UserList);
        //alert($("#UserLoginModelPS").length);
        if ($('#UserLoginModelPS').dialog('isOpen') && window.location.href.toLowerCase().indexOf('enterpriselist') >= 0)
        {
            openUserDetail();
        }

        if (window.location.href.toLowerCase().indexOf('enterpriselist') >= 0) {
            $('#eTurnsUsersRightNow').show();
            $('#lbleTurnsUsersRightNow').show();
            $("#clickCount").show();
        }
        else {
            $('#eTurnsUsersRightNow').hide();
            $('#lbleTurnsUsersRightNow').hide();
            $("#clickCount").hide();
        }
    };

    eTurnsHub.client.OnSapphireSessionTimout = function () {
        alert("Session Timout");
    };
    $.connection.hub.start(function () {
        if (EID > 0 && CID > 0 && RID > 0) {
            eTurnsHub.server.joinRoom(EID + "_" + CID + "_" + RID);
        }
    });
    $.connection.hub.start().done(function () { }).fail(function (e) { });

});