using System;
using System.ComponentModel;

namespace MaterialDesignThemes.UITests.Samples.DialogHost
{
    /// <summary>
    /// Interaction logic for WithDefaultButton.xaml
    /// </summary>
    public partial class WithDefaultButton
    {
        private WithDefaultButtonViewModel ViewModel { get; }
        public WithDefaultButton()
        {
            DataContext = ViewModel = new WithDefaultButtonViewModel();
            InitializeComponent();
        }

        private class WithDefaultButtonViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            private string? _InputText;
            public string? InputText
            {
                get => _InputText;
                set
                {
                    if (_InputText != value)
                    {
                        if (value == "123") throw new Exception();
                        _InputText = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputText)));
                    }
                }
            }
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = new WithDefaultButtonViewModel { InputText = ViewModel.InputText };
            await Wpf.DialogHost.Show(vm);
            ViewModel.InputText = vm.InputText;
        }
    }
}
