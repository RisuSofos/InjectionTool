using System;
using InjectionTool.Library;

namespace InjectionTool.Client {
	internal class Program {
		public static void Main(string[] args) {
			int passed = InjectDll(args[0], args[1]);

			Console.WriteLine(passed);
		}

		/// <summary>
		/// Temprary function to test the InjectionWrapper
		/// </summary>
		/// <param name="process"></param>
		/// <param name="dllPath"></param>
		/// <returns></returns>
		private static int InjectDll(string process, string dllPath) => InjectionWrapper.Inject(process, dllPath);

	}
}
