# DynamicAssembly.EmitCall Method (ILGenerator, OpCode, Type, String, Type[], Boolean, BindingFlags)
 _**\[This is preliminary documentation and is subject to change.\]**_

\[Missing <summary> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitCall(System.Reflection.Emit.ILGenerator,System.Reflection.Emit.OpCode,System.Type,System.String,System.Type[],System.Boolean,System.Reflection.BindingFlags)"\]

**Namespace:**&nbsp;<a href="N_Cauldron_Dynamic">Cauldron.Dynamic</a><br />**Assembly:**&nbsp;Cauldron.Dynamic (in Cauldron.Dynamic.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void EmitCall(
	this ILGenerator il,
	OpCode opcode,
	Type type,
	string methodOrPropertyName,
	Type[] parameterTypes,
	bool boxResult,
	BindingFlags bindingFlags = BindingFlags.Default|BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic
)
```


#### Parameters
&nbsp;<dl><dt>il</dt><dd>Type: System.Reflection.Emit.ILGenerator<br />\[Missing <param name="il"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitCall(System.Reflection.Emit.ILGenerator,System.Reflection.Emit.OpCode,System.Type,System.String,System.Type[],System.Boolean,System.Reflection.BindingFlags)"\]</dd><dt>opcode</dt><dd>Type: System.Reflection.Emit.OpCode<br />\[Missing <param name="opcode"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitCall(System.Reflection.Emit.ILGenerator,System.Reflection.Emit.OpCode,System.Type,System.String,System.Type[],System.Boolean,System.Reflection.BindingFlags)"\]</dd><dt>type</dt><dd>Type: System.Type<br />\[Missing <param name="type"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitCall(System.Reflection.Emit.ILGenerator,System.Reflection.Emit.OpCode,System.Type,System.String,System.Type[],System.Boolean,System.Reflection.BindingFlags)"\]</dd><dt>methodOrPropertyName</dt><dd>Type: System.String<br />\[Missing <param name="methodOrPropertyName"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitCall(System.Reflection.Emit.ILGenerator,System.Reflection.Emit.OpCode,System.Type,System.String,System.Type[],System.Boolean,System.Reflection.BindingFlags)"\]</dd><dt>parameterTypes</dt><dd>Type: System.Type[]<br />\[Missing <param name="parameterTypes"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitCall(System.Reflection.Emit.ILGenerator,System.Reflection.Emit.OpCode,System.Type,System.String,System.Type[],System.Boolean,System.Reflection.BindingFlags)"\]</dd><dt>boxResult</dt><dd>Type: System.Boolean<br />\[Missing <param name="boxResult"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitCall(System.Reflection.Emit.ILGenerator,System.Reflection.Emit.OpCode,System.Type,System.String,System.Type[],System.Boolean,System.Reflection.BindingFlags)"\]</dd><dt>bindingFlags (Optional)</dt><dd>Type: System.Reflection.BindingFlags<br />\[Missing <param name="bindingFlags"/> documentation for "M:Cauldron.Dynamic.DynamicAssembly.EmitCall(System.Reflection.Emit.ILGenerator,System.Reflection.Emit.OpCode,System.Type,System.String,System.Type[],System.Boolean,System.Reflection.BindingFlags)"\]</dd></dl>

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type ILGenerator. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Dynamic_DynamicAssembly">DynamicAssembly Class</a><br /><a href="Overload_Cauldron_Dynamic_DynamicAssembly_EmitCall">EmitCall Overload</a><br /><a href="N_Cauldron_Dynamic">Cauldron.Dynamic Namespace</a><br />