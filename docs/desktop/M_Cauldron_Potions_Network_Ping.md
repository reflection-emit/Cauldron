# Network.Ping Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Allows an application to determine whether a remote computer is accessible over the network.

**Namespace:**&nbsp;<a href="N_Cauldron_Potions">Cauldron.Potions</a><br />**Assembly:**&nbsp;Cauldron.Potions (in Cauldron.Potions.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public Task<PingResults> Ping(
	string hostname,
	uint port
)
```


#### Parameters
&nbsp;<dl><dt>hostname</dt><dd>Type: System.String<br />The hostname of the remote computer</dd><dt>port</dt><dd>Type: System.UInt32<br />The port to ping</dd></dl>

#### Return Value
Type: Task(<a href="T_Cauldron_Potions_PingResults">PingResults</a>)<br />An object that represents the ping results

#### Implements
<a href="M_Cauldron_Potions_INetwork_Ping">INetwork.Ping(String, UInt32)</a><br />

## See Also


#### Reference
<a href="T_Cauldron_Potions_Network">Network Class</a><br /><a href="N_Cauldron_Potions">Cauldron.Potions Namespace</a><br />