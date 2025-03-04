﻿#pragma checksum "..\..\..\..\MVVM\View\ProcessWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B7A0912122243605F2BF87169F172C9F7DF4CA28A0478DA94657EDC546D514EC"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using FontAwesome.WPF;
using FontAwesome.WPF.Converters;
using MathCore.WPF;
using MathCore.WPF.Commands;
using MathCore.WPF.Converters;
using MathCore.WPF.UIEvents;
using PlanningScheduleApp.MVVM.View;
using PlanningScheduleApp.MVVM.ViewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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


namespace PlanningScheduleApp.MVVM.View {
    
    
    /// <summary>
    /// ProcessWindow
    /// </summary>
    public partial class ProcessWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\..\..\MVVM\View\ProcessWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TaskTB;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\MVVM\View\ProcessWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TaskName;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\..\MVVM\View\ProcessWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock CurrentProgressTB;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\..\MVVM\View\ProcessWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TotalProgressTB;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\MVVM\View\ProcessWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FontAwesome.WPF.ImageAwesome Spinner;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\MVVM\View\ProcessWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ResultTB;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\MVVM\View\ProcessWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OpenFolderBtn;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\MVVM\View\ProcessWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Bitrix24Export;
        
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
            System.Uri resourceLocater = new System.Uri("/PlanningScheduleApp;component/mvvm/view/processwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\MVVM\View\ProcessWindow.xaml"
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
            this.TaskTB = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.TaskName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.CurrentProgressTB = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.TotalProgressTB = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.Spinner = ((FontAwesome.WPF.ImageAwesome)(target));
            return;
            case 6:
            this.ResultTB = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.OpenFolderBtn = ((System.Windows.Controls.Button)(target));
            return;
            case 8:
            this.Bitrix24Export = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

