   M S B A M L   `   `   `  ���� N  JPlanningScheduleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:PlanningScheduleAppPlanningScheduleApp  *    $PlanningScheduleApp.InfoCustomWindow   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PMathCore.WPF, Version=0.0.44.5, Culture=neutral, PublicKeyToken=38be1b026bbf82a9W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     3local!clr-namespace:PlanningScheduleApp       � Title$  Информация��$	��245��$	��495�� � 
ResizeMode$ NoResize=� � 	Resources  �� 
TargetType   MaterialDesignToolButton �� BasedOn �� Property �� Value��%a   '��       7   �� " ��8   .���  � ��$ 0q���  � ��
 ���� ��  � {� .  .��� .+��� -   $��InfoTC��.@��� -   $��WorkingDayTI��$��Рабочий день��.����  �� Orientation$ Vertical=�$��	10 5 10 0q�.+��� -   $��WorkingDayInfoTBX��$��14(��� -   $��	AbsenceTI��$��Отсутствие��.���� $ Vertical=�$��	10 5 10 0q�.+��� -   $��	AbsenceLV��$��Center=�$��Center=� �� View �� .P��� $��
Время��	 �� Width$
	 auto��
 �� DisplayMemberBinding
 �� *AbsenceTime+�� $��Причина��$
	 auto��
 �� *CauseAbsence+�� $��Дата��$
	 auto��
 �� *AbsenceDate+�� $
	 auto�� �� CellTemplate �� .R��� $��AbsenceRemoveBtn��-   $��0q�
������ 
������ $��25��.���� $%��Resources/icons/misc/remove.ico���� -   $��CloseBtn��$	��100��$��30��$��Закрыть��$��Bottom=�$��0 0 0 10q�