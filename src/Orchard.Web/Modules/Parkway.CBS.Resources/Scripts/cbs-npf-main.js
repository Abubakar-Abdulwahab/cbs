var loginBtn = document.getElementsByClassName("login-btn");
var loginCloseBtn = document.querySelector(".login-modal .modal-close-btn");
var loginLinkBtn = document.getElementsByClassName("login-toggle");
var signUpFullBtn = document.getElementsByClassName("signup-toggle");
var signUpFullCloseBtn = document.querySelector(".signup-modal-full .modal-close-btn");
var createExtractBtn = document.getElementsByClassName("create-extract-text");
var cancelExtractBtn = document.getElementsByClassName("cancel-extract-btn");
var saveExtractTemplateBtn = document.getElementsByClassName("save-template-btn");
var signUpCloseBtn = document.querySelector(".signup-modal .modal-close-btn");
var successAlertBarDeleteBtn = document.getElementsByClassName("delete-success-alert-icon");
var warningAlertBarDeleteBtn = document.getElementsByClassName("delete-warning-alert-icon");
var infoAlertBarDeleteBtn = document.getElementsByClassName("delete-info-alert-icon");
var errorAlertBarDeleteBtn = document.getElementsByClassName("delete-error-alert-icon");
var verifyAccountAlertBarDeleteBtn = document.getElementsByClassName("delete-verified-alert-icon");
var selectedPaymentOptions = document.querySelectorAll("#payment-options-section .nav-item");
var signupPwdIcon = document.querySelector("#signupPwdContainer img");
var signupCpwdIcon = document.querySelector("#signupCpwdContainer img");
var loginPwdIcon = document.querySelector("#loginPwdContainer img");


/*if(loginBtn[0] != null){ 
    loginBtn[0].addEventListener("click",toggleLoginModal,false);
    if(loginCloseBtn != null){
        loginCloseBtn.addEventListener("click",toggleLoginModal,false);
    }
}*/

if(signUpFullBtn[0] != null){
    signUpFullBtn[0].addEventListener("click",toggleSignUpFullModal,false);
    if(signUpFullCloseBtn != null){
    signUpFullCloseBtn.addEventListener("click",toggleSignUpFullModal,false);
    }
    if(loginLinkBtn[0] != null){
    loginLinkBtn[0].addEventListener("click",toggleLoginModal,false);
    }
}

if(createExtractBtn[0] != null){
    toggleExtractCreateBtn();
    createExtractBtn[0].addEventListener("click",toggleEditExtractTemplateModal, false);
    let deleteExtractBtn  = document.getElementById("deleteExtractIcon");
    let editExtractBtn = document.getElementById("editExtractIcon");
    let previewExtractBtn = document.getElementById("previewExtractIcon");
    let closeExtractBtn = document.querySelector(".show-extract-modal .modal-close-btn");
    deleteExtractBtn.addEventListener("click",toggleExtractIcons, false);
    editExtractBtn.addEventListener("click",toggleEditExtractTemplateModal, false);
    previewExtractBtn.addEventListener("click",toggleExtractModal,false);
    closeExtractBtn.addEventListener("click",toggleExtractModal,false);
}

if(cancelExtractBtn[0] != null){
    cancelExtractBtn[0].addEventListener("click",toggleEditExtractTemplateModal,false);
}

if(saveExtractTemplateBtn[0] != null){
    saveExtractTemplateBtn[0].addEventListener("click",saveExtractTemplate,false);
}

if(signUpCloseBtn != null){
    signUpCloseBtn.addEventListener("click",toggleSignupModal,false);
}

if(successAlertBarDeleteBtn[0] != null){
    successAlertBarDeleteBtn[0].addEventListener("click",toggleSuccessAlert,false);
}

if(warningAlertBarDeleteBtn[0] != null){
    warningAlertBarDeleteBtn[0].addEventListener("click",toggleWarningAlert,false);
}

if(infoAlertBarDeleteBtn[0] != null){
    infoAlertBarDeleteBtn[0].addEventListener("click",toggleInfoAlert,false);
}

if(errorAlertBarDeleteBtn[0] != null){
    errorAlertBarDeleteBtn[0].addEventListener("click",toggleErrorAlert,false);
}

if(verifyAccountAlertBarDeleteBtn[0] != null){
    verifyAccountAlertBarDeleteBtn[0].addEventListener("click",toggleVerifiedAccountAlert,false);
    toggleVerifiedAccountAlert();
}

if(selectedPaymentOptions[0] != null){
    for(let i = 0; i<selectedPaymentOptions.length; ++i){
        selectedPaymentOptions[i].addEventListener("click",togglePaymentOptions,false);
    }
}

if(signupPwdIcon != null){
    signupPwdIcon.addEventListener("click",toggleSignupPassword,false);
}

if(signupCpwdIcon != null){
    signupCpwdIcon.addEventListener("click",toggleSignupCpassword,false);
}

if(loginPwdIcon != null){
    loginPwdIcon.addEventListener("click",toggleLoginPassword,false);
}

function toggleLoginModal(){
    let loginModal = document.getElementsByClassName("login-modal");
    let signUpModalFull = document.getElementsByClassName("signup-modal-full");
    if(loginModal != null){
        if(loginModal[0].style.display != "block"){
            if(signUpModalFull[0].style.display == "block"){
                toggleSignUpFullModal();
            }
            loginModal[0].style.display = "block";
        }else{
            loginModal[0].style.display = "none";
        }
    }
}

function toggleSignUpFullModal(){
    let signUpModalFull = document.getElementsByClassName("signup-modal-full");
    if(signUpModalFull != null){
        if(signUpModalFull[0].style.display != "block"){
            toggleLoginModal();
            signUpModalFull[0].style.display = "block";
        }else{
            signUpModalFull[0].style.display = "none";
        }
    }
}

function toggleExtractIcons(){
    let extractIcons = document.getElementsByClassName("extract-icon-links");
    if(extractIcons[0] != null){
        if(extractIcons[0].style.display != "block"){
            toggleExtractCreateBtn();
            extractIcons[0].style.display = "block";
                }else{
            toggleExtractCreateBtn();
            extractIcons[0].style.display = "none";
                }
    }
}

function toggleExtractCreateBtn(){
    let createExtractToggle = document.getElementsByClassName("create-extract-text");
    if(createExtractToggle[0] != null){
        if(createExtractToggle[0].style.display != "block"){
            createExtractToggle[0].style.display = "block";    
                }else{
            createExtractToggle[0].style.display = "none";
                }
      }

}

function toggleEditExtractTemplateModal(){
    let editExtractModalToggle = document.getElementsByClassName("edit-extract-modal");
    if(editExtractModalToggle[0] != null){
        if(editExtractModalToggle[0].style.display != "block"){
            editExtractModalToggle[0].style.display = "block";
                }else{
            editExtractModalToggle[0].style.display = "none";
        }
    } 
}

function saveExtractTemplate(){
    let createExtractToggle = document.getElementsByClassName("create-extract-text");
    if(createExtractToggle[0] != null){
        if(createExtractToggle[0].style.display == "block"){
                toggleExtractIcons();
                toggleEditExtractTemplateModal();
        }else{
             toggleEditExtractTemplateModal();
        }
    }
}

function toggleExtractModal(){
    let showExtractBtn = document.getElementsByClassName("show-extract-modal");
    if(showExtractBtn[0] != null){
        if(showExtractBtn[0].style.display != "block"){
            showExtractBtn[0].style.display = "block";
        }else{
             showExtractBtn[0].style.display = "none";
        }
    }
}

function toggleSignupModal(){
     let toggleSignupModalBtn = document.getElementsByClassName("signup-modal");
    if(toggleSignupModalBtn[0] != null){
        if(toggleSignupModalBtn[0].style.display != "block"){
            toggleSignupModalBtn[0].style.display = "block";
        }else{
             toggleSignupModalBtn[0].style.display = "none";
        }
    }
}

function toggleSuccessAlert(){
    let successBar = document.getElementsByClassName("success-alert-bar");
    if(successBar[0] != null){
        if(successBar[0].style.display != "block"){
                successBar[0].style.display = "block";
            }else{
                successBar[0].style.display = "none";
            }
    }
}

function toggleWarningAlert(){
    let warningBar = document.getElementsByClassName("warning-alert-bar");
    if(warningBar[0] != null){
        if(warningBar[0].style.display != "block"){
                warningBar[0].style.display = "block";
            }else{
                warningBar[0].style.display = "none";
            }
    }
}

function toggleInfoAlert(){
    let infoBar = document.getElementsByClassName("info-alert-bar");
    if(infoBar[0] != null){
        if(infoBar[0].style.display != "block"){
                infoBar[0].style.display = "block";
            }else{
                infoBar[0].style.display = "none";
            }
    }
}

function toggleErrorAlert(){
    let errorBar = document.getElementsByClassName("error-alert-bar");
    if(errorBar[0] != null){
        if(errorBar[0].style.display === "none"){
                errorBar[0].style.display = "block";
            }else{
                errorBar[0].style.display = "none";
            }
    }
}

function toggleVerifiedAccountAlert(){
    let verifiedAccoutnBar = document.getElementsByClassName("verify-account-alert-bar");
    if(verifiedAccoutnBar[0] != null){
        if(verifiedAccoutnBar[0].style.display != "block"){
                verifiedAccoutnBar[0].style.display = "block";
            }else{
                verifiedAccoutnBar[0].style.display = "none";
            }
    }
}

function findActivePaymentOption(){
    let paymentOptions = document.querySelectorAll("#payment-options-section .nav-item");
    if(paymentOptions[0] != null){
    for(let i = 0; i<paymentOptions.length; ++i){
        if(paymentOptions[i].classList.contains("active")){
            return i;
        }
    }
}else{
        return 0;
    }

}

function togglePaymentOptions(){
    let paymentOptions = document.querySelectorAll("#payment-options-section .nav-item");
    paymentOptions[findActivePaymentOption()].classList.toggle("active");
    this.classList.toggle("active");
}

function toggleSignupPassword(){
    let pwd = document.getElementById("pwd");
    if(pwd != null){
        if(pwd.type != "text"){
            pwd.type = "text";
        }else{
            pwd.type = "password";
        }
    }

}

function toggleSignupCpassword(){
    let cpwd = document.getElementById("cpwd");
    if(cpwd != null){
        if(cpwd.type != "text"){
            cpwd.type = "text";
        }else{
            cpwd.type = "password";
        }
    }

}

function toggleLoginPassword(){
    let pwd = document.getElementById("pwd");
    if(pwd != null){
        if(pwd.type != "text"){
            pwd.type = "text";
        }else{
            pwd.type = "password";
        }
    }

}