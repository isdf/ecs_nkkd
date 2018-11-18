	using Unity.Entities;

	/// <summary>
	/// キャラのモーション
	/// </summary>
	public struct CharaMotion : IComponentData
	{
		public int motionNo;
		public int count;
	}