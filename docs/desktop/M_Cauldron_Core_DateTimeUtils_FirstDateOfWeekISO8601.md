# DateTimeUtils.FirstDateOfWeekISO8601 Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns the DateTime representation of the year and week of year

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static DateTime FirstDateOfWeekISO8601(
	int year,
	int weekOfYear
)
```


#### Parameters
&nbsp;<dl><dt>year</dt><dd>Type: System.Int32<br />The year to convert</dd><dt>weekOfYear</dt><dd>Type: System.Int32<br />The week of year to convert</dd></dl>

#### Return Value
Type: DateTime<br />\[Missing <returns> documentation for "M:Cauldron.Core.DateTimeUtils.FirstDateOfWeekISO8601(System.Int32,System.Int32)"\]

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentOutOfRangeException</td><td>Argument *weekOfYear* is more than the given year has weeks</td></tr><tr><td>ArgumentOutOfRangeException</td><td>Argument *weekOfYear* is lower than 0</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_DateTimeUtils">DateTimeUtils Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />