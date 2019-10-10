const copyInvoiceCheckBox = document.getElementById("CopyInvoiceFromAddress");
const billingAddressCollapse = document.getElementById("billingAddress");

function ShowInvoiceSection() {
    if (this.checked)
        $(billingAddressCollapse).collapse("hide");
    else
        $(billingAddressCollapse).collapse("show");
}

function addCopyInvoiceAddressCheckEventListener() {
    copyInvoiceCheckBox.addEventListener('click', ShowInvoiceSection, false);
}

addCopyInvoiceAddressCheckEventListener();