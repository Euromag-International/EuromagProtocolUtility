   M S B A M L   `   `   `  ���� Q  MEuromagProtocolUtility, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:EuromagProtocolUtilityEuromagProtocolUtility  K*clr-namespace:EuromagProtocolUtility.ToolsEuromagProtocolUtility.Tools  )    #EuromagProtocolUtility.MagNetEditor   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6local$clr-namespace:EuromagProtocolUtility     _ [MaterialDesignThemes.Wpf, Version=5.2.1.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  EmyAttachedProperties*clr-namespace:EuromagProtocolUtility.Tools     MaterialDesignBody#���   $��Regular"�$��13(�&   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=����#F1F1F1  MaterialDesignFont#���   � Title$ MagNet Xml Editor��$	��800��$
��1200�� � WindowStyle$ 
ToolWindow=�.���  � ColumnDefinitions �� ��  � RowDefinitions �� $
��Auto ��� .+��� $ ��Editor file XML Per MagNet�� $  MaterialDesignHeadline6TextBlock#���  $��10q�� $��1�� �� �� �� .+��� -    ��Name$ ImportButton��$��0��$��5q� � ToolTip$ Apri XML�� " MaterialDesignRaisedDarkButton#���  ���� *$"CommResources.OpenXMLMagNetFileCmd+.����  �� Orientation$ 
Horizontal=�.+�'  !MaterialDesignThemes.Wpf.PackIcon  	   Kind$	 Import=�$��Center=��� $��Apri File Xml��$��2q�$
��Auto���� $��10q�$��1��.��&   System.Windows.Controls.DataGrid  -   
  Name$
 parametersGrid��   AlternatingRowBackground
 �����   
IsReadOnly$
 True��   AutoGenerateColumns$ False������ *'%CommResources.VariableModelMagNetList+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Descrizione��   Width5  /System.Windows.Controls.DataGridLengthConverter$
 auto 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Description+ �� Mode$ OneWay=�#  MaterialDesignThemes.Wpf.Card  -     Name$ MagnetDataCard��$��1��$��1��$��10q�.���  �� �� .+�  -   $ TraduzioniCard��$��10q�.���  �� $
��Auto ��� .+��� $��
Traduzioni��$��10q�$
��Bold"��� $��1��$��10q�.��  -   $
 I18NDescriptionsGrid��
 �����$
 True��$��10q����� *$"CommResources.I18NDescriptionsView+� $��2�� �� $
��Auto ��� $
��Auto ���  �� $
��Auto ��� �� .+��� $��Factory��$
��Bold"�$��10q��� $��1��$��10q��� $�Menu: ��$
��Bold"� �� ��� *CommResources.MagNetFactory+�� $��2��$��10q��� $�Indice: ��$
��Bold"� �� ��� *" CommResources.MagNetFactoryIndex+�� $��Consumer��$
��Bold"�$��1��$��10q��� $��1��$��1��$��10q��� $�Menu: ��$
��Bold"� �� ��� *CommResources.MagNetConsumer+�� $��2��$��1��$��10q��� $�Indice: ��$
��Bold"� �� ��� *#!CommResources.MagNetConsumerIndex+� $��2��$��3�� �� $
��Auto ��� .+��� $��
Traduzioni��$��10q�$
��Bold"��� $��1��$��10q�.��  -   $
 WritingConditionGrid��$
 True��
 �����$
 True��$��10q����� *)'CommResources.WritableConditionViewList+