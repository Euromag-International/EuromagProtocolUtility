   M S B A M L   `   `   `  ���� Q  MEuromagProtocolUtility, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:EuromagProtocolUtilityEuromagProtocolUtility  K*clr-namespace:EuromagProtocolUtility.ToolsEuromagProtocolUtility.Tools  '    !EuromagProtocolUtility.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6local$clr-namespace:EuromagProtocolUtility     _ [MaterialDesignThemes.Wpf, Version=5.2.1.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  EmyAttachedProperties*clr-namespace:EuromagProtocolUtility.Tools     MaterialDesignBody#���   $��Regular"�$��13(�&   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�   TextRenderingMode$
 Auto=����#F1F1F1  MaterialDesignFont#���   � Icon$! /EuromagProtocolUtility.ico�� � Title$ Euromag Protocol Utility��$	��800��$
��1200��.���  � RowDefinitions �� �� $
��0.6* ��� $
��Auto �.+��� -    * &MaterialDesignNavigationRailTabControl#��[ .@��� -    ��Name$ 	ParamsTab�� ' #MaterialDesignNavigationRailTabItem#��[ ��0  *MaterialDesignThemes.Wpf.PackIconExtension     Kind$ Database=�   Size 24.���  �� $
��Auto ��� .+��� $#��Lista Parametri EEProm Da XML�� $  MaterialDesignHeadline6TextBlock#���  $��10q��� -    ��Name$ OpenEditorBtn��$y�	Collapsed=�$��Right=�$��5q�	 � ToolTip$ 	 Apri Editor XML per MagNet�� " MaterialDesignRaisedDarkButton#���  {��� *CommResources.ControlsEnable+.���� 
 �� Orientation$
 
Horizontal=�.+�'  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Pencil=�$��Center=��� $��MagNet Editor��$��2q�$
��Auto��� $��10q�$��1�� �� $
��Auto ��� �� $
��Auto � � ColumnDefinitions �� �� $
��1.5* �.+��  �� �� $
��Auto ��� $
��Auto �.+��� -   $ ImportButton��$��5q�$	 Apri XML��#���  ���� *CommResources.OpenXMLFileCmd+.���� $
 
Horizontal=�.+�  $ Import=�$��Center=��� $��Apri File Xml��$��2q�$
��Auto���� -   $ ExportButton��$��1��$��5q�$	 Crea CSV��#���  ���� *CommResources.BuildCsvFileCmd+.���� $
 
Horizontal=�.+�  $ FileCsv=�$��Center=��� $��Crea File CSV��$��2q�$
��Auto���� -   $ CodeGenButton��$��2��$��5q�$	 Crea .C��#���  ���� *CommResources.BuildCFileCmd+.���� $
 
Horizontal=�.+�  $ FileCode=�$��Center=��� $��Crea File .C��$��2q�$
��Auto��� $��1�� �� �� $
��Auto �.+��� $��10q�.��&   System.Windows.Controls.DataGrid  -     Name$ parametersGrid��   AlternatingRowBackground
 �����   
IsReadOnly$
 True��   AutoGenerateColumns$ False������ *,*CommResources.DeviceParametersViewFiltered+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Descrizione��   Width5  /System.Windows.Controls.DataGridLengthConverter$
 auto 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Description+ �� Mode$ OneWay=�(	  "MaterialDesignThemes.Wpf.ColorZone	  $��1��
�������$��16q�$��
10,0,10,10q�.��]� .+�	  .
  (MaterialDesignThemes.Wpf.ElevationAssist 
  	Elevation$	 Dp3=� 	  Mode$ Standard=�$��8 4 8 4q� 	  CornerRadius$ 2�� K� ZIndex$ 1��.���  �� $
��Auto ��� �� $
��Auto �.+��� -   $ ParFilterButton��  MaterialDesignToolButton#���  .��  $ Magnify=� J� Opacity$	 .56Q�$��Center=��� -	    ��Name$ ParFilterBox��$��1��$��8 0 0 0q�)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Filter��.  (MaterialDesignThemes.Wpf.TextFieldAssist   DecorationVisibility$ Hidden=�$��0q�$	��200��$��Center=� � Cursor$ Arrow������ *&$CommResources.DeviceParametersFilter+	  
�������$
��Auto��$��Stretch=�$	 Dp1=�$ 3��$��1��$��	10,5,10,5q�.���� $
��Bold"�$��Center=�$��Center=����� *CommResources.ParDescription+�� $��1��$��1��.���� $
 Vertical=�$��1��$��1��.+�#  MaterialDesignThemes.Wpf.Card  $��1��$	��Top=�$	 Dp3=�$��1��$��10q�.��� $��10q�$	��Top=� �� �� ��  �� �� �� �� �� �� �� �� �� �� �� .+��� $��Indice file XML��$��5q�$��0���� $��5q�$��0��$��1������ *CommResources.ParFileID+�� $
��Tipo��$
��Bold"�$��5q�$��2���� $��5q�$��2��$��1������ *CommResources.ParDataType+�� $��10q�$��2��$��2��$��8��$	��150��y��� *(&CommResources.ParOptionsListVisibility+.��  $
 True��$
 True��
 ��������� *CommResources.ParOptionsList+�� $
��Nome��$��5q�$��1���� $��5q�$��1��$��1������ *CommResources.ParName+�� $��Indirizzo EEPROM��$��5q�$��4���� $��5q�$��4��$��1������ *CommResources.ParEEPRomId+�� $��Indirizzo Protocollo��$
��Bold"�$��5q�$��5���� $��5q�$��5��$��1������ *CommResources.ParProtocolId+�� $��Valore di Default��$��5q�$��6���� $��5q�$��6��$��1������ *CommResources.ParDefault+�� $��Massimo��$��5q�$��7���� $��5q�$��7��$��1������ *CommResources.ParMax+�� $��Minimo��$��5q�$��8���� $��5q�$��8��$��1������ *CommResources.ParMin+�� $��Valore��$��5q�$��9���� $��5q�$��9��$��1������ *CommResources.ParValue+� $��10��$��3�� �� �� .+��� -
   $ 
GSMReadPar��$��10��$��5q�$	 Apri XML��#���  {��� *CommResources.ControlsEnable+���� *CommResources.ReadParCmd+.���� $
 
Horizontal=�.+�  $ Download=�$��Center=��� $��Frame Lettura��$��2q�$
��Auto���� -   $ GSMWritePar��$��10��$��1��$��5q�$	 Apri XML��#���  {��� *CommResources.ControlsEnable+���� *CommResources.WriteParCmd+.���� $
 
Horizontal=�.+�  $ Upload=�$��Center=��� $��Frame Scrittura��$��2q�$
��Auto���� -   $ BundleEEpromTab��#��[ {��� *CommResources.ControlsEnable+��  $ ViewList=� 24.���  �� $
��Auto ��� �� $
��Auto �.+��� $��Lista Bundles EEProm��#���  $��10q�� $��1�� �� $
��Auto ��� �� $
��Auto � �� �� .+�	  
�������$
��Auto��$��Stretch=�$	 Dp1=�$ 3��$��1��$��	10,5,10,5q�.���� $��10q�$
��Bold"�$��Center=�$��Center=��� $�Indirizzo Bundle:��$
��Bold"� �� $��Normal"���� *CommResources.BundleIndex+ �� $� �� �� $�Range Indirizzi EEPROM:��$
��Bold"� �� $��Normal"���� *CommResources.BundleSpanStart+ �� $�-��$��Normal"� �� $��Normal"���� *CommResources.BundleSpanStop+�� $��1��$��10q�.��  -   $ BundleListGrid��$��10q�
 �����$
 True��$ False������ *'%CommResources.DeviceEEpromBundlesView+   $ Nome Bundle��$
 auto  �� *Name+$ OneWay=��� $��1��$��1��$��10q�.��  -   $ BundleParListGrid��$��10q�
 �����$
 True��$ False������ *+)CommResources.DeviceEEpromBundlesListView+   $ 	Parametro��$
 auto  �� *Description+$ OneWay=�  $ Valore��$
 auto  �� *ValAsString+$ OneWay=�  -     Name$ FrameBundleCard��$��10q�$	 Dp3=�$��Bottom=�$��3��$��2���� -   $ VariablesTab��#��[ {��� *CommResources.ControlsEnable+��  $ Memory=� 24.���  �� $
��Auto ��� �� $
��Auto �.+��� $��Lista Variabili RAM��#���  $��10q�� $��1�� �� $
��Auto ���  �� $
��Auto ��� .+��� $��2��.��  -   $ VariableListGrid��$��10q�
 �����$
 True��$ False������ *#!CommResources.DeviceVariablesView+   $
 Nome��$
 auto  �� *Name+$ OneWay=�	  
�������$
��Auto��$��Stretch=�$	 Dp1=�$ 3��$��1��$��	10,5,10,5q�.���� $��10q�$
��Bold"�$��Center=�$��Center=����� *CommResources.VarName+�� $��1��$��1��.���� $
 Vertical=�.+�  -   $ VariableInfoCard��$��10q�.��� $��10q� �� ��  �� �� �� �� .+��� $
��Nome��$��5q�$��0���� $��5q�$��1��$��0������ *CommResources.VarName+�� $
��Tipo��$��5q�$��1���� $��5q�$��1��$��1������ *CommResources.VarType+�� $��Indirizzo protocollo��$
��Bold"�$��5q�$��2���� $��5q�$��1��$��2������ *CommResources.VarAddress+�� -   $ ReadVar��$��10��$��2��$��5q�$	 Apri XML��#���  ���� *CommResources.ReadVarCmd+.���� $
 
Horizontal=�.+�  $ Download=�$��Center=��� $��Frame Lettura��$��2q�$
��Auto���� -   $ BundleRamTab��#��[ {��� *CommResources.ControlsEnable+��  $ ViewListOutline=� 24.���  �� $
��Auto ��� �� $
��Auto �.+��� $��Lista Bundles RAM��#���  $��10q�� $��1�� �� $
��Auto ��� �� $
��Auto � �� �� .+�	  
�������$
��Auto��$��Stretch=�$	 Dp1=�$ 3��$��1��$��	10,5,10,5q�.���� $��10q�$
��Bold"�$��Center=�$��Center=��� $�Indirizzo Bundle: ��$
��Bold"� �� $��Normal"���� *CommResources.BundleRamIndex+�� $��1��$��10q�.��  -   $ BundleRamListGrid��$��10q�
 �����$
 True��$ False������ *$"CommResources.DeviceRamBundlesView+   $ Nome Bundle��$
 auto  �� *Name+$ OneWay=��� $��1��$��1��$��10q�.��  -   $ BundleRamParListGrid��$��10q�
 �����$
 True��$ False������ *(&CommResources.DeviceRamBundlesListView+   $ 	Parametro��$
 auto  �� *Description+$ OneWay=�  $ Valore��$
 auto  �� *ValAsString+$ OneWay=��� -   $ GSMTab��#��[ {��� *CommResources.ControlsEnable+��  $ Signal=� 24.���  �� $
��Auto ��� �� $
��Auto �.+��� $��Gestione GSM��#���  $��10q�� $��1�� �� $
��Auto ��� �� $
��Auto � �� $
��0.6* ��� .+��� $��5q����� *CommResources.GsmCommandTypes+  �� SelectedValue  �� *CommResources.GsmCommandType+�� $��10q�$��1��.��� .+�  -   $ GSMParametersGrid��
 �����$
 True��$ False��y��� *" CommResources.GsmParamVisibility+���� *!CommResources.GsmParametersView+   $ Descrizione��$
 auto  �� *Name+$ OneWay=�  -   $ GSMVariablesGrid��
 �����$
 True��$ False��y��� *%#CommResources.GsmVariableVisibility+���� * CommResources.GsmVariablesView+   $ Descrizione��$
 auto  �� *Name+$ OneWay=��� $��1��$��1��.���� $
 Vertical=�$��1��$��1��.+�  -   $ SetGSMmodeCard��$��10q�.��� $��10q� �� ��  �� �� .+��� $ ��Attiva modalità debug GSM��$��Center=��� -   $ SendDebugModeCmdBtn��$��1��$��5q�#���  {��� *CommResources.ControlsEnable+���� *,*CommResources.EnterGSMConfigurationModeCmd+.���� $
 
Horizontal=�.+�  $ Upload=�$��Center=��� $��Frame Scrittura��$��2q�$
��Auto���� $#��Disattiva modalità debug GSM��$��1��$��Center=��� -   $ SendDebugModeOffCmdBtn��$��1��$��1��$��5q�#���  {��� *CommResources.ControlsEnable+���� *+)CommResources.ExitGSMConfigurationModeCmd+.���� $
 
Horizontal=�.+�  $ Upload=�$��Center=��� $��Frame Scrittura��$��2q�$
��Auto��  $��1��$	��Top=�$	 Dp3=�$��1��$��10q�.��� $��10q�$	��Top=� �� $
��Auto ���  �� �� �� �� �� �� �� .+��� $��Indice��$��5q�$��0���� $��5q�$��0��$��1������ *CommResources.ParGsmID+�� $
��Tipo��$
��Bold"�$��5q�$��2���� $��5q�$��2��$��1������ *CommResources.ParGsmDataType+�� $
��Nome��$��5q�$��1���� $��5q�$��1��$��1������ *CommResources.ParGsmName+�� $��5q�$��4��$��1������ *CommResources.ParGsmAddress+�� $��Indirizzo Protocollo��$
��Bold"�$��5q�$��4���� $��5q�$��6��$��1������ *CommResources.ParDefault+�� $��5q�$��7��$��1������ *CommResources.ParMax+�� $��5q�$��8��$��1������ *CommResources.ParMin+�� $��Valore��$��5q�$��5���� $��5q�$��5��$��1������ *CommResources.ParGsmValue+� $��6��$��3�� �� �� .+��� -   $ ReadPar��$��10��$��5q�$	 Apri XML��#���  {��� *CommResources.ControlsEnable+���� *CommResources.ReadGsmParCmd+.���� $
 
Horizontal=�.+�  $ Download=�$��Center=��� $��Frame Lettura��$��2q�$
��Auto���� -   $ WritePar��$��10��$��1��$��5q�$	 Apri XML��#���  {��� *CommResources.ControlsEnable+���� *CommResources.WriteGsmParCmd+.���� $
 
Horizontal=�.+�  $ Upload=�$��Center=��� $��Frame Scrittura��$��2q�$
��Auto���� -   $ SettingsTab��#��[ ��  $	 Cog=� 24.���  �� $
��Auto ��� �� $
��Auto �.+��� $��Impostazioni��#���  $��10q�  -    $ SettingsCard��$��1��$��10q�.��� $��10q� �� ��  �� $
��Auto ��� $
��Auto ��� �� .+�� -!   ! �Name$! ComPortSetting�� �� �� �� .+��� $��Porta Seriale��$��Center=�$��10q��� -"   " ��Name$" ComPortSelection��$��1��# �� 
IsEditable$# False������ *CommResources.SerialPortsCom+$ �� SelectedItem$ �� *UserCommPort+�� -#   $ SavePortBtn��$��10q�$��2��$��1��#���  $
��Left=�{��� *DataChanged+���� *
SaveComCmd+.��  $ ContentSave=�$	 .56Q�$��Center=�$��24��$��24��� -$   $! BaudRateSetting��$��1�� �� �� �� .+��� $��	Baud Rate��$��Center=�$��10q��� -%   $" BaudRateSelection��$��1��$# False������ *	BaudRates+$ �� *UserBaudrate+�� -&   $ SaveBaudRateBtn��$��10q�$��2��$��1��#���  $
��Left=�{��� *DataChanged+���� *
SaveComCmd+.��  $ ContentSave=�$	 .56Q�$��Center=�$��24��$��24���� -'   $ 	FramesTab��#��[ ��  $
 Code=� 24.���  �� $
��Auto ��� .+��� $��Lista Frame Protocollo��#���  $��10q��� $��1��$��10q�.��  -(   $ FrameListGrid��$��10q�
 �����$
 True��$
 True������ *CommResources.FrameTypesView+� $��1�� �� $
��Auto ���  �� �� .+��� $��Frame��#���  $��10q�  -)   $ ProtocolFrameCard��$��1��$��10q�.���  �� �� $
��Auto �.+��� $��10q�$��1��.���� "% �� VerticalScrollBarVisibility$
% Auto=�$	��Top=�<   6EuromagProtocolUtility.Tools.TextBoxAttachedProperties&   AutoScrollToEnd$
& True��' �� TextWrapping$
' Wrap=����� *CommResources.CommandFrameHex+�� -*   $ SendCurrentCmdBtn��$��1��$��10q�$��3��#���  $	��Top=�$
��Left=�{��� *CommResources.CommPortReady+���� *CommResources.SendCommandCmd+.��  $
 Send=�$	 .56Q�$��Center=�$��24��$��24��  -+   $ 	FrameCard��$��10q�$	 Dp3=�$��Bottom=�$��1��$��1��.��� $��10q� �� $
��Auto ��� $��0* ��� �� �� �� �� �� ��  �� $
��Auto ��� �� �� $
��Auto �.+��� $	��Key��$��2��$��5q��� $��2��$��1��$��5q����� *CommResources.Key+�� $��2��$��2��$��5q����� *CommResources.DeviceKey+�� $��Header Lenght��$��3��$��5q��� $��3��$��1��$��5q����� *CommResources.HeaderLenght+�� $��10��$��3��$��2��$��5q��� $��
Frame Type��$��4��$��5q��� $��4��$��1��$��5q����� *CommResources.FrameTypeStr+�� $��4��$��2��$��5q����� *CommResources.FrameTypeCode+�� $��Data Type (Tipo)��$
��Bold"�$��5��$��5q��� $��5��$��1��$��5q����� *CommResources.DataType+�� $��5��$��2��$��5q����� *CommResources.FrameDataType+y��� *(&CommResources.WriteFrameDataVisibility+�� $��Non necessario (0)��$��5��$��2��$��5q�y��� *'%CommResources.ReadFrameDataVisibility+�� $��Data Lenght��$��6��$��5q��� $��6��$��1��$��5q����� *CommResources.DataLenght+�� $��Non necessario (0)��$��6��$��2��$��5q�y��� *'%CommResources.ReadFrameDataVisibility+�� $��6��$��2��$��5q����� *CommResources.DataLenghtValue+y��� *(&CommResources.WriteFrameDataVisibility+�� $$��Address (Indirizzo Protocollo)��$
��Bold"�$��7��$��5q��� $��7��$��1��$��5q����� *CommResources.Address+�� $��7��$��2��$��5q����� *CommResources.AddressVal+�� $��CRC16-CCIT (0xFFFF)��$��8��$��5q��� $��8��$��1��$��5q����� *CommResources.Crc+	  
�������$
��Auto��$��Stretch=�$	 Dp1=�$ 3��$��2��$��2��$��10,10,10,10q�.���� $��10q�$
��Bold"�$��Center=�$��Center=��� ��� *CommResources.XmlFileName+ �� $�  -  �� �� ��� *CommResources.UserCommPort+