using System;

namespace MackySoft.MasterTools
{
	public interface IMasterBuilderProcessor
	{
		IDatabaseBuilder Setup (BuildContext context);
	}

	public static class MasterBuilderProcessor
	{
		public static IMasterBuilderProcessor Create (Func<BuildContext, IDatabaseBuilder> setup)
		{
			if (setup == null)
			{
				throw new ArgumentNullException(nameof(setup));
			}
			return new AnonymousMasterBuilderProcessor(setup);
		}

		sealed class AnonymousMasterBuilderProcessor : IMasterBuilderProcessor
		{

			readonly Func<BuildContext, IDatabaseBuilder> m_Setup;

			public AnonymousMasterBuilderProcessor (Func<BuildContext, IDatabaseBuilder> setup)
			{
				m_Setup = setup;
			}

			public IDatabaseBuilder Setup (BuildContext context)
			{
				return m_Setup(context);
			}
		}
	}
}