import './admin/filter/optionField';
import './plant-overview/filterMenu';
import './chat/chat';
import './file-upload';
// import for password hide/show
import 'vanilla-js-show-hide-password/toggle-password'

// open mobile menu
window.openMenu = () => {
    document.getElementById("mobile_menu").classList.contains("hidden") ?
        document.getElementById("mobile_menu").classList.remove("hidden") :
        document.getElementById("mobile_menu").classList.add("hidden")

    document.getElementById("mobile_menu").classList.contains("block") ?
        document.getElementById("mobile_menu").classList.remove("block") :
        document.getElementById("mobile_menu").classList.add("block")
}