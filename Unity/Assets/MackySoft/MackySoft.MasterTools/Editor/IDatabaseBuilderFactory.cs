using System;

namespace MackySoft.MasterTools
{
	public interface IDatabaseBuilderFactory
	{
		IDatabaseBuilder Create (BuildContext context);
	}

	public static class DatabaseBuilderFactory
	{
		public static IDatabaseBuilderFactory Create (Func<BuildContext, IDatabaseBuilder> factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException(nameof(factory));
			}
			return new AnonymousDatabaseBuilderFactory(factory);
		}

		sealed class AnonymousDatabaseBuilderFactory : IDatabaseBuilderFactory
		{
			readonly Func<BuildContext, IDatabaseBuilder> m_Factory;

			public AnonymousDatabaseBuilderFactory (Func<BuildContext, IDatabaseBuilder> factory)
			{
				m_Factory = factory;
			}

			public IDatabaseBuilder Create (BuildContext context)
			{
				return m_Factory(context);
			}
		}
	}
}