using System;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
	[Serializable]
	public class Normal : Deferred
	{
		public ShapePropertyGroup shape;

		public NormalPropertyGroup normal;

		protected Material[] deferredMaterials;

		private Material Mobile => MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Normal"));

		public override Material MobileDeferredOpaque => Mobile;

		public override Material MobileDeferredTransparent => Mobile;

		private Material Standard => MaterialFromShader(Shader.Find("Projection/Decal/Standard/Normal"));

		public override Material StandardDeferredOpaque => Standard;

		public override Material StandardDeferredTransparent => Standard;

		private Material Packed => MaterialFromShader(Shader.Find("Projection/Decal/Packed/Normal"));

		public override Material PackedDeferredOpaque => Packed;

		public override Material PackedDeferredTransparent => Packed;

		public override int InstanceLimit => 500;

		protected override void Apply(Material Material)
		{
			base.Apply(Material);
			shape.Apply(Material);
			normal.Apply(Material);
		}

		protected override void OnEnable()
		{
			if (shape == null)
			{
				shape = new ShapePropertyGroup(this);
			}
			if (normal == null)
			{
				normal = new NormalPropertyGroup(this);
			}
			base.OnEnable();
		}

		protected override void GenerateIDs()
		{
			base.GenerateIDs();
			shape.GenerateIDs();
			normal.GenerateIDs();
		}

		public override void UpdateProperties()
		{
			if (properties == null || properties.Length != 2)
			{
				properties = new ProjectionProperty[2];
			}
			properties[0] = new ProjectionProperty("Opacity", shape._Multiplier, shape.Multiplier);
			properties[1] = new ProjectionProperty("Normal Strength", normal._BumpScale, normal.Strength);
		}
	}
}
