const deliveryRadios = document.getElementsByName("DeliveryMethod");
const homeDeliveryCollapse = document.getElementById("homeSection");
const storeDeliveryCollapse = document.getElementById("storeSection");
const copyInvoiceRadios = document.getElementsByName("CopyInvoiceFromAddress");
const invoiceDetailsCollapse = document.getElementById("invoiceDetails");
const copyAddressBlock = document.getElementById('copyInvoiceSection');
const iptWantInvoice = document.getElementById('WantInvoiceWithTaxNumber');
const btnWantInvoice = document.getElementById('btnWantInvoice');
const btnCancelInvoice = document.getElementById('btnCancelInvoice');
const shippingArticle = document.getElementById('shipping-cost');
const basketTotalSection = document.getElementById('basket-total');

let basketTotalWithShipping;
let basketTotalWithoutShipping;

function setBasketTotals(subtotal, total) {
    basketTotalWithoutShipping = subtotal;
    basketTotalWithShipping = total;
}

function ShowDeliverySection() {
    if (this.checked) {
        if (this.id === 'DeliveryMethodType@HOME') {
            $(storeDeliveryCollapse).collapse('hide');
            $(homeDeliveryCollapse).collapse('show');
            /* Invoice Section */
            $(copyAddressBlock).show();
            $(invoiceDetailsCollapse).collapse('hide');
            /* Shipping Section */
            $(shippingArticle)
                .removeClass("d-none")
                .addClass("d-flex");
            $(basketTotalSection).text(toNumber(basketTotalWithShipping));

        }
        else if (this.id === 'DeliveryMethodType@STORE') {
            $(homeDeliveryCollapse).collapse('hide');
            $(storeDeliveryCollapse).collapse('show');
            /* Invoice Section */
            $(copyAddressBlock).hide();
            $(invoiceDetailsCollapse).collapse('show');
            $(shippingArticle)
                .removeClass("d-flex")
                .addClass("d-none");
            $(basketTotalSection).text(toNumber(basketTotalWithoutShipping));
        }            
    }
}

function ShowInvoiceSection() {
    if (this.checked) {
        if (this.id === 'CopyInvoiceFromAddress@true') {
            $(invoiceDetailsCollapse).collapse('hide');
        }
        else if (this.id === 'CopyInvoiceFromAddress@false') {
            $(invoiceDetailsCollapse).collapse('show');
        }
    }
}

function SetWantInvoiceValue() {
    iptWantInvoice.value = this.wantInvoiceValue;
}

function addDeliveryRadioChangeEventListener() {
    for (var i = 0; i < deliveryRadios.length; i++) {
        deliveryRadios[i].addEventListener('click', ShowDeliverySection, false);
    }
}

function addCopyInvoiceAddressRadioChangeEventListener() {
    for (var i = 0; i < copyInvoiceRadios.length; i++) {
        copyInvoiceRadios[i].addEventListener('click', ShowInvoiceSection, false);
    }

}

function addWantInvoiceButtonsClickListener() {
    btnWantInvoice.addEventListener('click', SetWantInvoiceValue, false);
    btnWantInvoice.wantInvoiceValue = true;
    btnCancelInvoice.addEventListener('click', SetWantInvoiceValue, false);
    btnCancelInvoice.wantInvoiceValue = false;
}

addDeliveryRadioChangeEventListener();
addCopyInvoiceAddressRadioChangeEventListener();
addWantInvoiceButtonsClickListener();