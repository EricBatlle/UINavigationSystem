using System;
using UnityEngine;

namespace UINavigation
{
	[Serializable]
	public struct ViewId
	{
		[SerializeField] private string value;

		public override string ToString() => value ?? string.Empty;
		
		public static implicit operator string(ViewId v) => v.value;
		public static implicit operator ViewId(string s) => new ViewId { value = s };
	}
}