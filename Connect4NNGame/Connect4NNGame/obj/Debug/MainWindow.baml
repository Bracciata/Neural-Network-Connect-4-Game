   M S B A M L   `   `   `  ����I  EConnect4NNGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:Connect4NNGameConnect4NNGame  5   	       Connect4NNGame.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:Connect4NNGame     5   	     � Title$  Connnect Four��5   	   $	��350��6   $	��525��6,    � 	Resources 5	       �� 
TargetType 
  Button �� Property �� Value �� 
TargetType �� Triggers P� Property P� Value��%@  &	        �� 5
   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   ._�5      �� $��Stretch=�6/   $��Stretch=�6M   5      5      5       5      P� 5       �@J�IsMouseOver . . �5      ��  � ��
 � ���63   5      5      5      .��5      � -   5        $
��grid��5      	 � RowDefinitions	 5      �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   .+�5!   
   �� -   5        $��
gameCanvas��5!      $��0��6#   5#      5$      5&      