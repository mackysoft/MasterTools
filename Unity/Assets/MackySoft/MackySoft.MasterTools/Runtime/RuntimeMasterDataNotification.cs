using System;

namespace MackySoft.MasterTools
{

	/// <summary>
	/// <para> This class provides an event that is called when master data import is complete. </para>
	/// <para> The functionality of this can be used in a runtime assembly so that changes to the master data can be received at runtime and applied to the application. </para>
	/// </summary>
	public static class RuntimeMasterDataNotification
	{

		/// <summary>
		/// Event called when master data import is complete.
		/// </summary>
		public static event Action OnImported;

		internal static void NotifyImported ()
		{
			OnImported?.Invoke();
		}
	}
}
