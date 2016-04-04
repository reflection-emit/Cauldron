# FactoryCreationPolicy Enumeration
 _**\[This is preliminary documentation and is subject to change.\]**_

Describes the creation policy of an object through the <a href="T_Couldron_Factory">Factory</a>

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public enum FactoryCreationPolicy
```


## Members
&nbsp;<table><tr><th></th><th>Member name</th><th>Value</th><th>Description</th></tr><tr><td /><td target="F:Couldron.FactoryCreationPolicy.Instanced">**Instanced**</td><td>0</td><td>Always creates a new instance. 

 Instances are not managed by the <a href="T_Couldron_Factory">Factory</a>. 

 Default policy</td></tr><tr><td /><td target="F:Couldron.FactoryCreationPolicy.Singleton">**Singleton**</td><td>1</td><td>Only a single instance is created and reused everytime. 

 Instances are managed by the <a href="T_Couldron_Factory">Factory</a></td></tr></table>

## See Also


#### Reference
<a href="N_Couldron">Couldron Namespace</a><br />