using System;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
	[Serializable]
	public class Metallic : Projection
	{
		public AlbedoPropertyGroup albedo;

		public MetallicPropertyGroup metallic;

		public NormalPropertyGroup normal;

		public EmissivePropertyGroup emissive;

		public override Material MobileForward => MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Metallic/Forward"));

		public override Material MobileDeferredOpaque => MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Metallic/DeferredOpaque"));

		public override Material MobileDeferredTransparent => MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Metallic/DeferredTransparent"));

		public override Material StandardForward => MaterialFromShader(Shader.Find("Projection/Decal/Standard/Metallic/Forward"));

		public override Material StandardDeferredOpaque => MaterialFromShader(Shader.Find("Projection/Decal/Standard/Metallic/DeferredOpaque"));

		public override Material StandardDeferredTransparent => MaterialFromShader(Shader.Find("Projection/Decal/Standard/Metallic/DeferredTransparent"));

		public override Material PackedForward => MaterialFromShader(Shader.Find("Projection/Decal/Packed/Metallic/Forward"));

		public override Material PackedDeferredOpaque => MaterialFromShader(Shader.Find("Projection/Decal/Packed/Metallic/DeferredOpaque"));

		public override Material PackedDeferredTransparent => MaterialFromShader(Shader.Find("Projection/Decal/Packed/Metallic/DeferredTransparent"));

		public override RenderingPaths SupportedRendering => RenderingPaths.Both;

		public override int InstanceLimit => 500;

		protected override void Apply(Material Material)
		{
			base.Apply(Material);
			albedo.Apply(Material);
			metallic.Apply(Material);
			normal.Apply(Material);
			emissive.Apply(Material);
		}

		protected override void OnEnable()
		{
			if (albedo == null)
			{
				albedo = new AlbedoPropertyGroup(this);
			}
			if (metallic == null)
			{
				metallic = new MetallicPropertyGroup(this);
			}
			if (normal == null)
			{
				normal = new NormalPropertyGroup(this);
			}
			if (emissive == null)
			{
				emissive = new EmissivePropertyGroup(this);
			}
			base.OnEnable();
		}

		protected override void GenerateIDs()
		{
			base.GenerateIDs();
			albedo.GenerateIDs();
			metallic.GenerateIDs();
			normal.GenerateIDs();
			emissive.GenerateIDs();
		}

		public override void UpdateProperties()
		{
			if (properties == null || properties.Length != 2)
			{
				properties = new ProjectionProperty[2];
			}
			properties[0] = new ProjectionProperty("Albedo", albedo._Color, albedo.Color);
			properties[1] = new ProjectionProperty("Emission", emissive._EmissionColor, emissive.Color, emissive.Intensity);
		}
	}
}
