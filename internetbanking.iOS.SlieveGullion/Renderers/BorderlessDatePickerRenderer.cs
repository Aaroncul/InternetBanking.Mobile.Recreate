﻿using InternetBanking.Controls;
using InternetBanking.iOS.SlieveGullion.Renderers;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessDatePicker), typeof(BorderlessDatePickerRenderer))]
namespace InternetBanking.iOS.SlieveGullion.Renderers
{
    public class BorderlessDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null)
            {
                Control.Layer.BorderWidth = 0;
                Control.BorderStyle = UITextBorderStyle.None;
            }
        }
    }
}
