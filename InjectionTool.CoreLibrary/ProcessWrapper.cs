using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionTool.CoreLibrary {
	public class ProcessWrapper {
		public static IEnumerable<Process> GetProcesses(string processName) => Process.GetProcessesByName(processName).AsEnumerable();
	}
}
