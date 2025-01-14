namespace App
{
	public class WhoIsItemDefinition
	{
		public string rootTag;

		public string[] childTags;

		public bool hasChild;

		public bool hasColliderOnRoot;

		public WhoIsItemDefinition(string rootTag, string childTag = "", bool hasColliderOnRoot = false)
		{
			this.rootTag = rootTag;
			if (childTag.Length > 0)
			{
				childTags = new string[1]
				{
					childTag
				};
				hasChild = true;
			}
			else
			{
				childTags = new string[0];
			}
			this.hasColliderOnRoot = (hasColliderOnRoot || !hasChild);
		}

		public WhoIsItemDefinition(string rootTag, string[] childTags, bool hasColliderOnRoot = false)
		{
			this.rootTag = rootTag;
			this.childTags = childTags;
			hasChild = (childTags.Length != 0);
			this.hasColliderOnRoot = (hasColliderOnRoot || !hasChild);
		}
	}
}
