   M S B A M L   `   `   `  ����F  BSpaceTrader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:SpaceTraderSpaceTrader  5	   	       SpaceTrader.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  5   	   T PPrism.Wpf, Version=8.1.97.5141, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59"cmdhttp://prismlibrary.com/  5   	   Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5   	   %localclr-namespace:SpaceTrader  5	   	   |�.   � Title$5  /Equilibrium simulation version 0.0.3  pre-Alpha��5
      
 � Top$ 50��6U    � Left$ 50��6^   ��6h   �� *,*CommonSettings.ScreenSettings.ScreenHeight+ �� Mode$ TwoWay=���6�   �� *+)CommonSettings.ScreenSettings.ScreenWidth+$ TwoWay=�.��5      ]� 
���   �.+�5      o� $	��Top=�6   
���   �.@�5      m� 
���   �
�������$��_Game��6D   .@�5      m� 
���   �$(��"Set parameters for Galaxy creation��6.    m� Command 6Z   �� *'%IShowGalacticGenerationSettingsScreen+m� 5      
���   �$��_New Galaxy��6.    6C   �� *ISetNewGamedata+5      m� 5      
���   �
�������$��Display��6D   .@�5      �� $��11(�6   $��Center=�6)   $��Draw starlanes��6L    d� 	IsChecked 6e   �� **(CommonSettings.ScreenSettings.BDrawLines+��6�   �� *IRedrawScreen+6�   �� 5      $��11(�6   $��Center=�6)   $��Display Ship Info��6L    6h   �� *75CommonSettings.ScreenSettings.DisplayShipInfoonScreen+6�   �� 5      
���OO/�6   �� 5       �� Orientation$ 
Horizontal=�6   .+�5      �� $��11(�6    $��Center=�6.   
���   �$�� Highlight -> ��6]   �� 5      $��0q�6   $��10(�6*   $	��140��68   $��20��6D   ��6P   �� *Items+ �� SelectedItem 6n   �� *CurrentItem+��5      �� 5      .R�5   "   �� $ 
Horizontal=�6-   .+�5   &   �� ��60   �� 5   #   5      5       5!      5"      m� 5#      
���   �
�������$��_Information screens��6E   .@�5$      m� 
���   �$��_Celestial body��6/    6H   �� * IShowCelestialBodyInfoonScreen+�� 5%      
���OO/�6   m� 5&      
���   �$��_Initialisation results��6/    6P   �� *(&IShowGameInitialisationResultsonScreen+5(      5)      � 5*   
    � ColumnDefinitions 5+      �� 5,      $
��Auto �6#   �� 5-      $
��Auto �6#   �� 5.      .+�50      �� $��1��6   $��Center=�6+   $��Stretch=�6H   
���   �	 �� ShowsPreview	 . $��5��6�   � 51      $��0��6   
���   �$��15��66   52      � 53      $��2��6   
���   �$
��Auto��67   *  $Microsoft.Xaml.Behaviors.Interaction
   Triggers
 54      +  %Microsoft.Xaml.Behaviors.EventTrigger  55         	EventName$ PreviewKeyDown��6%   *  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 56      -  'Prism.Interactivity.InvokeCommandAction     Command 62   �� *IKeyPressedDown+57        58      $ PreviewKeyUp��6%   . 59         62   �� *IKeyPressedUp+5:      .+�5<      �� .+�5=      �� $ 
Horizontal=�6!   .+�5>      �� $��9(�6!   $��30��6.   $��0q�6:   
���   �
�������$��  ||  ��6t   ��6�   �� *IPauseShips+�� 5?      $��9(�6!   $��30��6.   $��0q�6:   
���   �
�������$��  |>  ��6t   ��6�   �� *IUnpauseShips+�� 5@      $��9(�6!   $	��100��6.   $��0q�6;   
���   �
�������$��
 set path ��6u   ��6�   �� *ISetShipPath+�� 5A      $��  -  ��6$   �� 5B      
���   �
���������6K   �� *
StarDate+5C      �� 5D      $��Hidden=�6#   $��Hidden=�6W   .��5E      � -   5        $��GrdMain��5E      $
��Auto��6.   $
��Auto��6;   $
��Left=�6I    5F      �� 5G   "   $��10 �63   �� 5H   &   $	��150 �67   �� 5I   &   $	��150 �67   �� 5J   &   $
��1550 �67   �� 5K   &   $	��600 �67   .+�5M      �� -   5        $��image1��5M   �   $��1��6A   $��4��6Q   $
��Left=�6e   $	��Top=�6�   $��0,0,0,0q�6�   $
��4000��6�   $
��4000��6�   ��6$   �� *	TestImage+6�   �� 5O      $��2��6%   $��5��65   
������ .+�5P   "   �� $��8(�6*   $��0q�67   
����d�
������ ��6h   �� *SelectedShipSystemText+y�6�   �� *:8CommonSettings.ScreenSettings.VisibilityShipInfoonScreen+ �� Left 6�   �� *0.Ships.ShipSelectedonScreen.ScreenCoordinates.X+
 �� Top 6C  �� *0.Ships.ShipSelectedonScreen.ScreenCoordinates.Y+5Q      �� 5R      $��1��6%   $��5��65   
������ .+�5S   "   �� $��9(�6)   $��0q�66   
�������$�� ship->stellarobject ��6]   
������ y�6}   �� *LJCommonSettings.ScreenSettings.VisibilityButtonCalculateShiptoStellarObject+��6�   �� *)'ICalculatePathFromShiptoDestinationStar+ 64  �� *?=CelestialBodies.StellarObjectSelectedOnScreen.FinalPosition.X+ 6�  �� *?=CelestialBodies.StellarObjectSelectedOnScreen.FinalPosition.Z+�� 5T   "   $��9(�6)   $��0q�66   
������ 
�������$��
 set path ��6v   y�6�   �� *LJCommonSettings.ScreenSettings.VisibilityButtonCalculateShiptoStellarObject+��6�   �� *ISetShipPath+ 6  �� *?=CelestialBodies.StellarObjectSelectedOnScreen.FinalPosition.X+ 6d  �� *ECCelestialBodies.StellarObjectSelectedOnScreen.FinalPosition2ndBtn.Z+5U      �� 5V      $ Vertical=�6)   $��3��6@   .+�5W   "   �� $	��600��6/   $	��250��6<   $	��Top=�6H   $
��Left=�6`   $��3��6{   $��Hidden=�6�   $��Hidden=�6�   .��5X   &   �� $��0��6Y   $��3��6i   $
��2000��6}   $��8(�6�   $
��left=�6�   $	��Top=�6�   
������ 
����d���60   �� *StellarobjectSystemText+6�   5Y   #   �� 5Z   "   $��-----��6,   $��20��6:   
������ 
����d�6_   �� 5[   "   $	��300��6/   $	��250��6<   $	��Top=�6H   $
��Left=�6`   $��3��6{   $��Hidden=�6�   $��Hidden=�6�   .��5\   &   �� $��0��6N   $��3��6^   $
��2000��6r   $��8(�6�   $
��left=�6�   $	��Top=�6�   
������ 
����d���60   �� *OverviewText+6�   5]   #   5^      
 5b        5c   "   $ PreviewMouseMove��61   . 5d   &      6>   �� *
IMouseMove+5e   #     5f   "   $ PreviewMouseDown��61   . 5g   &      6>   �� *IMouseClick+5h   #     5i   "   $ PreviewMouseWheel��61   . 5j   &      6>   �� *IMouseWheelCommand+5k   #   5m      5n      5o      5p      5q      5r      5s      