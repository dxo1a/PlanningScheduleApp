   M S B A M L   `   `   `  ����N  JPlanningScheduleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:PlanningScheduleAppPlanningScheduleApp  5   	   *    $PlanningScheduleApp.InfoCustomWindow   5   	   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PMathCore.WPF, Version=0.0.44.5, Culture=neutral, PublicKeyToken=38be1b026bbf82a9W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:PlanningScheduleApp     5   	     � Title$  Информация��5   	   $	��245��6   $	��495��6)    � 
ResizeMode$ NoResize=�65    � 	Resources 5	       �� 
TargetType   MaterialDesignToolButton �� BasedOn �� Property �� Value��%�   '��       7   �� 5
   
   " ��6   8   .�5      ��  � ��$ 0q�6(   �� 5       � ��
 ���� 6+   �� 5       � {� .  6*   5      5      .��5      � .+�5   
   �� -   5        $��InfoTC��5      .@�5      �� -   5        $��WorkingDayTI��5   ,   $��Рабочий день��6   .��5      ��  �� Orientation$ Vertical=�6   $��	10 5 10 0q�64   .+�5      �� -   5        $��WorkingDayInfoTBX��5       $��14(�69   5      5      �� 5      -   5        $��	AbsenceTI��5   *   $��Отсутствие��6   .��5      �� $ Vertical=�6   $��	10 5 10 0q�64   .+�5      �� -   5        $��	AbsenceLV��5      $��Center=�60   $��Center=�6R    �� View 5      �� 5      .P�5   "   �� $��
Время��61   	 �� Width$
	 auto��6m   
 �� DisplayMemberBinding
 6@   �� *AbsenceTime+6m   �� 5   "   $��Причина��61   $
	 auto��6p   
 6B   �� *CauseAbsence+6p   �� 5   "   $��Дата��61   $
	 auto��6l   
 6?   �� *AbsenceDate+6l   �� 5   "   $
	 auto��61    �� CellTemplate 5    &   �� 5!   *   .R�5"   .   �� $��AbsenceRemoveBtn��65   -   5        $��0q�5"   l   
������ 
������ $��25��6�   .��5#   2   �� $%��Resources/icons/misc/remove.ico��68   5$   /   5%   +   5'   #   5(      5*      5+      5,      5-      �� 5/   
   -   5        $��CloseBtn��5/      $	��100��6!   $��30��6-   $��Закрыть��69   $��Bottom=�6K   $��0 0 0 10q�6f   6x   50      51      