   M S B A M L   `   `   `  ���� N  JPlanningScheduleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:PlanningScheduleApp.PagesPlanningScheduleApp.Pages  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem /    )PlanningScheduleApp.Pages.SelectedDepPage   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PMathCore.WPF, Version=0.0.44.5, Culture=neutral, PublicKeyToken=38be1b026bbf82a9W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     9local'clr-namespace:PlanningScheduleApp.Pages     X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  .s&clr-namespace:System;assembly=mscorlib    N� Title$  SelectedDepPage�� � 	Resources    TextBoxWithToolTipStyle �� 
TargetType  TBXStyle �� BasedOn �� Property �� Value �� 
TargetType ��Name ��Name
	 � Tag
 �� Padding �� Triggers �� Value �� Binding �� RelativeSource  ToolTipVisibilityConveter �� 	Converter �� 
TargetName �� Triggers*  $System.Windows.Controls.DataGridCell  MaterialDesignDataGridCell��%n  &	        7  7  7  7  '      7  �� " ��8   .���  � �� �� " ��.R��� #��z��#��z��._�� .+��� $ PART_ContentHost��$��2q��� $ PlaceholderText��#��z	 $y�	Collapsed=�$��Center=�
�������$	
 5 0q� �� $
 True�� �� *Text+ �� *Self+8  .a��� $ PlaceholderText�� � y�$ Visible=��� $ False�� �� *Text+ �� *Self+8  .a��� $ PlaceholderText�� � y�$ 	Collapsed=� ��  �� *Text+ �� *Self+8   True.a���  �@�ToolTip$ Введите текст���� "  8   .���  � ��$ 10 7.5q�.���  � RowDefinitions �� $��0.10* ��� $��* ��� $��20 � � ColumnDefinitions �� $��* ��� $��0.24* �.+��� $��0��$��	10 5 10 5q�$��Center=�$��Center=�$��0��.+���  �� Orientation$ 
Horizontal=�.+��� $��Поиск ��$��16(�$��30���� -    ��Name$ 	SearchTBX��$	��150��$	��220��$��16(� �� TextAlignment$ Center=�$��0q�$��25��#��[ 
�������$��0q��� $
��по��$��16(�$��30��$
��10 0q��� -   $��	filterCMB��$	��100��$	��170��$��16(�$��30��$��0q��� $��0��$��0��$��6 0 0 0q�$��Bottom=�$��2��.+�]� $��Stretch=�.+��� $ 
Horizontal=�$
��Left=�.+��� -   $��StaffRemoveBtn��$��40��$��40��
������  � ToolTip$ Удалить��  UpRoundedButtonStyle#��[ 
������ {�.  .���� $&�� /Resources/icons/misc/remove.ico��$��30��$��30���� -   $��StaffRefreshBtn��$��40��$��40��
������ $ Изменить��#��[ {�. .���� $'��!/Resources/icons/misc/refresh.ico��$��30��$��30���� $ 
Horizontal=�$��Right=�.+��� -   $��ExcelBtn��$��40��$��40��
������ $ Excel��#��[ {�. .���� $)��#/Resources/icons/software/excel.ico��$��30��$��30��&   System.Windows.Controls.DataGrid  -     Name$ StaffDG��$y�Visible=�   SelectionMode$ Extended=�   AutoGenerateColumns .     CanUserAddRows .  $��1��$��0��   ColumnHeaderHeight$ 60Q�$��5 0 0 5q�   CanUserDeleteRows .    DataGridOwn#��[  � ContextMenu �� -   $
��DGCM��$��12(�$	��150��.@�m� -   $)��#Удалить отсутствие��$��	-10 0 0 0q�   ColumnHeaderStyle �� =  7System.Windows.Controls.Primitives.DataGridColumnHeader"   & "MaterialDesignDataGridColumnHeader# [ .���  � ��$	 5 0q���  � �� �� .R��� $��WrapWithOverflow=�  �� TextAlignment$  Center=����� !   Columns! 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn"   Header$!" Код сотрудника��#   Width5  /System.Windows.Controls.DataGridLengthConverter$# * $   
IsReadOnly$ . %   MinWidth$% 80Q�&   MaxWidth$& 80Q�1  +System.Windows.Controls.DataGridBoundColumn'   Binding' �� *
STAFF_ID+  $" Таб. номер��$# * $ . $% 55Q�$& 70Q�' �� *
TABEL_ID+  $" ФИО��$# * $ . $% 90Q�' �� *	SHORT_FIO+  $" Время работы��$# * $ . $	% 100Q�$	& 100Q�' �� *
WorkTime+  $" Дата��$# * $ . $	% 170Q�$	& 170Q�' �� *DTA+  DateWithDayConverter# [   $" Обед��$# * $ . $	% 100Q�$	& 100Q�' �� *	LunchTime+  $" Рабочие часы��$# * $ . $& 60Q�$% 60Q�' �� *WorkingHours+( �� StringFormat$
( 0.##��  $" Отсутствия��$	& 190Q�$	% 100Q�)   ElementStyle) �� " ��.���  � �� �� *CauseAbsence+ ��  �� *CauseAbsence+  .a���  � �� Z�   $" Дата��$# * $ . $% 90Q�$	& 160Q�' �� *AbsenceDate+$( 
dd.MM.yyyy��  $" 
Время��$# * $ . $	& 100Q�$	% 100Q�' �� *AbsenceTime+]� $��1��$��2�����#F0F0F0$��0 0 0 5q�.+��� $��0 1 1 1q����#DDDDDD._��� $ Vertical=�$��Center=�$	��200��$
��15 0q�.+��� $��График��$��14(�$��SemiBold"�$	��0 5q��� $ Vertical=�$��0 0 0 5q�.+��� $ 
Horizontal=�$��Center=�.+��� -	   $��StaffTBX��$	��240��#��[  $	 Сотрудник��$ Сотрудник��$��5 10 5 0q��� -
   * ��Name$* ClearBtn��$��30��$��30��$��Right=�$��	-35 0 5 0q�  ClearButton#��[ 
������ $��0q�$	��Top=�.���� $'��!/Resources/icons/misc/cross_1.ico��$��30��$��30��$ Очистить��+ J� Opacity$	+ 0.5Q��� -   $��StaffLV����.  $��Center=�$	��240��$y�	Collapsed=�$��90��$��12(�$	��0 0q�
�������$��1 0 1 1q���Z� �� $ 
Horizontal=�$��Center=�$	��200��.+��� $��Тип графика��$	��5 0q�$��Center=��� -   $��
TemplateCB��$	��100��$��0 2.5q��� -   $��ManageScheduleBtn��$��25��$��25��$��0q�$��10 0 0 0q�
�������
�������.���� $(��"/Resources/icons/misc/schedule.ico��$��20��$��20���� $ 
Horizontal=�$��Center=�$	��0 5q�.+��� $ Vertical=�$��5 0 5 5q�.+��� $��Дата начала��$	��5 0q�(  "System.Windows.Controls.DatePicker  -   $��ScheduleStartDP��$��75���� $ Vertical=�$	��5 0q�.+��� $!��Дата окончания��$	��5 0q�  -   $��ScheduleEndDP��$��75��{�.  �� $
��0 10q�
�������$	��100�� 	 shadForText, J� Effect#, [	 �� $ 
Horizontal=�$��0 0 0 5q�.+��� $��Отсутствие��$��14(�$��SemiBold"��� $#��(не обязательно)��$��10(�$��3 3 0 0q�$	+ 0.5Q��� $ Vertical=�.+��� $ 
Horizontal=�$��Center=�.+��� -   $��CauseTBX��$	��240��#��[  $)	 #Причина отсутствия��$) #Причина отсутствия��$��0q�- �� 
IsReadOnly- . �� -   $* ClearAbsenceBtn��$��30��$��30��$��Right=�$��	-35 0 0 0q�#��[ 
������ $��0q�$	��Top=�.���� $'��!/Resources/icons/misc/cross_1.ico��$��30��$��30��$ Очистить��$	+ 0.5Q��� -   $��CauseLV����.  $��Center=�$y�	Collapsed=�$	��240��$��90��$��12(�
�������$��1 0 1 1q���Z� �� $ Vertical=�$��Center=�$	��0 5q�.+��� $ 
Horizontal=�$��0 0 0 5q�.+��� $ Vertical=�$��0 0 5 0q�.+��� $��Дата начала��$	��5 0q�  -   $��AbsenceStartDP��$	��150���� $ Vertical=�.+��� $��
Время��%	  Xceed.Wpf.Toolkit.MaskedTextBox	  -   $��AbsenceTimeBeginMTBX��#��[ . 	  Mask$. 00:00��/ 	  
PromptChar$/  ��$��45��$��	12 0 12 0q��� $ 
Horizontal=�.+��� $ Vertical=�$��0 0 5 0q�.+��� $!��Дата окончания��$	��5 0q�  -   $��AbsenceFinishDP��$	��150��{�.  �� $ Vertical=�$��Stretch=�.+��� $��
Время��	  -   $��AbsenceTimeEndMTBX��#��[ $. 00:00��$/  ��$��45��$��	12 0 12 0q��� -   $��AddScheduleBtn��$��95��$��Добавить��$��0q�$��12(�$��0 15 0 0q��� $ 
Horizontal=�$��2��$��1��$��Center=�.+��� -   $��ResultTB���� -   $��ActionTB��$
��33.5��$��Center=�