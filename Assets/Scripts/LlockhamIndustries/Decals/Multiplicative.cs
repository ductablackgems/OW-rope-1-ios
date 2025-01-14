using System;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
	[Serializable]
	public class Multiplicative : Forward
	{
		public override Material MobileForward => MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Multiplicative"));

		public override Material StandardForward => MaterialFromShader(Shader.Find("Projection/Decal/Standard/Multiplicative"));

		public override Material PackedForward => MaterialFromShader(Shader.Find("Projection/Decal/Packed/Multiplicative"));
	}
}
