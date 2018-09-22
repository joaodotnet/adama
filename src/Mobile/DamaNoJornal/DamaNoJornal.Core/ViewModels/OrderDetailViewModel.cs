using DamaNoJornal.Core.Models.Orders;
using DamaNoJornal.Core.Services.Order;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Services.User;
using DamaNoJornal.Core.ViewModels.Base;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IOrderService _ordersService;
        private IUserService _userService;

        private Order _order;
        private bool _isSubmittedOrder;
        private string _orderStatusText;
        private bool _isInvoiceCreated;
        private bool _createInvoice;
        private string _errors;
        private bool _withNif;
        private bool _nameIsEnabled;
        private bool _taxNumberIsEnabled;
        private bool _customerEmailIsEnabled;
        private bool _streetIsEnabled;
        private bool _postalCodeIsEnabled;
        private bool _cityIsEnabled;

        public OrderDetailViewModel(ISettingsService settingsService, 
            IOrderService ordersService, 
            IUserService userService)
        {
            _settingsService = settingsService;
            _ordersService = ordersService;
            _userService = userService;
        }

        public Order Order
        {
            get => _order;
            set
            {
                _order = value;
                RaisePropertyChanged(() => Order);
            }
        }

        public bool IsSubmittedOrder
        {
            get => _isSubmittedOrder;
            set
            {
                _isSubmittedOrder = value;
                RaisePropertyChanged(() => IsSubmittedOrder);
            }
        }

        public string OrderStatusText
        {
            get => _orderStatusText;
            set
            {
                _orderStatusText = value;
                RaisePropertyChanged(() => OrderStatusText);
            }
        }

        public bool IsInvoiceCreated
        {
            get => _isInvoiceCreated;
            set
            {
                _isInvoiceCreated = value;
                RaisePropertyChanged(() => IsInvoiceCreated);
            }
        }

        public bool CreateInvoice
        {
            get => _createInvoice;
            set
            {
                _createInvoice = value;

                RaisePropertyChanged(() => CreateInvoice);
            }
        }

        public bool WithNIF
        {
            get => _withNif;
            set
            {
                _withNif = value;

                RaisePropertyChanged(() => WithNIF);
            }
        }
        public bool NameIsEnabled
        {
            get => _nameIsEnabled;
            set
            {
                _nameIsEnabled = value;

                RaisePropertyChanged(() => NameIsEnabled);
            }
        }

        public bool TaxNumberIsEnabled
        {
            get => _taxNumberIsEnabled;
            set
            {
                _taxNumberIsEnabled = value;

                RaisePropertyChanged(() => TaxNumberIsEnabled);
            }
        }

        public bool CustomerEmailIsEnabled
        {
            get => _customerEmailIsEnabled;
            set
            {
                _customerEmailIsEnabled = value;

                RaisePropertyChanged(() => CustomerEmailIsEnabled);
            }
        }

        public bool StreetIsEnabled
        {
            get => _streetIsEnabled;
            set
            {
                _streetIsEnabled = value;

                RaisePropertyChanged(() => StreetIsEnabled);
            }
        }

        public bool PostalCodeIsEnabled
        {
            get => _postalCodeIsEnabled;
            set
            {
                _postalCodeIsEnabled = value;

                RaisePropertyChanged(() => PostalCodeIsEnabled);
            }
        }
        public bool CityIsEnabled
        {
            get => _cityIsEnabled;
            set
            {
                _cityIsEnabled = value;

                RaisePropertyChanged(() => CityIsEnabled);
            }
        }

        public string Errors
        {
            get { return _errors; }
            set
            {
                _errors = value;
                RaisePropertyChanged(() => Errors);
            }
        }


        public ICommand ToggleCancelOrderCommand => new Command(async () => await ToggleCancelOrderAsync());
        public ICommand CheckInvoiceCommand => new Command(CheckInvoiceToggle);
        public ICommand WithNifCommand => new Command(WithNifToggle);
        public ICommand CreateInvoiceCommand => new Command(async () => await CreateInvoiceAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is Order)
            {
                IsBusy = true;

                var order = navigationData as Order;

                // Get order detail info
                var authToken = _settingsService.AuthAccessToken;
                Order = await _ordersService.GetOrderAsync(order.OrderNumber, authToken);
                IsSubmittedOrder = Order.OrderStatus == OrderStatus.SUBMITTED;
                IsInvoiceCreated = !string.IsNullOrEmpty(Order.SalesInvoiceNumber);
                OrderStatusText = Order.OrderStatus.ToString().ToUpper();

                IsBusy = false;
            }
        }

        private async Task ToggleCancelOrderAsync()
        {
            var responseOk = await DialogService.ShowDialogAsync("Tem a certeza que quer cancelar a encomenda?", "Cancelar Encomenda", "Sim", "Não");
            if(responseOk)
            {
                var authToken = _settingsService.AuthAccessToken;

                var result = await _ordersService.CancelOrderAsync(_order.OrderNumber, authToken);

                if (result)
                {
                    OrderStatusText = OrderStatus.CANCELED.ToString().ToUpper();
                }
                else
                {
                    Order = await _ordersService.GetOrderAsync(Order.OrderNumber, authToken);
                    OrderStatusText = Order.OrderStatus.ToString().ToUpper();
                }

                IsSubmittedOrder = false;
            }            
        }

        private void CheckInvoiceToggle()
        {
            if (CreateInvoice)
            {
                WithNIF = true;
                EnabledControls(true, true);
            }
            else
            {
                WithNIF = false;
                EnabledControls(false, false);
            }
        }

        private void WithNifToggle()
        {
            if (!WithNIF)
            {
                EnabledControls(false, true);
            }
            else
            {
                if (CreateInvoice)
                    EnabledControls(true, true);
                else
                    EnabledControls(true, false);
            }
        }

        private void EnabledControls(bool enable, bool customerEmailEnable)
        {
            NameIsEnabled = enable;
            TaxNumberIsEnabled = enable;
            StreetIsEnabled = enable;
            PostalCodeIsEnabled = enable;
            CityIsEnabled = enable;
            CustomerEmailIsEnabled = customerEmailEnable;
        }

        private async Task CreateInvoiceAsync()
        {
            //Validate Form
            Errors = "";
            if (CreateInvoice && WithNIF)
            {
                if (string.IsNullOrEmpty(Order.BillingName))
                    Errors = "Para faturas com NIF, o nome do cliente é obrigatório.\r";
                if (string.IsNullOrEmpty(Order.BillingStreet))
                    Errors += "Para faturas com NIF, a morada é obrigatória.\r";
                if (string.IsNullOrEmpty(Order.BillingPostalCode))
                    Errors += "Para faturas com NIF, o código postal é obrigatório.\r";
                else if (!Regex.Match(Order.BillingPostalCode, "^\\d{4}-\\d{3}$", RegexOptions.IgnoreCase).Success)
                    Errors += "Para faturas com NIF, o código postal tem que estar no formato XXXX-XXX\r";
                if (string.IsNullOrEmpty(Order.BillingCity))
                    Errors += "Para faturas com NIF, a cidade é obrigatória";
            }
            if (!string.IsNullOrEmpty(Errors))
                await DialogService.ShowAlertAsync($"Existe erros no pedido, por favor corrija", "Erro", "Ok");
            else
            {
                if(!WithNIF)
                    Order.TaxNumber = null;                    
                try
                {
                    if (CreateInvoice)
                    {
                        var authToken = _settingsService.AuthAccessToken;
                        
                        Order.CreateInvoice = true;
                        var result = await _ordersService.CreateInvoiceOrderAsync(Order, authToken);                       

                        // Show Dialog
                        await DialogService.ShowAlertAsync(result.ResultMessage, "Geração de Fatura", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    await DialogService.ShowAlertAsync($"Ocorreu um erro: {ex.Message}", "Oops!", "Ok");
                }
            }
        }
    }
}