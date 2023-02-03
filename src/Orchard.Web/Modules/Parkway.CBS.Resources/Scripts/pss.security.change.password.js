$(document).ready(function(){

const changepasswordLink = '<a class="nav-item nav-link" href="'+changePwdURL+'">Change Password</a>';
const securityLink = '<a class="nav-item nav-link" href="'+changePwdURL+'">Settings</a>';
const subUsersLink = '<a href="'+ subUsersURL +'" class="nav-item nav-link">Sub Users</a>';
const branchesLink = '<a href="'+ branchesURL +'" class="nav-item nav-link">Branches</a>';

    updateDropdownMenu();
        
    $(window).resize(function(){
       updateDropdownMenu();
      }
    );

    function updateDropdownMenu(){
        if(parseInt($(window).attr("innerWidth")) < 992){
            $(".dropdown-menu #signoutForm #settings .nav-item").remove();
            $(".dropdown-menu #signoutForm #settings").append(changepasswordLink);
            if (canShowSubUsersRequestReport && isMain) { $(".dropdown-menu #signoutForm #settings").append(branchesLink, subUsersLink); }
            }else{
            $(".dropdown-menu #signoutForm #settings .nav-item").remove();
            $(".dropdown-menu #signoutForm #settings").append(securityLink);
           }
            $("#signoutlnk").click(function (e) {
                    $('#signoutForm').get(0).submit();
                });
    }

});