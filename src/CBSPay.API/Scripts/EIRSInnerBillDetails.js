function editItemAmount() {
    var itemId = document.getElementById("EditItemRefRuleID").innerText;
    var EditRuleAmountTopayStr = parseInt(document.getElementById("EditRuleAmountTopay").value).toLocaleString() + '.00';
    document.getElementById('payamount_' + itemId).innerText = EditRuleAmountTopayStr;
    document.getElementById('payamountHidden_' + itemId).innerText = document.getElementById("EditRuleAmountTopay").value;
    document.getElementById('RefRules_' + itemId).value = document.getElementById("EditRuleAmountTopay").value;
    var pay_amount_array = document.getElementsByClassName("pay_amount");
    var total = 0;
    for (var i = 0; i < pay_amount_array.length; i++)
    {
        total += parseInt(Number(pay_amount_array[i].innerText));
        console.log(pay_amount_array[i].innerText);
    }
    console.log("the computed total is ",total);
    document.getElementById("TotalAmountToPay").value = total;
    document.getElementById("TotalAmountToPay").setAttribute('value', total);
    document.getElementById("ShowTotalAmountToPay").innerText = total.toLocaleString() + '.00';
}
function ShowdvViewAssessmentRuleModal(name, item, itemName, computation) {
    console.log(name, item, itemName, computation);
    document.getElementById("ViewitemRuleName").innerText = name;
    document.getElementById("ViewitemRuleItemRef").innerText = item;
    document.getElementById("ViewitemRuleItemName").innerText = itemName;
    document.getElementById("ViewitemRuleComputation").innerText = computation;
}
function ShowdvEditAmountModal(RuleName, RuleItemRef, RuleItemName, OutstandingAmountToString, OutStandingAmount, EditItemRefRuleID)
{
    document.getElementById("EditItemRuleName").innerText = RuleName;
    document.getElementById("EditItemRuleItemRef").innerText = RuleItemRef;
    document.getElementById("EditItemRuleItemName").innerText = RuleItemName;
    document.getElementById("EditItemOutstandingAmountToString").innerText = OutstandingAmountToString;
    document.getElementById("EditRuleAmountTopay").value = OutStandingAmount;

    document.getElementById("EditItemRefRuleID").innerText = EditItemRefRuleID;

    document.getElementById("EditRuleAmountTopay").max = OutStandingAmount;
    document.getElementById("EditRuleAmountTopay").min = 0;

}
function ShowdvViewPaymentModal(PaymentitemRuleName, PaymentitemRuleItemRef, PaymentitemSettledAmountToString) {
    document.getElementById("PaymentitemRuleName").innerText = PaymentitemRuleName;
    document.getElementById("PaymentitemRuleItemRef").innerText = PaymentitemRuleItemRef;
    document.getElementById("PaymentitemSettledAmountToString").innerText = PaymentitemSettledAmountToString;

}
function check(e) {
    console.log(e);
    var value = Number(e.value);
    if (value < 0) {
        e.value = 0;
        alert("Sorry!,you can't input a negative amount")
        return;
    }
    var max = Number(e.max);
    if (max < value) {
        e.value = e.max;
        alert("Sorry!, you have entered an amount that exceeds your outstanding debt!!")
        return;
    }
    //!(e.value >= 0 && e.value <= e.max)
    //Number(value) == NaN || Number(value) == Infinity || Number(value) == -Infinity || Number(value) == undefined || Number(value) == null
    // Number(value).toString() == "NaN"
    // 
    if (!Number(e.value) && !(Number(e.value) === 0 && e.value !== '')) {
        e.value = 0;
        alert("Sorry! you have entered an invalid number!!")
        return;
    }
}