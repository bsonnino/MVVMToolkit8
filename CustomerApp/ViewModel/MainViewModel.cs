using System;
using System.Collections.Generic;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomerLib;

namespace CustomerApp.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ICustomerRepository _customerRepository;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
        private Customer _selectedCustomer;

        public MainViewModel(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ??
                                  throw new ArgumentNullException("customerRepository");
        }

        public IEnumerable<Customer> Customers => _customerRepository.Customers;

        [RelayCommand]
        private void Add()
        {
            var customer = new Customer();
            _customerRepository.Add(customer);
            SelectedCustomer = customer;
            OnPropertyChanged("Customers");
        }

        [RelayCommand(CanExecute = "HasSelectedCustomer")]
        private void Remove()
        {
            if (SelectedCustomer != null)
            {
                _customerRepository.Remove(SelectedCustomer);
                SelectedCustomer = null;
                OnPropertyChanged("Customers");
            }
        }

        private bool HasSelectedCustomer() => SelectedCustomer != null;

        [RelayCommand]
        private void Save()
        {
            _customerRepository.Commit();
        }

        [RelayCommand]
        private void Search(string textToSearch)
        {
            var coll = CollectionViewSource.GetDefaultView(Customers);
            if (!string.IsNullOrWhiteSpace(textToSearch))
                coll.Filter = c => ((Customer)c).Country.ToLower().Contains(textToSearch.ToLower());
            else
                coll.Filter = null;
        }
    }
}