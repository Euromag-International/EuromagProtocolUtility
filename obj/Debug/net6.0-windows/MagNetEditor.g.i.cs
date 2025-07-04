﻿#pragma checksum "..\..\..\MagNetEditor.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2801EA51EFB2ECC81200C03ACD1E9A6718A3C6A2"
//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

using EuromagProtocolUtility;
using EuromagProtocolUtility.Tools;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace EuromagProtocolUtility {
    
    
    /// <summary>
    /// MagNetEditor
    /// </summary>
    public partial class MagNetEditor : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 36 "..\..\..\MagNetEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ImportButton;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\MagNetEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid parametersGrid;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\MagNetEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.Card MagnetDataCard;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\MagNetEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.Card TraduzioniCard;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\MagNetEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid I18NDescriptionsGrid;
        
        #line default
        #line hidden
        
        
        #line 144 "..\..\..\MagNetEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid WritingConditionGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.3.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/EuromagProtocolUtility;component/magneteditor.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MagNetEditor.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.3.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ImportButton = ((System.Windows.Controls.Button)(target));
            return;
            case 2:
            this.parametersGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 56 "..\..\..\MagNetEditor.xaml"
            this.parametersGrid.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.parametersGrid_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.MagnetDataCard = ((MaterialDesignThemes.Wpf.Card)(target));
            return;
            case 4:
            this.TraduzioniCard = ((MaterialDesignThemes.Wpf.Card)(target));
            return;
            case 5:
            this.I18NDescriptionsGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 6:
            this.WritingConditionGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

