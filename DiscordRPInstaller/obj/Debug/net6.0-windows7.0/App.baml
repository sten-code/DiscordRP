   M S B A M L   `   `   `  ����M  IDiscordRPInstaller, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:DiscordRPInstallerDiscordRPInstaller  5          DiscordRPInstaller.App   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      2local clr-namespace:DiscordRPInstaller     5        �� 	Resources  5         Whitney-Light  Whitney-Book  Whitney-Medium  Whitney-SemiBold  Whitney-Bold  Whitney-Black �� 
TargetType  CloseButton �� Property �� Value �� 
TargetType ��Name'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard	 �� 
TargetName
 �� TargetProperty	 �� To g� Duration F�Name �� Stretch  DiscordButton �� CornerRadius  DiscordTextBox �� Color ��Name 	 DiscordComboBox��%�  &	        &	 >     &	 z     &	 �     &	 �     &	 3    &	 p    &	     7  &	 	    7  &		      7  *�5   
   *&/Fonts/whitney-light.otf#Whitney Light)�6S   *�5   
   ($/Fonts/whitney-book.otf#Whitney Book)�6P   *�5	   
   %!/Fonts/whitney-medium.otf#Whitney)�6O   *�5
   
   0,/Fonts/whitney-semibold.otf#Whitney Semibold)�6\   *�5   
   ($/Fonts/whitney-bold.otf#Whitney Bold)�6P   *�5   
   )%/Fonts/whitney-bold.otf#Whitney Black)�6R   �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      �� $ Border��6!   $��24��5   !   $��24��6,   
������ $��0q�5   !    5        5   "   CommonStates��. 5   &     
Normal��. 5   *   �� .��5   .   �� $
	 Path��6=   $)
 #(Path.Fill).(SolidColorBrush.Color)�5   =    #73767a5   =   $ 	0:0:0.125D�5   =   5   +   5   '     5    &   	MouseOver��. 5!   *   �� .��5"   .   �� $
	 Path��6=   $)
 #(Path.Fill).(SolidColorBrush.Color)�5#   =    #dcddde5$   =   $ 	0:0:0.125D�5%   =   5&   +   5'   '   5(   #   ._�5*      � $��15��6#   $��15��6.   $��Center=�5+   #   $��Center=�6>   .+�5,   "   F� $
 Path��6'   |���� � 	= � �  �� � sU  	= � 	=  sU � ��  � � 	=  �� sU  -1� �  �� � � -1� -1 �� ��  � � -1 sU � � 	= 	���#73767a$
 Fill=�5/   '   50      51      52      54      55      �� 57   
   " ��6   .�58      ��  � �� �#dcddde67   �� 59       � ��8   67   �� 5:       � ��$ 16(�65   �� 5;       � �� 5<      �� 5=      " ��6&   .R�5>      �� $ Border��6!   $ 4��5?   !   ���#5865f2 5A        5B   "   CommonStates��. 5C   &     
Normal��. 5D   *   �� .��5E   .   �� $	 Border��6=   $1
 +(Border.Background).(SolidColorBrush.Color)�5F   =    #5865f25G   =   $ 	0:0:0.125D�5H   =   5I   +   5J   '     5K   &   	MouseOver��. 5L   *   �� .��5M   .   �� $	 Border��6=   $1
 +(Border.Background).(SolidColorBrush.Color)�5N   =    #4752c45O   =   $ 	0:0:0.125D�5P   =   5Q   +   5R   '     5S   &   Pressed��. 5T   *   �� .��5U   .   �� $	 Border��6=   $1
 +(Border.Background).(SolidColorBrush.Color)�5V   =    #3c45a55W   =   $ 	0:0:0.125D�5X   =   5Y   +   5Z   '   5[   #   ._�5]      �� $��Center=�6/   $��Center=�6L   5^      5_      5a      5b      �� 5d   
   " ��6   .�5e      ��  � �� �#dcddde67   �� 5f       � ��8   67   �� 5g       � ��$ 16(�65   �� 5h       �@��
CaretBrush 5i      �� 5j      $ #dcddde��6&   5l      �� 5m       � �� 5n      �� 5o      " ��6&   .R�5p      �� $��Border��6!   $��0q�5q   !   $ 4��5r   !   $��40��5s   !   ���#202225._�5u      �� $ PART_ContentHost��5w   +   $��7,7,7,7q�5u   +   $��Center=�5v   +   5w   +   5x      5y      5{      5|      �� 5~   
   " ��6   .�5�      ��  � �� 5�      �� 5�      " ��6&   .R�5�      �� $��0q�6!   $ 4��5�   !   $��40��5�   !   ���#202225._�5�      �� 
������ $��
very noice��5�   (   $��Center=�5�   (   ���#dcddde$��16(�5�   (   8��  $��8,7,7,7q�5�   (   5�      5�      5�      5�      5�      5�      