using System;
using System.Linq;
using UnityEditor;

namespace UINavigation.Editor
{
	public static class ViewIdProvider
	{
		private static string[] cache;
		private static double lastScan;

		public static string[] GetAll()
		{
			// Cache: re-scan every 2s
			if (cache != null && EditorApplication.timeSinceStartup - lastScan < 2.0)
				return cache;

			lastScan = EditorApplication.timeSinceStartup;
			cache = ViewIdScanner.ScanAllViewIds();
			return cache;
		}
		
		public static int IndexOf(string[] array, string value)
		{
			for (var i = 0; i < array.Length; i++)
			{
				if (string.Equals(array[i], value, StringComparison.Ordinal))
				{

					return i;
				}
			}
			return 0;
		}

		public static bool Contains(string[] array, string value) => array.Any(t => string.Equals(t, value, StringComparison.Ordinal));
	}
}