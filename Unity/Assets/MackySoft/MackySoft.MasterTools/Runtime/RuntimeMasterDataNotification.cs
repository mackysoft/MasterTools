using System;

namespace MackySoft.MasterTools
{
	public static class RuntimeMasterDataNotification
	{

		public static event Action OnImported;

		internal static void NotifyImported ()
		{
			OnImported?.Invoke();
		}
	}
}
