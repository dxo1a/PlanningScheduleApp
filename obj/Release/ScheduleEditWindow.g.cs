﻿#pragma checksum "..\..\ScheduleEditWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3B68BAE39061FB85E1E4042283BFE2E2F3C4760F7BBDD4F8FDC2FE4FF55E0F06"
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
    /// ScheduleEditWindow
    /// </summary>
    public partial class ScheduleEditWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 52 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TemplateNameTBX;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TemplateAdditionalNameTBX;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl ScheduleTC;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabItem StaticScheduleTI;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl StaticDaysIC;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SaveFixedTemplateBtn;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabItem FlexibleScheduleTI;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox WorkingDaysCountCMB;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox RestingDaysCountCMB;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl FlexibleDaysIC;
        
        #line default
        #line hidden
        
        
        #line 137 "..\..\ScheduleEditWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SaveFlexibleTemplateBtn;
        
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
            System.Uri resourceLocater = new System.Uri("/PlanningScheduleApp;component/scheduleeditwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ScheduleEditWindow.xaml"
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
            this.ScheduleTC = ((System.Windows.Controls.TabControl)(target));
            return;
            case 4:
            this.StaticScheduleTI = ((System.Windows.Controls.TabItem)(target));
            return;
            case 5:
            this.StaticDaysIC = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 11:
            this.SaveFixedTemplateBtn = ((System.Windows.Controls.Button)(target));
            
            #line 86 "..\..\ScheduleEditWindow.xaml"
            this.SaveFixedTemplateBtn.Click += new System.Windows.RoutedEventHandler(this.SaveFixedTemplateBtn_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.FlexibleScheduleTI = ((System.Windows.Controls.TabItem)(target));
            return;
            case 13:
            this.WorkingDaysCountCMB = ((System.Windows.Controls.ComboBox)(target));
            
            #line 94 "..\..\ScheduleEditWindow.xaml"
            this.WorkingDaysCountCMB.DropDownClosed += new System.EventHandler(this.WorkingDaysCountCMB_DropDownClosed);
            
            #line default
            #line hidden
            return;
            case 14:
            this.RestingDaysCountCMB = ((System.Windows.Controls.ComboBox)(target));
            
            #line 98 "..\..\ScheduleEditWindow.xaml"
            this.RestingDaysCountCMB.DropDownClosed += new System.EventHandler(this.RestingDaysCountCMB_DropDownClosed);
            
            #line default
            #line hidden
            return;
            case 15:
            this.FlexibleDaysIC = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 20:
            this.SaveFlexibleTemplateBtn = ((System.Windows.Controls.Button)(target));
            
            #line 137 "..\..\ScheduleEditWindow.xaml"
            this.SaveFlexibleTemplateBtn.Click += new System.Windows.RoutedEventHandler(this.SaveFlexibleTemplateBtn_Click);
            
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
            case 6:
            
            #line 67 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.MTBX_LostFocus);
            
            #line default
            #line hidden
            
            #line 67 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 67 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 67 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.MaskedTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 67 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 7:
            
            #line 69 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.MTBX_LostFocus);
            
            #line default
            #line hidden
            
            #line 69 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 69 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 69 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.MaskedTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 69 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 8:
            
            #line 75 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 75 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 75 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 9:
            
            #line 77 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 77 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 77 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 10:
            
            #line 80 "..\..\ScheduleEditWindow.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Checked += new System.Windows.RoutedEventHandler(this.isRestingDayCB_Checked);
            
            #line default
            #line hidden
            break;
            case 16:
            
            #line 110 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.MTBX_LostFocus);
            
            #line default
            #line hidden
            
            #line 110 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 110 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 110 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.MaskedTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 110 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 17:
            
            #line 116 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.MTBX_LostFocus);
            
            #line default
            #line hidden
            
            #line 116 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 116 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 116 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.MaskedTextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 116 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 18:
            
            #line 126 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 126 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 126 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            case 19:
            
            #line 128 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.TBX_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 128 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TBX_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 128 "..\..\ScheduleEditWindow.xaml"
            ((Xceed.Wpf.Toolkit.MaskedTextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.MTBX_KeyDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

