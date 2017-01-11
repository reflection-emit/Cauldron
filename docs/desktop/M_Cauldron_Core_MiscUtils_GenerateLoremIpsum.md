# MiscUtils.GenerateLoremIpsum Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Generates a random lorem ipsum text

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static string GenerateLoremIpsum(
	int minWords,
	int maxWords,
	int minSentences = 1,
	int maxSentences = 1,
	uint paragraphCount = 1
)
```


#### Parameters
&nbsp;<dl><dt>minWords</dt><dd>Type: System.Int32<br />The minimum word count to generate</dd><dt>maxWords</dt><dd>Type: System.Int32<br />The maximum word count to generate</dd><dt>minSentences (Optional)</dt><dd>Type: System.Int32<br />The minimum sentence count to generate</dd><dt>maxSentences (Optional)</dt><dd>Type: System.Int32<br />The maximum sentence count to generate</dd><dt>paragraphCount (Optional)</dt><dd>Type: System.UInt32<br />The number of paragraphs to generate</dd></dl>

#### Return Value
Type: String<br />The generated lorem ipsum text

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>*minWords* is 0</td></tr><tr><td>ArgumentException</td><td>*minSentences* is 0</td></tr><tr><td>ArgumentException</td><td>*paragraphCount* is 0</td></tr><tr><td>ArgumentException</td><td>*minWords* is greater than *maxWords*</td></tr><tr><td>ArgumentException</td><td>*minSentences* is greater than *maxSentences*</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_MiscUtils">MiscUtils Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />