var btnSubscribeNewsletter = document.getElementsByClassName("subscribeNewsLetter");

$(document).ready(function () {
    $('body').on('mouseenter mouseleave', '.dropdown', function (e) {
        var _d = $(e.target).closest('.dropdown');
        _d.addClass('show');
        setTimeout(function () {
            _d[_d.is(':hover') ? 'addClass' : 'removeClass']('show');
        }, 1000);
    });

    $('#instagram-feed').on('click', function (e) {
        window.open("https://www.instagram.com/damanojornal/");
    });

    $('#newslettercollapse').on('show.bs.collapse', function () {
        $('#searchcollapse').collapse('hide');
    });
    $('#searchcollapse').on('show.bs.collapse', function () {
        $('#newslettercollapse').collapse('hide');
    });

    $('.add-product').on('click', function (e) {
        e.preventDefault();
        $('#product-added-img').empty();
        $('#product-added-name').empty();
        $('#product-added-price').empty();
        $('#basket-products').text('?');
        $('#basket-total').text('?');
        var formObj = $(this).closest('form');
        $('#product-added-img').append(formObj.find('img').clone());
        $('#product-added-name').append(formObj.find('.esh-catalog-name span').clone());
        $('#product-added-price').append(formObj.find('.dnj-catalog-price span').clone());
        $('#productAddedModal').modal('show');        
        $.post('/Basket/Index', formObj.serialize(), function (data) {
            $('#basket-products').text(data.items);
            $('#basket-total').text(toNumber(data.total));
        });
    });
});

function GetSliderBasicOptions(glideType) {
    return {
        type: glideType,
        startAt: 0,
        perView: 5,
        breakpoints: {
            544: { perView: 2 }
        }
    };
}

function toNumber(value) {
    return parseFloat(value).toFixed(2).replace('.', ',');
}

function ValidateNewsletter(e) {
    var iptNewsletterEmail = $(this).closest('form').find('input[name=newsletterEmail]');
    var chkNewsletterTerms = $(this).closest('form').find('input[name=newsletterTerms]');
    var spnErrorMessage = $(this).closest('form').find('.errorNewsletterSubscribeMsg');
    if (!IsEmailValid(iptNewsletterEmail.val())) {
        spnErrorMessage.html("O email que introduziu está inválido!");
        e.preventDefault();
    }
    else if (!chkNewsletterTerms.is(':checked')) {
        spnErrorMessage.html("Tem que aceitar os termos de Política de Privacidade");
        e.preventDefault();
    }
    else
        spnErrorMessage.html("");
}

function IsEmailValid(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

function addBtnSubscribeClickEventListener() {
    for (var i = 0; i < btnSubscribeNewsletter.length; i++) {
        btnSubscribeNewsletter[i].addEventListener("click", ValidateNewsletter, false);
    }
}
addBtnSubscribeClickEventListener();
