   M S B A M L   `   `   `  ����Q  MEuromagProtocolUtility, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:EuromagProtocolUtilityEuromagProtocolUtility  5   	   K*clr-namespace:EuromagProtocolUtility.ToolsEuromagProtocolUtility.Tools  5	   	   )    #EuromagProtocolUtility.MagNetEditor   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:EuromagProtocolUtility     5   	   _ [MaterialDesignThemes.Wpf, Version=5.2.1.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      EmyAttachedProperties*clr-namespace:EuromagProtocolUtility.Tools  5	   	      MaterialDesignBody#���   5
   	   $��Regular"�5   	   $��13(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	   ���#F1F1F1  MaterialDesignFont#���  5   	    � Title$ MagNet Xml Editor��5   	   $	��800��6#   $
��1200��60    � WindowStyle$ 
ToolWindow=�6=   .��5      �  � ColumnDefinitions 5   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      .+�5   
   �� $ ��Editor file XML Per MagNet��6    $  MaterialDesignHeadline6TextBlock#���  5      $��10q�5      � 5   
   $��1��6    5      �� 5       �� 5!      �� 5"      .+�5$   
   �� -   5         ��Name$ ImportButton��5$      $��0��5%      $��5q�5&       � ToolTip$ Apri XML��5'       " MaterialDesignRaisedDarkButton#���  5(      ��5)      �� *$"CommResources.OpenXMLMagNetFileCmd+.��5*      ��  �� Orientation$ 
Horizontal=�6   .+�5+      '  !MaterialDesignThemes.Wpf.PackIcon  	   Kind$	 Import=�5,      $��Center=�5-      �� 5.      $��Apri File Xml��6   $��2q�61   $
��Auto��6<   5/      50      51      �� 52   
   $��10q�6   $��1��53      .��54      &   System.Windows.Controls.DataGrid  -   5        
  Name$
 parametersGrid��54         AlternatingRowBackground
 �����   
IsReadOnly$
 True��56         AutoGenerateColumns$ False��57      ��59      �� *'%CommResources.VariableModelMagNetList+   Columns 5:      0  *System.Windows.Controls.DataGridTextColumn  5;      ,  &System.Windows.Controls.DataGridColumn   Header$ Descrizione��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$
 auto 5=   )   1  +System.Windows.Controls.DataGridBoundColumn   Binding 5<   )   �� *Description+ �� Mode$ OneWay=�5=   )   5?      5@      #  MaterialDesignThemes.Wpf.Card  5A   
   -   5          Name$ MagnetDataCard��5A      $��1��5B      $��1��5C      $��10q�5D      .��5E      �  5F      �� 5G      �� 5H      .+�5J        -   5        $ TraduzioniCard��5J   &   $��10q�5K   &   .��5L      �  5M      �� 5N      $
��Auto �6,   �� 5O      .+�5Q      �� $��
Traduzioni��6$   $��10q�66   $
��Bold"�6B   �� 5R      $��1��6'   $��10q�5S   '   .��5T        -   5        $
 I18NDescriptionsGrid��5T   '   
 �����$
 True��5V   '   $��10q�5W   '   ��5X   '   �� *$"CommResources.I18NDescriptionsView+5Y      5Z      5\      5]      � 5^      $��2��6    5_      �� 5`      $
��Auto �6(   �� 5a      $
��Auto �6(   �� 5b       5d      �� 5e      $
��Auto �6+   �� 5f      �� 5g      .+�5i      �� $��Factory��6    $
��Bold"�5j       $��10q�5k       �� 5l      $��1��6    $��10q�5m       �� 5n      $�Menu: ��6   $
��Bold"�6,    �� 5o      �6   �� *CommResources.MagNetFactory+5p      �� 5q      $��2��6    $��10q�5r       �� 5s      $�Indice: ��6   $
��Bold"�6.    �� 5t      �6   �� *" CommResources.MagNetFactoryIndex+5u      �� 5w      $��Consumer��6    $
��Bold"�5x       $��1��5y       $��10q�5z       �� 5{      $��1��6    $��1��5|       $��10q�5}       �� 5~      $�Menu: ��6   $
��Bold"�6,    �� 5      �6   �� *CommResources.MagNetConsumer+5�      �� 5�      $��2��6    $��1��5�       $��10q�5�       �� 5�      $�Indice: ��6   $
��Bold"�6.    �� 5�      �6   �� *#!CommResources.MagNetConsumerIndex+5�      � 5�      $��2��6   $��3��5�       5�      �� 5�      $
��Auto �6,   �� 5�      .+�5�      �� $��
Traduzioni��6$   $��10q�66   $
��Bold"�6B   �� 5�      $��1��6'   $��10q�5�   '   .��5�        -   5        $
 WritingConditionGrid��5�   '   $
 True��5�   '   
 �����$
 True��5�   '   $��10q�5�   '   ��5�   '   �� *)'CommResources.WritableConditionViewList+5�      5�      5�      5�      5�      5�      5�      5�      