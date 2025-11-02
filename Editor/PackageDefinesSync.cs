using System;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.Build;
#endif

namespace UINavigation.Editor
{
    [InitializeOnLoad]
    public static class PackageDefinesSync
    {
        private const string PackageName = "com.ericbatlle.ui-navigation-system";
        private const string Define = "UNITASK_DOTWEEN_SUPPORT";
        private const string PreviousRequiredDefine = "DOTWEEN";

        static PackageDefinesSync()
        {
            // Post-import fallback: Ensures the define on each domain reload.
            // This runs after the package is imported for the first time.
            EditorApplication.delayCall += EnsureDefinePresent;

            // Package Manager events to add/remove in updates and, above all, remove when uninstalling.
            Events.registeringPackages -= OnRegisteringPackages;
            Events.registeringPackages += OnRegisteringPackages;

            Events.registeredPackages  -= OnRegisteredPackages;
            Events.registeredPackages  += OnRegisteredPackages;
        }

        private static void EnsureDefinePresent()
        {
            AddDefineToAllGroups();
        }

        private static void OnRegisteringPackages(PackageRegistrationEventArgs args)
        {
            // Before applying changes (our code still exists)
            var ourPackageWillBeRemoved = args.removed.Any(p => p.name == PackageName);
            if (ourPackageWillBeRemoved)
                RemoveDefineFromAllGroups();
        }

        private static void OnRegisteredPackages(PackageRegistrationEventArgs args)
        {
                // After applying changes: useful in updates or if Unity triggered this after compiling.
                var ourPackageJustAddedOrUpdated =
                args.added.Any(p => p.name == PackageName) ||
                args.changedFrom.Any(p => p.name == PackageName) ||
                args.changedTo.Any(p => p.name == PackageName);

            if (ourPackageJustAddedOrUpdated)
                AddDefineToAllGroups();
        }

#if UNITY_2021_2_OR_NEWER
        private static void AddDefineToAllGroups()
        {
            foreach (BuildTargetGroup g in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (g == BuildTargetGroup.Unknown || IsObsolete(g)) continue;
                var nbt = NamedBuildTarget.FromBuildTargetGroup(g);
                if (nbt == NamedBuildTarget.Unknown) continue;

                var defines = PlayerSettings.GetScriptingDefineSymbols(nbt);
                var list = defines.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (list.Contains(PreviousRequiredDefine) && !list.Contains(Define))
                    PlayerSettings.SetScriptingDefineSymbols(nbt, string.Join(";", list.Append(Define)));
            }
        }

        private static bool IsObsolete(Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null) return false;

            var mem = type.GetMember(name);
            if (mem.Length == 0) return false;

            return Attribute.IsDefined(mem[0], typeof(ObsoleteAttribute));
        }

        private static void RemoveDefineFromAllGroups()
        {
            foreach (BuildTargetGroup g in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (g == BuildTargetGroup.Unknown) continue;
                var nbt = NamedBuildTarget.FromBuildTargetGroup(g);
                if (nbt == NamedBuildTarget.Unknown) continue;

                var defines = PlayerSettings.GetScriptingDefineSymbols(nbt);
                var list = defines.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (list.RemoveAll(d => d == Define) > 0)
                    PlayerSettings.SetScriptingDefineSymbols(nbt, string.Join(";", list));
            }
        }
#else
        private static void AddDefineToAllGroups()
        {
            foreach (BuildTargetGroup g in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (g == BuildTargetGroup.Unknown) continue;
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(g);
                var list = defines.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

               if (list.Contains(PreviousRequiredDefine) && !list.Contains(Define))
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(g, string.Join(";", list.Append(Define)));
            }
        }

        private static void RemoveDefineFromAllGroups()
        {
            foreach (BuildTargetGroup g in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (g == BuildTargetGroup.Unknown) continue;
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(g);
                var list = defines.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (list.RemoveAll(d => d == Define) > 0)
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(g, string.Join(";", list));
            }
        }
#endif
    }
}
