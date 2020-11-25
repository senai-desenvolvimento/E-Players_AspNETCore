// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function botaoMenu() {

    var m = document.getElementById("menu")
    if (m.style.display == "block") {
        m.style.display = "none";
    } else {
        m.style.display = "block"
    }
}
