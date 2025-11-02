using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace UINavigation.Editor
{
	internal static class ViewIdScanner
	{
		public static string[] ScanAllViewIds()
		{
			var types = GetAttributedAndConventionalTypes();

			var ids = new List<string>(64);
			foreach (var t in types)
			{
				ExtractConstStrings(t, ids);
			}
			
			return ids
				.Where(s => !string.IsNullOrEmpty(s))
				.Distinct(StringComparer.Ordinal)
				.OrderBy(s => s, StringComparer.Ordinal)
				.Prepend("")
				.ToArray();
		}
		
		private static IEnumerable<Type> GetAttributedAndConventionalTypes()
		{
			var result = new List<Type>();

			// Types with [ViewIdsContainer] attribute
			var attributed = TypeCache.GetTypesWithAttribute<ViewIdsContainerAttribute>();
			result.AddRange(attributed);

			// Static types with names like ViewId or ViewIds
			var conventional = FindConventionalTypes(new[] { "ViewId", "ViewIds" });
			result.AddRange(conventional);

			return result.Distinct();
		}
		
		private static void ExtractConstStrings(Type t, List<string> acc)
		{
			const BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
			foreach (var f in t.GetFields(flags))
			{
				if (f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
				{
					var s = (string)f.GetRawConstantValue();
					if (!string.IsNullOrEmpty(s)) acc.Add(s);
				}
			}
		}
		
		private static IEnumerable<Type> FindConventionalTypes(IEnumerable<string> typeNames)
		{
			var names = new HashSet<string>(typeNames ?? Array.Empty<string>(), StringComparer.Ordinal);
			var result = new List<Type>();
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type[] types;
				try { types = asm.GetTypes(); }
				catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }

				foreach (var t in types)
				{
					// static class: abstract + sealed
					if (t is not { IsClass: true } || !t.IsAbstract || !t.IsSealed)
						continue;

					if (names.Contains(t.Name))
						result.Add(t);
				}
			}
			return result;
		}
	}
}