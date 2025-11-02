using System;

namespace UINavigation
{
	/// <summary>
	/// Mark static classes that expose public const string fields to be discovered as View IDs.
	/// Example:
	/// [ViewIdsContainer]
	/// public static class ViewId { public const string Lose = "Lose"; }
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class ViewIdsContainerAttribute : Attribute { }
}