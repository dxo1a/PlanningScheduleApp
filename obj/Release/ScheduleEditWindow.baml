   M S B A M L   `   `   `  ���� N  JPlanningScheduleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:PlanningScheduleAppPlanningScheduleApp  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem ,    &PlanningScheduleApp.ScheduleEditWindow   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PMathCore.WPF, Version=0.0.44.5, Culture=neutral, PublicKeyToken=38be1b026bbf82a9W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     3local!clr-namespace:PlanningScheduleApp     X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  .s&clr-namespace:System;assembly=mscorlib  _	 [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes 	   � Title$-  'Редактировать шаблон��$	��550��$	��550��$	��380��$	��380�� � 	Resources    BorderCustom �� 
TargetType �� Property �� Value  
CustomMTBX%  Xceed.Wpf.Toolkit.MaskedTextBox  TBXStyle �� BasedOn �� Triggers �� Binding �� Value. 	 (MaterialDesignThemes.Wpf.TextFieldAssist  	CustomTBX��%�  &	        &	 �     7  &	 �    7  �� " ��.���  � �� �	#FFC1C1C1��  � ��$ 2q���  � ��
 �������  �@��CornerRadius$	 2.5����  �@��Padding$ 0 5 5 5q���  � ��$	 300����  � ��$ Center=���  � ��$ 0 2.5q��� "  8    ��  �� *isRestingDay+ True.a���  � {� .  ��  �@ TextFieldCornerRadius$ 0����  �� *isRestingDay+	 False.a���  � {� . �� " ��8    ��  �� *isRestingDay+ True.a���  � {� .  ��  �@ TextFieldCornerRadius$ 0����  �� *isRestingDay+	 False.a���  � {� . .��� 	 � RowDefinitions	 �� $��0.075* ��� $��* �.+��� 
 �� Orientation$
 
Horizontal=�$	��250��$��Center=�.+��� -   $��TemplateNameTBX��$	��200��$��	5 7.5 5 0q�#��[ $��15(�$��0 0 5 0q� J� IsHitTestVisible .  �� -   $��TemplateAdditionalNameTBX��$��50��$��	5 7.5 5 0q�#��[ $��15(� .  �� -   $��
ScheduleTC��$��1��.@��� -   $��StaticScheduleTI��$ ��Фиксированный��$y�	Collapsed=�.���� $
 Vertical=�$��Center=�.+��� -   $��StaticDaysIC��$��1������ *
StaticDays+���� .R��� #��[  $	��355��._��� $
 
Horizontal=�.+��� $��Center=�$��14(�$��5 0 10 0q�$��90������ *Day+  DayOfWeekConverter �� 	Converter# [ �� $
 Vertical=�.+��� $��Рабочее время��$��Center=��� $
 
Horizontal=�.+�  -   #��[    Mask$ 00:00��$��45��   
PromptChar$  ��$��0 3 0 0q� �� TextAlignment$ Center=�$��0 2 0 0q����� *	WorkBegin+�� $��-��$��Center=�$	��5 0q�$��16(�$
��Bold"�  -   #��[ $ 00:00��$��45��$  ��$��0 3 0 0q�$ Center=�$��0 2 0 0q����� *	WorkEnd+�� $
 Vertical=�.+��� $��Обед��$��Center=��� $
 
Horizontal=�.+�  -   #��[ $ 00:00��$��5 3 0 0q�$��45��$  ��$ Center=�$��0 2 0 0q� �� 	MaxLength$ 4������ *LunchTimeBegin+�� $��-��$��Center=�$	��5 0q�$��16(�$
��Bold"�  -	   #��[ $ 00:00��$��0 3 5 0q�$��45��$  ��$ Center=�$��0 2 0 0q�$ 4������ *LunchTimeEnd+�� $��isRestingDayCB��-
   $��
0 17.5 0 0q� d� 	IsChecked �� *isRestingDay+�� -   $��SaveFixedTemplateBtn��$��85��$��30��$��0q�$��Сохранить��$��0 10 0 0q��� -   $��FlexibleScheduleTI��$��Плавающий��$y�	Collapsed=�.���� $
 Vertical=�$��Center=�$��0 10 0 0q� � Cursor$  ��.+��� $
 
Horizontal=�$��Center=�.+��� $
 Vertical=�$	��5 0q�.+��� $)��#Кол-во рабочих дней���� -   $��WorkingDaysCountCMB���� $
 Vertical=�$	��5 0q�.+��� $+��%Кол-во выходных дней���� -   $��RestingDaysCountCMB���� -   $��FlexibleDaysIC��$��1��$��0 15 0 0q����� *FlexibleDays+���� .R��� #��[  $	��355��._��� $
 
Horizontal=�.+��� $��Center=�$��14(�$��5 0 10 0q�$��90������ *Day+�� $
 Vertical=�.+��� $��Рабочее время��$��Center=��� $
 
Horizontal=�.+�  -   $ 00:00��$��45��$  ��$��0 3 0 0q�$ Center=�$��0 0 0 5q����� *	WorkBegin+.������ "  # [ .���� $��-��$��Center=�$	��5 0q�$��16(�$
��Bold"�  -   $ 00:00��$��45��$  ��$��0 3 0 0q�$ Center=�$��0 0 0 5q����� *	WorkEnd+.������ "  # [ .���� $
 Vertical=�.+��� $��Обед��$��Center=��� $
 
Horizontal=�.+�  -   #��[ $ 00:00��$��15 3 0 0q�$��45��$  ��$ Center=�$��0 2 0 0q�$ 4������ *LunchTimeBegin+�� $��-��$��Center=�$	��5 0q�$��16(�$
��Bold"�  -   #��[ $ 00:00��$��0 3 5 0q�$��45��$  ��$ Center=�$��0 2 0 0q�$ 4������ *LunchTimeEnd+�� -   $��SaveFlexibleTemplateBtn��$��85��$��30��$��0q�$��Сохранить��$��0 10 0 0q�