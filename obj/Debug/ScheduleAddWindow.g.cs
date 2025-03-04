﻿#pragma checksum "..\..\ScheduleAddWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "0C5A1E57D1E65057ED4D764AB41FB153AC4B274D2CD9CAF2AAC9A16321ACE212"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using MathCore.WPF;
using MathCore.WPF.Commands;
using MathCore.WPF.Converters;
using MathCore.WPF.UIEvents;
using PlanningScheduleApp;
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Converters;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Mag.Converters;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace PlanningScheduleApp {
    
    
    /// <summary>
    /// ScheduleAddWindow
    /// </summary>
    public partial class ScheduleAddWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 52 "..\..\ScheduleAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TemplateNameTBX;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\ScheduleAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TemplateAdditionalNameTBX;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\ScheduleAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl ScheduleTB;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\ScheduleAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddFixedTemplateBtn;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\ScheduleAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox WorkingDaysCountCMB;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\ScheduleAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox RestingDaysCountCMB;
        
        #line default
        #line hidden
        
        
        #line 139 "..\..\ScheduleAddWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddFlexibleTemplateBtn;
        
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
            System.Uri resourceLocater = new System.Uri("/PlanningScheduleApp;component/scheduleaddwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ScheduleAddWindow.xaml"
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
            this.TemplateNameTBX = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.TemplateAdditionalNameTBX = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.ScheduleTB = ((System.Windows.Controls.TabControl)(target));
            return;
            case 9:
            this.AddFixedTemplateBtn = ((System.Windows.Controls.Button)(target));
            
            #line 89 "..\..\ScheduleAddWindow.xaml"
            this.AddFixedTemplateBtn.Click += new System.Windows.RoutedEventHandler(this.AddFixedTemplateBtn_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.WorkingDaysCountCMB = ((System.Windows.Controls.ComboBox)(target));
            
            #line 97 "..\..\ScheduleAddWindow.xaml"
            this.WorkingDaysCountCMB.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.WorkingDaysCountCMB_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.RestingDaysCountCMB = ((System.Windows.Controls.ComboBox)(target));
            
            #line 101 "..\..\ScheduleAddWindow.xaml"
            this.RestingDaysCountCMB.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.RestingDaysCountCMB_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 16:
            this.AddFlexibleTemplateBtn = ((System.Windows.Controls.Button)(target));
            
            #line 139 "..\..\ScheduleAddWindow.xaml"
            this.AddFlexibleTemplateBtn.Click += new System.Windows.RoutedEventHandler(this.AddFlexibleTemplateBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 4:
            
            #line 68 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.MTBX_LostFocus);
            
            #line default
            #line hidden
            
            #line 68 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 68 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 68 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.MaskedTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 68 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 5:
            
            #line 70 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.MTBX_LostFocus);
            
            #line default
            #line hidden
            
            #line 70 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 70 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 70 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.MaskedTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 70 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 6:
            
            #line 77 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 77 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 77 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 7:
            
            #line 79 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 79 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 79 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 8:
            
            #line 83 "..\..\ScheduleAddWindow.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Checked += new System.Windows.RoutedEventHandler(this.isRestingDayCB_Checked);
            
            #line default
            #line hidden
            
            #line 83 "..\..\ScheduleAddWindow.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Unchecked += new System.Windows.RoutedEventHandler(this.isRestingDayCB_Unchecked);
            
            #line default
            #line hidden
            break;
            case 12:
            
            #line 113 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.MTBX_LostFocus);
            
            #line default
            #line hidden
            
            #line 113 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 113 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 113 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.MaskedTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 113 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 13:
            
            #line 119 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.MTBX_LostFocus);
            
            #line default
            #line hidden
            
            #line 119 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 119 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 119 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.MaskedTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 119 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 14:
            
            #line 129 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 129 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 129 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 15:
            
            #line 131 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 131 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 131 "..\..\ScheduleAddWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

