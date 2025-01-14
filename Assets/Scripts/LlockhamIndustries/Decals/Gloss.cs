using System;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
	[Serializable]
	public class Gloss : Deferred
	{
		[SerializeField]
		public GlossType glossType;

		public GlossPropertyGroup gloss;

		public GlossType GlossType
		{
			get
			{
				return glossType;
			}
			set
			{
				if (glossType != value)
				{
					glossType = value;
					Mark();
				}
			}
		}

		private Material Mobile
		{
			get
			{
				Shader p_Shader = null;
				switch (glossType)
				{
				case GlossType.Shine:
					p_Shader = Shader.Find("Projection/Decal/Mobile/Wet");
					break;
				case GlossType.Dull:
					p_Shader = Shader.Find("Projection/Decal/Mobile/Dry");
					break;
				}
				return MaterialFromShader(p_Shader);
			}
		}

		public override Material MobileDeferredOpaque => Mobile;

		public override Material MobileDeferredTransparent => Mobile;

		private Material Standard
		{
			get
			{
				Shader p_Shader = null;
				switch (glossType)
				{
				case GlossType.Shine:
					p_Shader = Shader.Find("Projection/Decal/Standard/Wet");
					break;
				case GlossType.Dull:
					p_Shader = Shader.Find("Projection/Decal/Standard/Dry");
					break;
				}
				return MaterialFromShader(p_Shader);
			}
		}

		public override Material StandardDeferredOpaque => Standard;

		public override Material StandardDeferredTransparent => Standard;

		private Material Packed
		{
			get
			{
				Shader p_Shader = null;
				switch (glossType)
				{
				case GlossType.Shine:
					p_Shader = Shader.Find("Projection/Decal/Packed/Wet");
					break;
				case GlossType.Dull:
					p_Shader = Shader.Find("Projection/Decal/Packed/Dry");
					break;
				}
				return MaterialFromShader(p_Shader);
			}
		}

		public override Material PackedDeferredOpaque => Packed;

		public override Material PackedDeferredTransparent => Packed;

		public override int InstanceLimit => 500;

		protected override void Apply(Material Material)
		{
			base.Apply(Material);
			gloss.Apply(Material);
		}

		protected override void OnEnable()
		{
			if (gloss == null)
			{
				gloss = new GlossPropertyGroup(this);
			}
			base.OnEnable();
		}

		protected override void GenerateIDs()
		{
			base.GenerateIDs();
			gloss.GenerateIDs();
		}

		public override void UpdateProperties()
		{
			if (properties == null || properties.Length != 1)
			{
				properties = new ProjectionProperty[1];
			}
			properties[0] = new ProjectionProperty("Glossiness", gloss._Glossiness, gloss.Glossiness);
		}
	}
}
