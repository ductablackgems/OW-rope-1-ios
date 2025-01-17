using UnityEngine;

namespace com.ootii.Utilities.Debug
{
	public class Disk
	{
		public Vector3[] Vertices;

		public int[] Triangles;

		public Disk()
		{
			Vertices = CreateVertices();
			Triangles = CreateTriangles();
			Vector3[] array = new Vector3[Triangles.Length];
			for (int i = 0; i < Triangles.Length; i++)
			{
				array[i] = Vertices[Triangles[i]];
				Triangles[i] = i;
			}
			Vertices = array;
		}

		private Vector3[] CreateVertices()
		{
			int num = 3;
			float[] array = new float[150]
			{
				0.482963f,
				-0.001076f,
				-0.129409f,
				0.433012f,
				-0.001076f,
				-0.25f,
				0.353553f,
				-0.001076f,
				-0.353553f,
				0.25f,
				-0.001076f,
				-0.433012f,
				0.12941f,
				-0.001076f,
				-0.482963f,
				0f,
				-0.001076f,
				-0.5f,
				-0.129409f,
				-0.001076f,
				-0.482963f,
				-0.25f,
				-0.001076f,
				-0.433013f,
				-0.353553f,
				-0.001076f,
				-0.353553f,
				-0.433013f,
				-0.001076f,
				-0.25f,
				-0.482963f,
				-0.001076f,
				-0.12941f,
				-0.5f,
				-0.001076f,
				-0f,
				-0.482963f,
				-0.001076f,
				0.129409f,
				-0.433013f,
				-0.001076f,
				0.25f,
				-0.353553f,
				-0.001076f,
				0.353553f,
				-0.25f,
				-0.001076f,
				0.433013f,
				-0.12941f,
				-0.001076f,
				0.482963f,
				-0f,
				-0.001076f,
				0.5f,
				0.129409f,
				-0.001076f,
				0.482963f,
				0.25f,
				-0.001076f,
				0.433013f,
				0.353553f,
				-0.001076f,
				0.353553f,
				0.433013f,
				-0.001076f,
				0.25f,
				0.482963f,
				-0.001076f,
				0.12941f,
				0.5f,
				-0.001076f,
				0f,
				0.482963f,
				0.001076f,
				-0.129409f,
				0.433012f,
				0.001076f,
				-0.25f,
				0.353553f,
				0.001076f,
				-0.353553f,
				0.25f,
				0.001076f,
				-0.433012f,
				0.12941f,
				0.001076f,
				-0.482963f,
				0f,
				0.001076f,
				-0.5f,
				-0.129409f,
				0.001076f,
				-0.482963f,
				-0.25f,
				0.001076f,
				-0.433013f,
				-0.353553f,
				0.001076f,
				-0.353553f,
				-0.433013f,
				0.001076f,
				-0.25f,
				-0.482963f,
				0.001076f,
				-0.12941f,
				-0.5f,
				0.001076f,
				-0f,
				-0.482963f,
				0.001076f,
				0.129409f,
				-0.433013f,
				0.001076f,
				0.25f,
				-0.353553f,
				0.001076f,
				0.353553f,
				-0.25f,
				0.001076f,
				0.433013f,
				-0.12941f,
				0.001076f,
				0.482963f,
				-0f,
				0.001076f,
				0.5f,
				0.129409f,
				0.001076f,
				0.482963f,
				0.25f,
				0.001076f,
				0.433013f,
				0.353553f,
				0.001076f,
				0.353553f,
				0.433013f,
				0.001076f,
				0.25f,
				0.482963f,
				0.001076f,
				0.12941f,
				0.5f,
				0.001076f,
				0f,
				0f,
				-0.001076f,
				0f,
				0f,
				0.001076f,
				0f
			};
			Vector3[] array2 = new Vector3[array.Length / num];
			for (int i = 0; i < array.Length; i += num)
			{
				array2[i / num] = new Vector3(array[i], array[i + 1], array[i + 2]);
			}
			return array2;
		}

		private int[] CreateTriangles()
		{
			int[] array = new int[576]
			{
				25,
				0,
				24,
				1,
				0,
				2,
				0,
				3,
				1,
				4,
				25,
				5,
				26,
				6,
				25,
				7,
				1,
				8,
				1,
				9,
				2,
				10,
				26,
				11,
				2,
				12,
				3,
				13,
				27,
				14,
				2,
				15,
				27,
				16,
				26,
				17,
				28,
				18,
				3,
				19,
				4,
				20,
				3,
				21,
				28,
				22,
				27,
				23,
				4,
				24,
				5,
				25,
				29,
				26,
				4,
				27,
				29,
				28,
				28,
				29,
				30,
				30,
				5,
				31,
				6,
				32,
				5,
				33,
				30,
				34,
				29,
				35,
				31,
				36,
				6,
				37,
				7,
				38,
				6,
				39,
				31,
				40,
				30,
				41,
				7,
				42,
				8,
				43,
				32,
				44,
				7,
				45,
				32,
				46,
				31,
				47,
				33,
				48,
				8,
				49,
				9,
				50,
				8,
				51,
				33,
				52,
				32,
				53,
				34,
				54,
				9,
				55,
				10,
				56,
				9,
				57,
				34,
				58,
				33,
				59,
				35,
				60,
				10,
				61,
				11,
				62,
				10,
				63,
				35,
				64,
				34,
				65,
				36,
				66,
				11,
				67,
				12,
				68,
				11,
				69,
				36,
				70,
				35,
				71,
				37,
				72,
				12,
				73,
				13,
				74,
				12,
				75,
				37,
				76,
				36,
				77,
				38,
				78,
				13,
				79,
				14,
				80,
				13,
				81,
				38,
				82,
				37,
				83,
				39,
				84,
				38,
				85,
				14,
				86,
				39,
				87,
				14,
				88,
				15,
				89,
				40,
				90,
				39,
				91,
				15,
				92,
				40,
				93,
				15,
				94,
				16,
				95,
				41,
				96,
				40,
				97,
				16,
				98,
				41,
				99,
				16,
				100,
				17,
				101,
				42,
				102,
				41,
				103,
				17,
				104,
				42,
				105,
				17,
				106,
				18,
				107,
				43,
				108,
				42,
				109,
				18,
				110,
				43,
				111,
				18,
				112,
				19,
				113,
				44,
				114,
				43,
				115,
				19,
				116,
				44,
				117,
				19,
				118,
				20,
				119,
				45,
				120,
				44,
				121,
				20,
				122,
				20,
				123,
				21,
				124,
				45,
				125,
				46,
				126,
				45,
				127,
				21,
				128,
				21,
				129,
				22,
				130,
				46,
				131,
				47,
				132,
				46,
				133,
				22,
				134,
				22,
				135,
				23,
				136,
				47,
				137,
				24,
				138,
				47,
				139,
				23,
				140,
				23,
				141,
				0,
				142,
				24,
				143,
				1,
				144,
				0,
				145,
				48,
				146,
				2,
				147,
				1,
				148,
				48,
				149,
				3,
				150,
				2,
				151,
				48,
				152,
				4,
				153,
				3,
				154,
				48,
				155,
				5,
				156,
				4,
				157,
				48,
				158,
				6,
				159,
				5,
				160,
				48,
				161,
				7,
				162,
				6,
				163,
				48,
				164,
				8,
				165,
				7,
				166,
				48,
				167,
				9,
				168,
				8,
				169,
				48,
				170,
				10,
				171,
				9,
				172,
				48,
				173,
				11,
				174,
				10,
				175,
				48,
				176,
				12,
				177,
				11,
				178,
				48,
				179,
				13,
				180,
				12,
				181,
				48,
				182,
				14,
				183,
				13,
				184,
				48,
				185,
				15,
				186,
				14,
				187,
				48,
				188,
				16,
				189,
				15,
				190,
				48,
				191,
				17,
				192,
				16,
				193,
				48,
				194,
				18,
				195,
				17,
				196,
				48,
				197,
				19,
				198,
				18,
				199,
				48,
				200,
				20,
				201,
				19,
				202,
				48,
				203,
				21,
				204,
				20,
				205,
				48,
				206,
				22,
				207,
				21,
				208,
				48,
				209,
				23,
				210,
				22,
				211,
				48,
				212,
				0,
				213,
				23,
				214,
				48,
				215,
				24,
				216,
				25,
				217,
				49,
				218,
				25,
				219,
				26,
				220,
				49,
				221,
				26,
				222,
				27,
				223,
				49,
				224,
				27,
				225,
				28,
				226,
				49,
				227,
				28,
				228,
				29,
				229,
				49,
				230,
				29,
				231,
				30,
				232,
				49,
				233,
				30,
				234,
				31,
				235,
				49,
				236,
				31,
				237,
				32,
				238,
				49,
				239,
				32,
				240,
				33,
				241,
				49,
				242,
				33,
				243,
				34,
				244,
				49,
				245,
				34,
				246,
				35,
				247,
				49,
				248,
				35,
				249,
				36,
				250,
				49,
				251,
				36,
				252,
				37,
				253,
				49,
				254,
				37,
				255,
				38,
				256,
				49,
				257,
				38,
				258,
				39,
				259,
				49,
				260,
				39,
				261,
				40,
				262,
				49,
				263,
				40,
				264,
				41,
				265,
				49,
				266,
				41,
				267,
				42,
				268,
				49,
				269,
				42,
				270,
				43,
				271,
				49,
				272,
				43,
				273,
				44,
				274,
				49,
				275,
				44,
				276,
				45,
				277,
				49,
				278,
				45,
				279,
				46,
				280,
				49,
				281,
				46,
				282,
				47,
				283,
				49,
				284,
				47,
				285,
				24,
				286,
				49,
				287
			};
			int[] array2 = new int[array.Length / 2];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[i * 2];
			}
			return array2;
		}
	}
}
