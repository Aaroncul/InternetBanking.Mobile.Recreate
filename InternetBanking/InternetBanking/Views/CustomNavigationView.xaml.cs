﻿using Xamarin.Forms;

namespace InternetBanking.Views
{
    public partial class CustomNavigationView : NavigationPage
    {
        public CustomNavigationView()
        {
            InitializeComponent();
        }

        public CustomNavigationView(Page root)
            : base(root)
        {
            InitializeComponent();
        }
    }
}
