disableCodeBoxes();
registerFocusListener();

function getCodeBoxes(){
    let codeBoxes = document.querySelectorAll(".code-box");
    return codeBoxes;
}

function disableCodeBoxes(){
    let codes = getCodeBoxes();
    for(let i = 1; i<codes.length; ++i){
        codes[i].disabled = true;
    }
    codes[0].focus();
}

function registerFocusListener(){
    let codes = getCodeBoxes();
    for(let i = 0; i<codes.length; ++i){
        codes[i].addEventListener("input",()=>{switchCodeBox(i);},false);
    }
    
}

function switchCodeBox(nextBoxId){
    let codes = getCodeBoxes();
    if(nextBoxId !== (codes.length-1)){
        codes[nextBoxId+1].disabled = false;
        codes[nextBoxId+1].focus();
    }
    concatenateCode();
}

function concatenateCode(){
    let hiddenCodeField = document.querySelector("#codeTextBox");
    let codes = getCodeBoxes();
    let code = "";
    for(let i = 0; i<codes.length; ++i){
        code += codes[i].value;
    }
    hiddenCodeField.value = code;
}

function removeSwitchCodeBox(){
    let codes = getCodeBoxes();
    for(let i = 0; i<codes.length; ++i){
        codes[i].removeEventListener("input",()=>{switchCodeBox(i);},false);
    }
}
