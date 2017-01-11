# DynamicAssembly.EmitIfElse Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

\[Missing <summary> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitIfElse(System.Reflection.Emit.ILGenerator,System.Action{System.Reflection.Emit.Label,System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label})"\]

**Namespace:**&nbsp;<a href="N_Cauldron_Dynamic">Cauldron.Dynamic</a><br />**Assembly:**&nbsp;Cauldron.Dynamic (in Cauldron.Dynamic.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void EmitIfElse(
	this ILGenerator il,
	Action<Label, Label> condition,
	Action<Label> ifStatement,
	Action<Label> elseStatement
)
```


#### Parameters
&nbsp;<dl><dt>il</dt><dd>Type: System.Reflection.Emit.ILGenerator<br />\[Missing <param name="il"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitIfElse(System.Reflection.Emit.ILGenerator,System.Action{System.Reflection.Emit.Label,System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label})"\]</dd><dt>condition</dt><dd>Type: System.Action(Label, Label)<br />\[Missing <param name="condition"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitIfElse(System.Reflection.Emit.ILGenerator,System.Action{System.Reflection.Emit.Label,System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label})"\]</dd><dt>ifStatement</dt><dd>Type: System.Action(Label)<br />\[Missing <param name="ifStatement"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitIfElse(System.Reflection.Emit.ILGenerator,System.Action{System.Reflection.Emit.Label,System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label})"\]</dd><dt>elseStatement</dt><dd>Type: System.Action(Label)<br />\[Missing <param name="elseStatement"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitIfElse(System.Reflection.Emit.ILGenerator,System.Action{System.Reflection.Emit.Label,System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label},System.Action{System.Reflection.Emit.Label})"\]</dd></dl>

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type ILGenerator. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Dynamic_DynamicAssembly">DynamicAssembly Class</a><br /><a href="N_Cauldron_Dynamic">Cauldron.Dynamic Namespace</a><br />