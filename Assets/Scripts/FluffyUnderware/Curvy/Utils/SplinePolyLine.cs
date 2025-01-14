using System;
using UnityEngine;

namespace FluffyUnderware.Curvy.Utils
{
	[Serializable]
	public class SplinePolyLine
	{
		public enum VertexCalculation
		{
			ByApproximation,
			ByAngle
		}

		public CurvySplineBase Spline;

		public VertexCalculation VertexMode;

		public float Angle;

		public float Distance;

		public bool IsClosed
		{
			get
			{
				if ((bool)Spline)
				{
					return Spline.IsClosed;
				}
				return false;
			}
		}

		public bool IsContinuous
		{
			get
			{
				if ((bool)Spline)
				{
					return Spline.IsContinuous;
				}
				return false;
			}
		}

		public SplinePolyLine(CurvySplineBase spline)
			: this(spline, VertexCalculation.ByApproximation, 0f, 0f)
		{
		}

		public SplinePolyLine(CurvySplineBase spline, float angle, float distance)
			: this(spline, VertexCalculation.ByAngle, angle, distance)
		{
		}

		private SplinePolyLine(CurvySplineBase spline, VertexCalculation vertexMode, float angle, float distance)
		{
			Spline = spline;
			VertexMode = vertexMode;
			Angle = angle;
			Distance = distance;
		}

		public Vector3[] getVertices()
		{
			VertexCalculation vertexMode = VertexMode;
			if (vertexMode == VertexCalculation.ByAngle)
			{
				return Spline.GetPolygonByAngle(Angle, Distance);
			}
			return Spline.GetApproximation();
		}
	}
}
