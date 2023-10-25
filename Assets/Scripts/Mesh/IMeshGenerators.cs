using UnityEngine;

namespace StrengthInNumber.ProceduralMeshes
{
	public interface IMeshGenerator
	{
		Bounds Bounds { get; }
		int VertexCount { get; }
		int IndexCount { get; }
		int JobLength { get; }

		void Execute<S>(int i, S streams) where S : struct, IMeshStreams;
	}
}