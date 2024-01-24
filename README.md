# Local Privilege Escalation via AppX RPC coercion
This tool leverages a vulnerability I discovered in Windows 10/11 affecting the AppX MS-RPC interface. Specifically, this affects the AppX Deployment Service's AppXDeploymentServer.dll which exposes a Local RPC interface by default on Modern Windows workstations with UUID ae2dc901-312d-41df-8b79-e835e63db874. The exploit leverages the AppXApplyTrustLabelToFolder_58() method available on this interface on Windows 10, or the AppXSetTrustLabelOnPackage_59() method on Windows 11. The methods takes a UNC Path to a folder as an argument, which results in NT AUTHORITY\SYSTEM account interaction with the supplied folder. This tool will create a named pipe controlled by the user and supply it as an argument to the vulnerable methods. If the user has SeImpersonatePrivilege or equivalent, then the tool will spawn a new cmd.exe window as SYSTEM via the well-known impersonateNamedPipeClient technique (duplicating the client's token and using it to spawn a new process). This technique works on the latest Canary Channel build of Windows 11 Insider Preview as of 2024-01-23.

# Demo (win 11 AppXSetTrustLabelOnPackage_59 method)
![](https://github.com/PN-Tester/AppxPotato/blob/main/AppxPotatoDemo_Win11.gif)
# Demo (win 10 AppXApplyTrustLabelToFolder_58 method)
![](https://github.com/PN-Tester/AppxPotato/blob/main/AppxPotatoDemo.gif)

# Compiling
Ideally, you can just git clone the repo and build the project in VS Studio 2019 or higher. The project uses the NtAPIDotNet class library from James Forshaw to interact with the RPC interface. The interface class file itself was generated with NtObjectManager on win10 but I modified it to add the required method from the win11 interface so the projects works on both OS.


# Blog Post Coming Soon...


