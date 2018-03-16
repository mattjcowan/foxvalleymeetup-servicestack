$('.message a').click(function(){
   $('form').animate({height: "toggle", opacity: "toggle"}, "slow");
});

$('#login').click(function(event) {
    event.preventDefault();
    event.stopPropagation();
    var $login_username = $("#login_username");
    var $login_password = $("#login_password");
    $login_username.removeClass('error');
    $login_password.removeClass('error');
    
    var login_username = $login_username.val();
    var login_password = $login_password.val();
    $.ss.postJSON('/auth/credentials', { username: login_username, password: login_password, rememberMe: true }, function() {
        window.location = '/';
    }, function () {
        $login_username.addClass('error');
        $login_password.addClass('error');
    });
});

$('#register').click(function() {
    event.preventDefault();
    event.stopPropagation();
    var $register_username = $("#register_username");
    var $register_password = $("#register_password");
    var $register_email = $("#register_email");
    $register_username.removeClass('error');
    $register_password.removeClass('error');
    $register_email.removeClass('error');
    
    var register_username = $register_username.val();
    var register_password = $register_password.val();
    var register_email = $register_email.val();
    $.ss.postJSON('/register', { username: register_username, password: register_password, email: register_email, displayName: register_email }, function() {
        window.location = '/login';
    }, function () {
        $register_username.addClass('error');
        $register_password.addClass('error');
        $register_email.addClass('error');
    });
});
