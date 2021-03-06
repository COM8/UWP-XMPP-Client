﻿using Shared.Classes;
using Windows.UI.Xaml.Media;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class ComplianceTesterBadgeControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private Brush _BadgeBrush;
        public Brush BadgeBrush
        {
            get => _BadgeBrush;
            set => SetProperty(ref _BadgeBrush, value);
        }

        private string _RatingText;
        public string RatingText
        {
            get => _RatingText;
            set => SetProperty(ref _RatingText, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
