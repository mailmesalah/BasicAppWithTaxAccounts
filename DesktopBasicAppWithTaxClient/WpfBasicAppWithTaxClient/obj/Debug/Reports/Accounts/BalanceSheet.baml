   M S B A M L   `   `   `  ????O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	    WpfClientApp0   *WpfClientApp.Reports.Accounts.BalanceSheet   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	   -   5          ?Name$  BalanceSheetWindow??5   	    ? Title$ Balance Sheet??5   	   $	??551??6   $	??801??6,   $??Century Gothic)?68   ???	#FF101040 ? WindowStartupLocation$ CenterScreen=?6k    ? WindowStyle$ 
ToolWindow=?6?    ? Topmost . &   System.Windows.Media.TextOptions   TextFormattingMode$ Display=?6?   $??14(?6?   
???????$??SemiBold"?6  
???????$??2q?6,   ? 
ResizeMode$ NoResize=?6@  .??5      ? $??0,0,0,0q?6   $??Right=?6   .+?5	   
   (  "System.Windows.Controls.DatePicker  -   5          Name$ mDTPDate??5	      
 ?? Now	   SelectedDate#	 Z 6'   $
??Left=?6R   $??-146,17,0,0q?6m   $	??Top=?6?   $	??134??6?   
   IsTodayHighlighted
 . $??0??6?   ???	#FF101040
??????? ? 	Resources 5
      :  4System.Windows.Controls.Primitives.DatePickerTextBox ?? 
TargetType ?? Property ?? Value??%Z   '        ?? 5      "  6   .?5      ??  ? ?? ?	#FF10104063   5      5      5      &   System.Windows.Controls.DataGrid  5   
   -   5        $??mDataGridBGroup??5         CanUserAddRows .     HeadersVisibility$ Column=?6A      CanUserReorderColumns .     CanUserResizeColumns .     CanUserResizeRows .     CanUserSortColumns .     SelectionMode$ Single=?6?      SelectionUnit$ FullRow=?6?   !   EnableColumnVirtualization .     EnableRowVirtualization .     AutoGenerateColumns .  $
??Left=?6[  $??-768,79,0,0q?6v  $	??Top=?6?  $	??757??6?  $	??372??6?  $??Normal"?6?  
??????????	#FF101040   ColumnHeaderHeight$ 30Q?6?  $   HorizontalScrollBarVisibility$ Hidden=?6   5      =  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell??%?   '        '  x     ?? 5      "  6   .?5      ??  ? ?? ?	#FF10104063   ?? 5       ? ??$ Center=?6C   5      ?? 5      "  6   .?5      ??  ? ?? ?	#FF10104063   5      5         Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Asset??6%      
IsReadOnly .    Width5	  /System.Windows.Controls.DataGridLengthConverter$
 200*	 6`   1
  +System.Windows.Controls.DataGridBoundColumn  
  Binding  64   ?? *Asset+6`     5      $ Amount??6%    . $	 150	 6w     65   ?? *AssetAmount+! ?? StringFormat$! N2??" 
  ElementStyle" 5      ?? 5       " ??6    .?5!      ??  ? ??$ Right=?6D   5"      5$        5%      $ Liabilities??6%    . $
 200*	 6l     6:   ?? *Liabilities+6l     5&      $ Amount??6%    . $	 150	 6|     65   ?? *LiabilityAmount+$! N2??" 5'      ?? 5(      " ??6    .?5)      ??  ? ??$ Right=?6D   5*      5,        5-      $ 	AssetCode??6%    . #   
Visibility$# Hidden=?6i     68   ?? *	AssetCode+6i     5.      $ LiabilityCode??6%    . $# Hidden=?6q     6<   ?? *LiabilityCode+6q   50      ?? 52   
   -   5        $??mButtonPrint??52      $??Print??6%   $
??Left=?65   $??-768,472,0,0q?6P   $	??Top=?6f   $	??114??6~   $??35??6?   $??1??6?   ?? 53   
   -   5        $??mButtonClose??53      $??Close??6%   $
??Left=?65   $??-126,472,0,0q?6P   $	??Top=?6f   $	??114??6~   $??35??6?   $??2??6?   6?   55      56      