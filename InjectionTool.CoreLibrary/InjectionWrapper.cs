using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace InjectionTool.CoreLibrary {
	public class InjectionWrapper {

		#region Privlages
		private const int PROCESS_CREATE_THREAD = 0x0002;
		private const int PROCESS_QUERY_INFORMATION = 0x0400;
		private const int PROCESS_VM_OPERATION = 0x0008;
		private const int PROCESS_VM_WRITE = 0x0020;
		private const int PROCESS_VM_READ = 0x0010;
		#endregion
		#region Memory Allocation Constants
		private const uint MEM_COMMIT = 0x00001000;
		private const uint MEM_RESERVE = 0x00002000;
		private const uint PAGE_READWRITE = 4;
		#endregion

		/// <summary>
		/// Injects a dll into a process by creating a remote thread
		/// </summary>
		/// <param name="processName">Name of the process to inject into</param>
		/// <param name="dllPath">Path to the dll to inject into the process</param>
		/// <returns></returns>
		public static int Inject(string processName, string dllPath) {

			Process targetProcess = Process.GetProcessesByName(processName)[0];

			IntPtr handle = OpenProcess(
				PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ,
				false, targetProcess.Id
			);


			IntPtr allocAddress = VirtualAllocEx(
				handle, 
				IntPtr.Zero, 
				(uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), 
				MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE
			);

			UIntPtr allocBytes;

			WriteProcessMemory(
				handle, 
				allocAddress, 
				Encoding.Default.GetBytes(dllPath), (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), 
				out allocBytes
			);

			CreateRemoteThread(handle, IntPtr.Zero, 0, LoadLibrary(dllPath), allocAddress, 0, IntPtr.Zero);

			return 0;
		}

		#region Dll Imports
		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
			uint dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll")]
		private static extern IntPtr CreateRemoteThread(IntPtr hProcess,
			IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
		static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
		#endregion

	}
}
