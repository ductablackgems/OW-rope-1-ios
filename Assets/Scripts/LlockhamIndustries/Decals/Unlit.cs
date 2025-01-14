using System;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
	[Serializable]
	public class Unlit : Forward
	{
		public override Material MobileForward => MaterialFromShader(Shader.Find("Projection/Decal/Mobile/Unlit"));

		public override Material StandardForward => MaterialFromShader(Shader.Find("Projection/Decal/Standard/Unlit"));

		public override Material PackedForward => MaterialFromShader(Shader.Find("Projection/Decal/Packed/Unlit"));
	}
}
