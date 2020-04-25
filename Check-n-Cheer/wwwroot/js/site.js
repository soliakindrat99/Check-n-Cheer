const password = document.getElementById("password")
    , confirm_password = document.getElementById("confirm_password")
    , search_user_input = document.getElementById("search_user_input")
    , search_test_input = document.getElementById("search_test_input")
    , search_user_btn = document.getElementById("search_user_btn")
    , search_test_btn = document.getElementById("search_test_btn");

function search_user() {
    let text = search_user_input.value;
    location.replace("/User/AdminProfile/" + text);
}

function search_test() {
    let text = search_test_input.value;
    console.log(text);
    location.replace("/Test/TestHistory/" + text);
}

function validatePassword() {
    if (password.value != confirm_password.value) {
        confirm_password.setCustomValidity("Passwords Don't Match");
    } else {
        confirm_password.setCustomValidity('');
    }
}
if (search_user_btn !== null) {
    search_user_btn.onclick = search_user;
}
if (search_test_btn !== null) {
    search_test_btn.onclick = search_test;
}

if (password !== null) {
    password.onchange = validatePassword;
    confirm_password.onkeyup = validatePassword;
}