using System.Collections.Generic;
using System.Linq;

internal class VertexVerticesIndex
{
	internal class Vertex
	{
		public List<int> Vertices = new List<int>();
	}

	private List<Vertex> Vertices = new List<Vertex>();

	public int Capacity
	{
		get
		{
			return Vertices.Count;
		}
		set
		{
			Vertices.Clear();
			for (int i = 0; i < value; i++)
			{
				Vertices.Add(new Vertex());
			}
		}
	}

	public void Add(int vertex, int connected)
	{
		Vertices[vertex].Vertices.Add(connected);
	}

	public void ConsolidateIndex()
	{
		Vertices.ForEach(delegate(Vertex t)
		{
			t.Vertices = t.Vertices.Distinct().ToList();
		});
	}

	public void ListConnectedVertices(int vertex, List<int> vertices)
	{
		vertices.AddRange(Vertices[vertex].Vertices);
	}

	public List<int> ListConnectedVertices(int vertex)
	{
		List<int> list = new List<int>();
		ListConnectedVertices(vertex, list);
		return list;
	}
}
