using Injecter;
using SampleLogic;
using System;

namespace XamarinSample
{
    public partial class MainPage
    {
        [Inject] public ICounter ViewModel { get; } = default!;

        public MainPage()
        {
            InitializeComponent();

            BindingContext = ViewModel;
        }

        private void OnIncrementClicked(object _, EventArgs __) => ViewModel.Increment();
    }
}
