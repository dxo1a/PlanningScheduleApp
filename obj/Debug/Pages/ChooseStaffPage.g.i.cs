﻿#pragma checksum "..\..\..\Pages\ChooseStaffPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9A514CE5240E5431EB6F975BEFB5345F71454310886F7451418346E9D378CF63"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using MathCore.WPF;
using MathCore.WPF.Commands;
using MathCore.WPF.Converters;
using MathCore.WPF.UIEvents;
using PlanningScheduleApp.Pages;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace PlanningScheduleApp.Pages {
    
    
    /// <summary>
    /// ChooseStaffPage
    /// </summary>
    public partial class ChooseStaffPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\Pages\ChooseStaffPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel ChooseDepSP;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\Pages\ChooseStaffPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SearchDepTBX;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\Pages\ChooseStaffPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView DepLV;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Pages\ChooseStaffPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ClearBtn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PlanningScheduleApp;component/pages/choosestaffpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Pages\ChooseStaffPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ChooseDepSP = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.SearchDepTBX = ((System.Windows.Controls.TextBox)(target));
            
            #line 15 "..\..\..\Pages\ChooseStaffPage.xaml"
            this.SearchDepTBX.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.SearchDepTBX_TextChanged);
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\Pages\ChooseStaffPage.xaml"
            this.SearchDepTBX.GotFocus += new System.Windows.RoutedEventHandler(this.SearchDepTBX_GotFocus);
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\Pages\ChooseStaffPage.xaml"
            this.SearchDepTBX.LostFocus += new System.Windows.RoutedEventHandler(this.SearchDepTBX_LostFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.DepLV = ((System.Windows.Controls.ListView)(target));
            
            #line 16 "..\..\..\Pages\ChooseStaffPage.xaml"
            this.DepLV.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DepLV_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ClearBtn = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\..\Pages\ChooseStaffPage.xaml"
            this.ClearBtn.Click += new System.Windows.RoutedEventHandler(this.ClearBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

