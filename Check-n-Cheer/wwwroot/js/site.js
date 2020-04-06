const password = document.getElementById("password")
    , confirm_password = document.getElementById("confirm_password")
    , search_user = document.getElementById("search_user")
    , search_btn = document.getElementById("search_btn");

function search() {
    let text = search_user.value;
    location.replace("/User/AdminProfile/" + text);
}

function validatePassword() {
    if (password.value != confirm_password.value) {
        confirm_password.setCustomValidity("Passwords Don't Match");
    } else {
        confirm_password.setCustomValidity('');
    }
}
if (search_btn !== null) {
    search_btn.onclick = search;
}

if (password !== null) {
    password.onchange = validatePassword;
    confirm_password.onkeyup = validatePassword;
}