   M S B A M L   `   `   `  ����D  @TinyBombe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:TinyBombe	TinyBombe  5   	       TinyBombe.MainWindow   5   	   W SPresentationCore, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:TinyBombe     5   	     � Title$&   Pete's 8-Letter Bombe Visualizer��5   	   $	��800��62   $
��1200��6?   .��5	      � -   5         �Name$ mainGrid��5	      .+�5
   
   o� -   5         o�Name$ mainMenu��5
      $��18��6!   $	��173��6-   $
��Left=�69   $	��Top=�6T   $��0,0,0,0q�6l   .@�5      m� $��	Diagonals��6   .@�5      �� -   5         ��Name$ useDiagonals��5      $#��Use Welchman's Diagonal Board��62    d� 	IsChecked$
 TrueY�6Z   6m   m� 5      -   5         m�Name$ 
btnOpenAll��5      $��Open All��60   m� 5      -   5        $ btnCloseAll��5      $��	Close All��61   m� 5      -   5        $ btnToggleAll��5      $��
Toggle All��62   5      m� 5      $��Examples��6   m� 5      $
��Help��6   5      �� 5   
   -   5         ��Name$ tbWindow��5      $
��Left=�6$   $��20��6?   $��
503,32,0,0q�6K    �� TextWrapping$
 Wrap=�6_   $	��AAA��6s    �� CharacterCasing$ Upper=�6~   $	��Top=�6�   $��36��6�   6�   �� 5   
   -   5        	 ��Name$	 btnBack��5      $��-��6"   $
��Left=�6.   $��
479,32,0,0q�6I   $	��Top=�6]   $��20��6w   6�   �� 5   
   -	   5        $	 btnFwd��5      $��+��6!   $
��Left=�6-   $��
543,32,0,0q�6H   $	��Top=�6\   $��20��6u   6�   �� 5   
   -
   5        
 ��Name$
 btnCrib��5      $��Crib:��6"   $
��Left=�62   $��	33,78,0,0q�6M   $��0q�6`   $	��Top=�6o   $��18��6�   �� 5   
   $��
Intercept:��6   $
��Left=�6&   $��	10,58,0,0q�6A   $��0q�6T   $	��Top=�6c   $��18��6{   �� 5   
   -   5        $ cbReplaceSpaces��5      $��Replace 'G's by spaces��6,   $
��Left=�6M   $��
619,11,0,0q�6h   $	��Top=�6|   $
 TrueY�6�   $	��141��6�   �� 5   
   -   5        $��Scan All��5      $��0q�6&   $
��Left=�63   $��
189,32,0,0q�6P   $	��Top=�6d   $��59��6|   6�   �� 5   
   -   5        $	 	btnResume��5      $��Resume��6$   $��0q�66   $
��Left=�6C   $��
315,32,0,0q�6`   $	��Top=�6t   $��55��6�   6�   �� 5    
   -   5        $	 btnPause��5       $��Pause��6#   $��0q�64   $
��Left=�6A   $��
254,32,0,0q�6^   $	��Top=�6r   $��55��6�   6�   �� 5!   
   -   5        $
 RecoveredWithPlugs��5!      $��Hello, World!��6.   $
��Left=�6F   $��
778,35,0,0q�6a   $��0q�6u   $	��Top=�6�   $��18��6�   $��Consolas)�6�   $
��Bold"�6�   $��14(�6�   �� 5"   
   -   5        $ PlugGuesses��5"      $
��Left=�6'   $��19��6B   $�� ��6N   $ Upper=�6X   $��
664,32,0,0q�6q   $	��Top=�6�   $��97��6�   $��Consolas)�6�   $
��Bold"�6�   $��14(�6�   6�   �� 5$   
   -   5        $	 btnRandomPuzzle��5$      $��Generate Random puzzle��6*   $��0q�6L   $
��Left=�6Y   $��19��6t   $��	11,33,0,0q�6�   $	��Top=�6�   $	��149��6�   6�   �� 5%   
   -   5        $	 btnHint��5%      $
��Hint��6"   $
��Left=�61   $��
394,32,0,0q�6L   $	��Top=�6`   $��39��6x   6�   �� 5&   
   $��Recovered Message��6   $
��Left=�6-   $��	773,6,0,0q�6H   $	��Top=�6[   $
��Bold"�6s   5)      5*      