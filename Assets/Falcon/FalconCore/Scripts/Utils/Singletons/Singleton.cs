namespace Falcon.FalconCore.Scripts.Utils.Singletons
{
	public class Singleton<T> where T : Singleton<T>, new()
	{
		private static T _instance;

		public static T Instance => _instance = _instance ?? new T();

		protected Singleton()
		{
		}
	}
}