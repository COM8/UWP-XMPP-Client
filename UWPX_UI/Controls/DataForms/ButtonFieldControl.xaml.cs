﻿using UWPX_UI_Context.Classes.DataTemplates.Controls.IoT;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.XML.Messages.XEP_0336;

namespace UWPX_UI.Controls.DataForms
{
    public sealed partial class ButtonFieldControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public FieldDataTemplate Field
        {
            get => (FieldDataTemplate)GetValue(FieldProperty);
            set => SetValue(FieldProperty, value);
        }
        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register(nameof(Field), typeof(FieldDataTemplate), typeof(ButtonFieldControl), new PropertyMetadata(null, OnFieldChanged));

        private bool supressValueChanged;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ButtonFieldControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            // Update subscriptions:
            if (e.OldValue is FieldDataTemplate oldField)
            {
                oldField.PropertyChanged -= Field_PropertyChanged;
            }
            if (e.NewValue is FieldDataTemplate newField)
            {
                newField.PropertyChanged += Field_PropertyChanged;
            }
            UpdateUi();
        }

        private void UpdateUi()
        {
            supressValueChanged = true;
            Visibility = Field is null ? Visibility.Collapsed : Visibility.Visible;
            if (!(Field is null))
            {
                // General:
                button.Content = Field.Label;

                // Options:
                button.IsEnabled = !Field.Field.dfConfiguration.flags.HasFlag(DynamicFormsFlags.READ_ONLY);
            }
            supressValueChanged = false;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ButtonFieldControl control)
            {
                control.UpdateView(e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!supressValueChanged)
            {
                // Make sure we change the value of this field to enfore a button press:
                Field.Value = ((Field.Value is string s) && (s == "1")) ? "2" : "1";
                Field.OnValueChangedByUser();
            }
        }

        private void Field_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateUi();
        }

        #endregion
    }
}
