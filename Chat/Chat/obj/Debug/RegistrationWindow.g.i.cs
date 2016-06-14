﻿#pragma checksum "..\..\RegistrationWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7CD9DF626D7E9F9745AC4C838B2E909C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Chat.Resources;
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


namespace Chat {
    
    
    /// <summary>
    /// RegistrationWindow
    /// </summary>
    public partial class RegistrationWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label LoginLabel;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Menu MyMenu;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem LanguageMI;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem EnglishMI;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem RussianMI;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem UkrainianMI;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem GermanMI;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LoginBox;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label PassLabel;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox PassBox1;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox PassBox2;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\RegistrationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RegButton;
        
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
            System.Uri resourceLocater = new System.Uri("/Chat;component/registrationwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\RegistrationWindow.xaml"
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
            
            #line 5 "..\..\RegistrationWindow.xaml"
            ((Chat.RegistrationWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            
            #line 5 "..\..\RegistrationWindow.xaml"
            ((Chat.RegistrationWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LoginLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.MyMenu = ((System.Windows.Controls.Menu)(target));
            return;
            case 4:
            this.LanguageMI = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 5:
            this.EnglishMI = ((System.Windows.Controls.MenuItem)(target));
            
            #line 14 "..\..\RegistrationWindow.xaml"
            this.EnglishMI.Click += new System.Windows.RoutedEventHandler(this.EnglishMI_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.RussianMI = ((System.Windows.Controls.MenuItem)(target));
            
            #line 15 "..\..\RegistrationWindow.xaml"
            this.RussianMI.Click += new System.Windows.RoutedEventHandler(this.RussianMI_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.UkrainianMI = ((System.Windows.Controls.MenuItem)(target));
            
            #line 16 "..\..\RegistrationWindow.xaml"
            this.UkrainianMI.Click += new System.Windows.RoutedEventHandler(this.UkrainianMI_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.GermanMI = ((System.Windows.Controls.MenuItem)(target));
            
            #line 17 "..\..\RegistrationWindow.xaml"
            this.GermanMI.Click += new System.Windows.RoutedEventHandler(this.GermanMI_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.LoginBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.PassLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 11:
            this.PassBox1 = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 22 "..\..\RegistrationWindow.xaml"
            this.PassBox1.LostFocus += new System.Windows.RoutedEventHandler(this.PassBox1_LostFocus);
            
            #line default
            #line hidden
            return;
            case 12:
            this.PassBox2 = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 23 "..\..\RegistrationWindow.xaml"
            this.PassBox2.LostFocus += new System.Windows.RoutedEventHandler(this.PassBox2_LostFocus);
            
            #line default
            #line hidden
            return;
            case 13:
            this.RegButton = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\RegistrationWindow.xaml"
            this.RegButton.Click += new System.Windows.RoutedEventHandler(this.RegButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

