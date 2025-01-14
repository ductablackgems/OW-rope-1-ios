using System;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
	[Serializable]
	public class Additive : Forward
	{
		public override Material MobileForward => MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Additive"));

		public override Material StandardForward => MaterialFromShader(Shader.Find("Projection/Decal/Standard/Additive"));

		public override Material PackedForward => MaterialFromShader(Shader.Find("Projection/Decal/Packed/Additive"));
	}
}
