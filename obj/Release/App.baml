   M S B A M L   `   `   `  ���� N  JPlanningScheduleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:PlanningScheduleAppPlanningScheduleApp  W SStyledWindow.WPF, Version=7.0.0.0, Culture=neutral, PublicKeyToken=9ae6b54700176980[?clr-namespace:StyledWindow.WPF.Themes;assembly=StyledWindow.WPFStyledWindow.WPF.Themes cCclr-namespace:StyledWindow.WPF.Components;assembly=StyledWindow.WPFStyledWindow.WPF.Components O,clr-namespace:PlanningScheduleApp.ConvertersPlanningScheduleApp.Converters  O,clr-namespace:PlanningScheduleApp.ConvertersPlanningScheduleApp.Converters      PlanningScheduleApp.App   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PMathCore.WPF, Version=0.0.44.5, Culture=neutral, PublicKeyToken=38be1b026bbf82a9W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     3local!clr-namespace:PlanningScheduleApp     _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Lthemes?clr-namespace:StyledWindow.WPF.Themes;assembly=StyledWindow.WPF  T
componentsCclr-namespace:StyledWindow.WPF.Components;assembly=StyledWindow.WPF  9local1,clr-namespace:PlanningScheduleApp.Converters  =
converters,clr-namespace:PlanningScheduleApp.Converters    �� 	Resources   �� MergedDictionaries+  %MaterialDesignThemes.Wpf.BundledTheme   	BaseTheme   PrimaryColor   SecondaryColor   ColorAdjustment.  (MaterialDesignThemes.Wpf.ColorAdjustment �� Source7   1PlanningScheduleApp.Converters.DayOfWeekConverter   DayOfWeekConverter9   3PlanningScheduleApp.Converters.DateWithDayConverter  DateWithDayConverter?   9PlanningScheduleApp.Converters.ZeroToEmptyStringConverter  ZeroToEmptyStringConverter5   /PlanningScheduleApp.Converters.MinutesConverter  MinutesConverter>  8PlanningScheduleApp.Converters.IsStringNonemptyConverter  ToolTipVisibilityConveter3  -System.Windows.Media.Effects.DropShadowEffect  ShadowEffect   
BlurRadius   	Direction	   ShadowDepth
   Color   Opacity  ShadowEffectDown  ShadowEffectUp  shadForText 	 shadForTextOnLight 
 ShadowEffectForIcons-	  'System.Windows.Media.Effects.BlurEffect  
BlurEffect 	  Radius 	  
KernelType  ClearButton �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value  DataGridOwn&
   System.Windows.Controls.DataGrid=  7System.Windows.Controls.Primitives.DataGridColumnHeader)  #System.Windows.Controls.DataGridRow*  $System.Windows.Controls.DataGridCell �� 
TargetType �� Padding J� SnapsToDevicePixels  RoundedButtonStyle ��Name �� CornerRadius �� Triggers �� 
TargetName  UpRoundedButtonStyle  RightRoundedButtonStyle  TBXStyle  MaterialDesignTextBox �� BasedOn.  (MaterialDesignThemes.Wpf.TextFieldAssist ��Name  ShowTextboxAnimation �� Binding �� Value N� EnterActions  �� TargetProperty! W� From	" W� To# g� Duration$ N� ExitActions  VerticalSeparatorStyle% �� Angle  StandardBorderBrush& �� Color  StandardBackgroundBrush  HoverBorderBrush  SelectedBackgroundBrush  SelectedForegroundBrush  DisabledForegroundBrush  
GlyphBrush  NormalBrush  NormalBorderBrush  HorizontalNormalBrush  HorizontalNormalBorderBrush   ListBoxBackgroundBrush' �� 
StartPoint( �� EndPoint) � Color* � Offset ! StandardBrush " PressedBrush # ScrollBarLineButton+ �� Path, �� RelativeSource $ ScrollBarPageButton % ScrollBarThumb & ScrollBarThumbHorizontal ' VerticalScrollBar- � RowDefinitions. ]� DecreaseRepeatButton/ ]� Thumb0 ]� IncreaseRepeatButton ( HorizontalScrollBar1 � ColumnDefinitions ) FavsScrollViewer2 �� VerticalOffset3 � Value4 �� ScrollableHeight5 � Maximum6 �� ViewportHeight7 �� ViewportSize*8 �� #ComputedVerticalScrollBarVisibility9 � Triggers: :� RoutedEvent; �� ElementName< �� Orientation= �� HorizontalOffset> �� ScrollableWidth? �� ViewportWidth,@ �� %ComputedHorizontalScrollBarVisibility * TextBoxWithToolTipStyleA ��Name
B � TagC �� PaddingD �� 	Converter�� H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml        $ InheritX�$
 GreyX�$ 	LightBlueX�   �� $K Epack://application:,,,/StyledWindow.WPF;component/Themes/Generic.xaml?�%;"  &	        &	      &	 
     &	      &	      &	      &	 O     &	 �     &	 �     &		 �     &	
     &	 9    &	 V    &	 �    (�� �    *ToolBar.SeparatorStyleKey+)&	 +    &	 \	    &	 c    &	 �    7  '�� �    &	 ]    7  &	 +    7��&	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	     &	     &	 '    &	 <    &	  L    &	! �    &	"     &	# �    7  7  7  7 " 7  &	$ B    &	%     &	& �    &	' �    7 # 7 $ 7 % 7  7  7 $ 7 # &	( �    7 # 7 $ 7 & 7  7  7 $ 7 # '�� ;    7 ( 7 ' &	) v    &	* �    7  7  7  7              $ 15Q�$	 310Q�$	 8Q�$
 Black��$	 0.8Q�  $	 5Q�$	 240Q�$ 10Q�$ 1Q�$

 Gray��  $	 5Q�$ 90Q�$ 10Q�$ 1Q�$

 Gray��  $ 3Q�$ 0Q�$	 1Q�$
 Black��  $ 1Q�$ 0Q�$	 1Q�$
 Black��  $ 3Q�$ 0Q�$	 0Q�$
 Black��$	 0.8Q�	  $ 10Q�$ Gaussian=��� " �� P�  �@J�IsMouseOver . . ���  � ��
 �����P�  �@J�IsMouseOver .  . ���  � ��
 ���� �� " 
 .���  � �� �#DDDDDD��   �@
 HorizontalGridLinesBrush
 ���� ��  �@
 VerticalGridLinesBrush
 �������  �@
 RowHeaderWidth$ 0Q���  �@
 ColumnHeaderStyle �� "  .���  � ��
 �������  � ��
 �   ���  � ��$ 0.5 0q���  � ��$ 5 4 0 4q���  � ��$ SemiBold"� P�  �@J�IsMouseOver . . ���  � ��
 �������  �@
 RowStyle �� "  .���  � ��$	 150����  � ��
 ���� ��  � ��$ 0 0 0 0q� P�  �@J�IsMouseOver . . ���  � ��
 �������  � ��$ 0 1 0 1q���  � ��
 ���� ��  � ��$ 13(���  �@
 	CellStyle �� "   P�  �@ 
IsSelected . . ���  � �� �#d7d7d7��  � �� �#d7d7d7��  � ��
 �   ���  � ��$ 13(���  � ��$ SemiBold"�P�  �@J�	IsFocused . . ���  � �� �#d7d7d7��  � �� �#d7d7d7��  � ��$ 1q���  � ��
 �   �.���  � ��$ 0,3,0,3q���  � ��
 ���� ��  � ��$ 1q���  � �� �� "  .R��� # z��#��z��#��z��._��� ��  �@�FocusVisualStyle �� " ��.���  � ��
 ���� �� " ��.���  � ��# � ����  � ��$ 0,2,0,2q���  � |� .  ��  � �� �� " ��.R��� #��z��#��z��#��z��$��1�� . �� " ��.���  � �� �� " ��.R��� $ ButtonBorder��#��z��#��z��#��z��$ 10 10 10 10��._��� $��Center=�$��Center=� P�  � {� .  . ���  � ��
 ���� ��  � ��
 �   ���  �@J�Opacity$	 0.4Q���  � ��
 ���� P�  � {� . . ���  � ��
 ���� ��  � ��
 �   ���  �@J�Opacity$ 1Q���  � ��
 ���� P�  �@J�IsMouseOver . . ��� $ ButtonBorder�� � ��
 ������� $ ButtonBorder�� � ��
 ������� $ ButtonBorder�� � ��$ 2 2 2 2q��� " ��.���  � �� �� " ��.R��� $ ButtonBorder��#��z��#��z��#��z��$ 	10 10 0 0��._��� $��Center=�$��Center=� P�  � {� .  . ���  � ��
 ���� ��  � ��
 �   ���  �@J�Opacity$	 0.4Q�P�  � {� . . ���  � ��
 ���� ��  � ��
 �   ���  �@J�Opacity$ 1Q�P�  �@J�IsMouseOver . . ��� $ ButtonBorder�� � �� �#F0F0F0�� $ ButtonBorder�� � �� �#DDDDDD�� $ ButtonBorder�� � ��$ 2 2 2 0q��� " ��.���  � �� �� " ��.R��� $ ButtonBorder��#��z��#��z��#��z��$ 	0 10 10 0��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ��� $ ButtonBorder�� � ��
 ������� $ ButtonBorder�� � �� �	#FF4C4C4C�� $ ButtonBorder�� � ��$ 0q��� $ ButtonBorder�� �@�Cursor$
 Hand��P�  �@J�IsMouseOver .  . ��� $ ButtonBorder�� � ��
 ������� $ ButtonBorder�� � �� �	#FF4C4C4C�� $ ButtonBorder�� � ��$ 0q�P�  � {� .  . ���  � ��
 �������  � ��
 �   ��� " ��8   .���  � ��$ 22����  � ��$ 	10 5 35 0q���  � ��
 �   ���  � ��
 �������  �@ CharacterCounterStyle �� " ����  �@ TextFieldCornerRadius$ 5����  �@=�ErrorTemplate �� �� " ��.���  � �� �� " ��.R��� #��z��#��z��#��z�� . ._��� $ PART_ContentHost�� P�  �@J�IsMouseOver . . ���  � �� �	#FF4C4C4C�� " ��8    ��  �� *IsDetailSearchVisible+ True �� .���� .��W� $  Width�$! 0X�$	" 220X�$# 0:0:0.3D�$ �� .���� .��W� $  Width�$	! 220X�$" 0X�$# 0:0:0.3D��� " ��8   .���  � ��$ 5,0,0,0q���  �@�LayoutTransform V� ~�X� �� $% 90Q��� $
& #888���� $& Black���� $
& #DDD���� $
& Gray���� $& White���� $
& #888���� $
& #444���� $
& #888���� $
& #888���� $& 	#FF686868���� $
& #888���� $	' 0,0&�$( 1,0.001&���� � $) White��$	* 0.0Q�� $) White��$	* 0.6Q�� $) #DDDDDD��$	* 1.2Q��� $	' 0,0&�$	( 0,1&���� � $
) #FFF��$	* 0.0Q�� $
) #CCC��$	* 1.0Q��� $	' 0,0&�$	( 0,1&���� � $
) #BBB��$	* 0.0Q�� $
) #EEE��$	* 0.1Q�� $
) #EEE��$	* 0.9Q�� $
) #FFF��$	* 1.0Q��� " ��.���  � y�$ Hidden=���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � |� .  ��  � �� �� " ��.R��� $��Border��$��1q�$ 2��8��  8�� $��1q�._�F� $��Center=�$��Center=�8�� ���� $+ Content�, �� *TemplatedParent+ P�  � �� . . ��� $ Border��8   � ��P�  � {� .  . ��� 8   � ���� " ��.���  � y�$ Hidden=���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " ��.R��� 
���   ��� " n�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " n�.R��� $ 0 0 0 0��#��z��#��z��$��0q�$
��Auto��$��	0,-2,0,-2q��� " n�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " n�.R��� $ 0 0 0 0��#��z��#��z��$��0q�$
��Auto��$��0,0,0,0q��� " ��.R�� - �� $��0���� $��0.00001* ��� $��0��.+��� $��3��$ 2��
������ �� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����. . �� 8�� $��ScrollBar.PageUpCommand��/ n� 8�� $��0,0,-1,0q�8�� 8�� 0 �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� " ��.R�� 1 �� $��0.00001* �.+��� $��3��$ 2�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineLeftCommand��$��M 4 0 L 4 8 L 0 4 Z��]� $��
PART_Track��$��1����.  . �� 8�� $��ScrollBar.PageLeftCommand��/ n� 8�� $��0,0,0,-1q�8�� 8�� 0 �� 8�� $ ��ScrollBar.PageRightCommand���� $��3��8�� $��18��$ ��ScrollBar.LineRightCommand��$��M 0 0 L 4 4 L 0 8 Z���� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle .  P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8   P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  �� " ��.���  �@�OverridesDefaultStyle . ��  � �� �� " ��.R�� 1 �� $
��Auto ��� - �� �� $
��Auto �.+��� $��1���� $��PART_VerticalScrollBar��#3 z2 #5 z4 #7 z6 #y�z8 9 :� $: ScrollBar.Scroll��.[��� .���� .��W� $  Value�$# 0:0:2D�" �� *Value+$; PART_VerticalScrollBar���� $��PART_HorizontalScrollBar��$< 
Horizontal=�$��1��$��1��#3 z= #5 z> #7 z? #y�z@ 9 :� $: ScrollBar.Scroll��.[��� .���� .��W� $  Value�$# 0:0:0.5D�" �� *Value+$; PART_HorizontalScrollBar���� " ��8   .���  � ��$ 0 5 0 0q���  � �� �� " ��.R��� #��z��#��z��._�� .+��� $ PART_ContentHost��$��2q��� $A PlaceholderText��#��zB $y�	Collapsed=�$��Center=�
�������$	C 5 0q� �� $
 True�� �� *Text+, �� *Self+8D  .a��� $ PlaceholderText�� � y�$ Visible=��� $ False�� �� *Text+, �� *Self+8D  .a��� $ PlaceholderText�� � y�$ 	Collapsed=� ��  �� *Text+, �� *Self+8D   True.a���  �@�ToolTip$ Введите текст��