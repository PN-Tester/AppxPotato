# Local Privilege Escalation to SYSTEM via AppX RPC coercion
This tool leverages a vulnerability I discovered in Windows 10 (and possibly more) affecting the AppX Local RPC interface. Specifically, this affects AppXDeploymentServer.dll which exposes the ae2dc901-312d-41df-8b79-e835e63db874 Local RPC Interface by default on modern Windows. The exploit leverages the AppXApplyTrustLabelToFolder_58() method available on this interface. The method takes a UNC Path to a folder as an argument, which results in NT AUTHORITY/SYSTEM account interaction with the supplied folder. This tool will create a named pipe controlled by the user and supply it as an argument to the vulnerable method. If the user has SeImpersonatePrivilege or equivalent, then the tool will spawn a new cmd.exe window as SYSTEM via the well-known impersonateNamedPipeClient technique (duplicating the client's token and using it to spawn a new process).

# Compiling
Ideally, you can just git clone the repo and build the project in VS Studio 2019 or higher. The project uses the NtAPIDotNet class library from James Forshaw to interact with the RPC interface. The interface class file itself was generated with NtObjectManager.


# Blog Post Coming Soon...


