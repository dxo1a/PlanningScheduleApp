   M S B A M L   `   `   `  ����N  JPlanningScheduleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:PlanningScheduleApp.MVVM.ViewPlanningScheduleApp.MVVM.View  5   	   W0clr-namespace:PlanningScheduleApp.MVVM.ViewModel"PlanningScheduleApp.MVVM.ViewModel  5   9   1    +PlanningScheduleApp.MVVM.View.ProcessWindow   5   	   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PMathCore.WPF, Version=0.0.44.5, Culture=neutral, PublicKeyToken=38be1b026bbf82a9W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   =local+clr-namespace:PlanningScheduleApp.MVVM.View     5   	   Z VFontAwesome.WPF, Version=4.7.0.37774, Culture=neutral, PublicKeyToken=0758b07a11a4f466-fa$http://schemas.fontawesome.io/icons/  5   	   :vms0clr-namespace:PlanningScheduleApp.MVVM.ViewModel  69   $	��220��5	   '   $	��350��64     � Title  6	   �� *WindowTitle+ � DataContext 5
      F   @PlanningScheduleApp.MVVM.ViewModel.ProcessExportToExcelViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $
��0.7* �6   �� 5      $
��0.3* �6   .+�5   
   ��  �� Orientation$ Vertical=�6   $��Center=�6,   $��10q�6G   y�6S   �� *IsProcessPanelVisible+    BooleanToVisibilityConverter �� 	Converter# [  .+�5      �� $ 
Horizontal=�6   $��Center=�62   .+�5      �� -   5         ��Name$ TaskTB��5      $��Выполняется: ��6,   $��Center=�6A   $��Center=�6\   $��16(�6y   $
��Wrap=�6�    �� TextAlignment$ Center=�6�   �� 5      -   5        $ TaskName��5      $��Center=�6H   $��Center=�6c   $��16(�6�   $
��Wrap=�6�   $ Center=�6�   ��6.   �� *
TaskName+6�   5      �� 5      $ 
Horizontal=�6   $��Center=�62   .+�5      �� -   5        $ CurrentProgressTB��5      $��Center=�6X   $��Center=�6s   $��16(�6�   $
��Wrap=�6�   $ Center=�6�   ��67   �� *CurrentProgress+6�   �� 5      $��/��6   $��Center=�6%   $��Center=�6@   $��16(�6]   $
��Wrap=�6k   $ Center=�6   �� 5      -   5        $ TotalProgressTB��5      $��Center=�6T   $��Center=�6o   $��16(�6�   $
��Wrap=�6�   $ Center=�6�   ��65   �� *TotalProgress+6�   5      "  FontAwesome.WPF.ImageAwesome  5      -   5        $��Spinner��5         Icon$ Spinner=�6-      Spin . 	   SpinDuration$	 2Q�6H   $��35��6Y   $��35��6d   $��0 15 0 0q�6p   5      �� 5   
   $ Vertical=�6   $��Center=�6,   $��10q�6G   y�6S   �� *IsResultPanelVisible+# [  .+�5      �� -   5        $ ResultTB��5      $��Center=�6J   $��Center=�6e   $��16(�6�   $
��Wrap=�6�   $ Center=�6�   ��6*   �� *TaskTextResult+6�   5       �� 5!   
   -   5        $��OpenFolderBtn��5!      $��1��6&   $��10 0 0 0q�63   $��45��6E   $��45��6P     RoundedButtonStyle#��[ 6\   $
��Left=�6�   ��6�   �� *OpenFolderCommand+.��5"      �� $&�� /Resources/icons/misc/folder.ico��6   $��30��6>   $��30��6L   5#      �� 5$   
   $!��ООО «Трансмаш»��6   $��1��6*   $��Center=�67   $��Center=�6T   
 J� Opacity$

 0.25Q�6o   $��SemiBold"�6~   �� 5%   
   -   5        $��Bitrix24Export��5%      $��1��6'   $��0 0 10 0q�64   $��45��6F   $��45��6Q   #��[ 6]   $��Right=�6�   ��6�   �� *ExportToBitrixCommand+.��5&      �� $*��$/Resources/icons/software/bitrix.ico��6   $��30��6B   $��30��6P   5'      5(      5)      