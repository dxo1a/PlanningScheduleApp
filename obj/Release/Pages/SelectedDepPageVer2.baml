   M S B A M L   `   `   `  ���� [  WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms   PlanningScheduleApp3   -PlanningScheduleApp.Pages.SelectedDepPageVer2   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PMathCore.WPF, Version=0.0.44.5, Culture=neutral, PublicKeyToken=38be1b026bbf82a9W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Owf@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit    N� Title$  SelectedDepPageVer2�� � 	Resources *  $System.Windows.Controls.DataGridCell �� 
TargetType   MaterialDesignDataGridCell �� BasedOn �� Property �� Value��%:   '        7   �� "  8   .���  � ��$ 10 7.5q�.��� -    �Name$ grid1�� � RowDefinitions �� $��0.10* ��� $��* ��� $��20 � � ColumnDefinitions �� $��* ��� $��0.26* �.+��� $��0��$��	10 5 10 5q�$��Center=�$��Center=�$��0��.+��� 	 �� Orientation$	 
Horizontal=�.+��� $��Поиск ��$��16(�$��30���� -   
 ��Name$
 	SearchTBX��$	��150��$	��220��$��16(� �� TextAlignment$ Center=�$��0q�$��25��  TBXStyle#��[ 
�������$��0q��� $
��по��$��16(�$��30��$
��10 0q��� -   $��	filterCMB��$	��100��$	��170��$��16(�$��30��$��0q��� $��0��$��0��$��6 0 0 0q�$��Bottom=�$��2��.+�]� $��Stretch=�.+��� $	 
Horizontal=�$
��Left=�.+��� -   $��StaffRemoveBtn��$��40��$��40��
������  � ToolTip$ Удалить��  UpRoundedButtonStyle#��[ 
������ {�.  .���� $(��"/Resources/icons/misc/remove_2.ico��$��30��$��30��  ShadowEffectForIcons J� Effect# [ �� -   $��StaffRefreshBtn��$��40��$��40��
������ $ Изменить��#��[ {�. .���� $)��#/Resources/icons/misc/refresh_2.ico��$��30��$��30��# [ �� $	 
Horizontal=�$��Right=�.+��� -   $��ExcelBtn��$��40��$��40��
������ $ Excel��#��[ {�. .���� $)��#/Resources/icons/software/excel.ico��$��30��$��30���� -   $��MonthCMB��$	��100��$	��170��$��16(�$��30�� �� MaxDropDownHeight$	 200��$��0q�$��	0 20 10 0q�$��Right=���.  ]� $��1��$��2�����#F0F0F0$��0 0 0 5q�.+��� $��0 1 1 1q����#DDDDDD._��� $	 Vertical=�$��Center=�$	��200��$
��15 0q�.+��� $��График��$��14(�$��SemiBold"�$	��0 5q��� $	 Vertical=�$��0 0 0 5q�.+��� $	 
Horizontal=�$��Center=�.+��� -   $��StaffTBX��$	��240��  TextBoxWithToolTipStyle#��[ 
 � Tag$ Сотрудник��$ Сотрудник��$��5 10 5 0q��� -	    ��Name$ ClearBtn��$��30��$��30��$��Right=�$��	-35 0 5 0q�  ClearButton#��[ 
������ $��0q�$	��Top=�.���� $'��!/Resources/icons/misc/cross_1.ico��$��30��$��30��$ Очистить�� J� Opacity$	 0.5Q��� -
   $��StaffLV����.  $��Center=�$	��240��$y�	Collapsed=�$��90��$��12(�$	��0 0q�
�������$��1 0 1 1q���Z� �� $	 
Horizontal=�$��Center=�$	��200��.+��� $��Тип графика��$	��5 0q�$��Center=��� -   $��
TemplateCB��$	��100��$��0 2.5q��� -   $��ManageScheduleBtn��$��25��$��25��$��0q�$��10 0 0 0q�
�������
�������$- 'Управление шаблонами��.���� $*��$/Resources/icons/misc/schedule_2.ico��$��20��$��20���� $	 
Horizontal=�$��Center=�$	��0 5q�.+��� $	 Vertical=�$��5 0 5 5q�.+��� $��Дата начала��$	��5 0q�(  "System.Windows.Controls.DatePicker  -   $��ScheduleStartDP��$��75���� $	 Vertical=�$	��5 0q�.+��� $!��Дата окончания��$	��5 0q�  -   $��ScheduleEndDP��$��75��{�.  �� -   $��AddScheduleBtn��$��95��$��Добавить��$��0q�$��12(�$��0 15 0 0q��� $
��0 10q�
�������$	��100��  shadForText# [ �� $��Отсутствие��$��14(�$��SemiBold"��� $	 
Horizontal=�$��Center=�.+��� -   $��StaffAbsenceTBX��$	��240��#��[ $ Сотрудник��$ Сотрудник��$��5 10 5 0q��� -   $��30��$��30��$��Right=�$��	-35 0 5 0q�#��[ 
������ $��0q�$	��Top=�.���� $'��!/Resources/icons/misc/cross_1.ico��$��30��$��30��$ Очистить��$	 0.5Q��� -   $��StaffAbsenceLV����.  $��Center=�$	��240��$y�	Collapsed=�$��90��$��12(�$	��0 0q�
�������$��1 0 1 1q���Z� �� $	 
Horizontal=�$��Center=�$	��200��$��0 5 0 0q�.+��� $��Причина��$	��5 0q�$��Center=��� -   $��CauseCB��$	��100��$��0 2.5q��� -   $��ManageCauseBtn��$��25��$��25��$��0q�$��10 0 0 0q�
�������
�������$- 'Управление причинами��.���� $$��/Resources/icons/misc/list.ico��$��24��$��24���� $	 Vertical=�$��Center=�$	��0 5q�.+��� $	 
Horizontal=�$��0 0 0 5q�.+��� $	 Vertical=�$��0 0 5 0q�.+��� $��Дата начала��$	��5 0q�  -   $��AbsenceStartDP��$	��150���� $	 Vertical=�.+��� $��
Время��%  Xceed.Wpf.Toolkit.MaskedTextBox  -   $��AbsenceTimeBeginMTBX��#��[    Mask$ 00:00��   
PromptChar$  ��$��45��$��	12 0 12 0q��� $	 
Horizontal=�.+��� $	 Vertical=�$��0 0 5 0q�.+��� $!��Дата окончания��$	��5 0q�  -   $��AbsenceFinishDP��$	��150��{�.  �� $	 Vertical=�$��Stretch=�.+��� $��
Время��  -   $��AbsenceTimeEndMTBX��#��[ $ 00:00��$  ��$��45��$��	12 0 12 0q��� -   $��AddAbsenceBtn��$��95��$��Добавить��$��0q�$��12(�$��0 15 0 0q��� -    ��Name$ 	DGVBorder��$��1��$��0��
�������$��0q�$	��490��$��0 0 0 5q�._�7  1System.Windows.Forms.Integration.WindowsFormsHost  -     Name$ windowsFormsHost��   Child. '   !System.Windows.Forms.DataGridView  -   StaffDGV��   RowHeadersVisible .     ReadOnly .    AllowUserToResizeRows .     AllowUserToAddRows .     AutoSizeColumnsMode$
 Fill=�   AutoGenerateColumns .     BackgroundColorU	 QSystem.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a# 	 System.Drawing.ColorConverter$ White "   System.Windows.Forms.Control   Dock$
 Fill=��� -   $��StatusTB��$��15(�$��2��$��0��$��Center=�$��Center=�$	 0.5Q�