   M S B A M L   `   `   `  ���� N  JPlanningScheduleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:PlanningScheduleApp.PagesPlanningScheduleApp.Pages  ,    &PlanningScheduleApp.Pages.StaffAddPage   T PMathCore.WPF, Version=0.0.44.5, Culture=neutral, PublicKeyToken=38be1b026bbf82a9W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     9local'clr-namespace:PlanningScheduleApp.Pages       N� Title$  StaffAddPage�� � 	Resources    SP �� 
TargetType �� Property �� Value  TBXStyleEdited  TBXStyle �� BasedOn  BorderTodayTBStyle �� Triggers �� Binding �� Value��%f  &	        &	 �     7  &	 �     �� " ��.���  �@��Orientation$ Vertical=���  � ��$ 	5 7.5 5 0q���  �@K�ZIndex$ 2����  � ��$ 2���� " ��8   .���  � ��$ 5 10 5 0q���  � ��$ 30���� " ��.���  �@J�Opacity$ 0Q� ��  �� *IsTodayDateMatching+ True.a���  �@J�Opacity$ 1Q�.��� 	 � RowDefinitions	 �� $��0.15* ��� $��* ��� $��50 �
 � ColumnDefinitions
 �� $��20 ��� $��* ��� $��20 �.+��� $��Center=�$��0��$��1��$��2�� K� ZIndex$ 3��$	��Top=�$
��0 20q�.+���  �� Orientation$ 
Horizontal=�$��2��$ 3��.+��� $��Сотрудник: ��$��16(��� $ Vertical=�$��2��$ 3��.+��� -   $��StaffTBX��$��16(�$
��auto��$	��200��$	��150��#��[ $	��5 0q��� -   $��StaffLV��$
��auto��$	��200��$	��150��$��75��$	��150��$
��auto��$y�	Collapsed=�$��2��$ 3����Z� �� $��1��$��1��$ 
Horizontal=�$��Center=�.+��� $
��Auto=�$��1��$��1��$	��600��$
��1120��.���� $ 
Horizontal=�$��Center=�.+�A� $��0 0 0 0q�$��Center=�$	��Top=�.+��� ���� *	DatesList+���� .R���  �� CornerRadius$ 5��
�������$	��0 5q�$	��480��$
��auto��$	��480��$��60��$��2q�
�������$��Center=�  ShadowEffect J� Effect# [ ._��� .+��� $ 
Horizontal=�.+��� $��Center=�$
��10 0q��� $
��Bold"���� *Date+ �� StringFormat$ {0:dd.MM.yyyy}���� �� 
���������� *Date+  DayOfWeekConverter �� 	Converter# [ �� $��Рабочие часы: ��$��Center=�$��30 0 0 0q��� #��[  $��Center=�.+��� $��WorkingHoursTBX��-   $��30��$��50�� �� TextAlignment$ Center=�#��[ $��0 0 0 10q����� *WorkingHours+ �� Mode$ TwoWay=�  ZeroToEmptyStringConverter# [  �� UpdateSourceTrigger$ PropertyChanged=��� $��Сегодня��#��[ $��Right=�$��
0 -40 10 0q�$��Italic$�
��������� -   $��AddScheduleBTN��$	��100��$��40��$��2��$��1��  RoundedButtonStyle#��[ $��Добавить��