using System;
using UnityEngine;

namespace AnimationSystem
{
    [CreateAssetMenu]
    public class Mesh2D : ScriptableObject
    {
        [Serializable]
        public class Vertex
        {
            public Vector2 Point;
            public Vector2 Normal;
            public float U;
        }

        public Vertex[] Vertices;
        public int[] LineIndices;

        public int VertexCount => Vertices.Length;
        public int LineCount => LineIndices.Length;

        public float CalculateUspan()
        {
            var distance = 0f;
            for (var i = 0; i < LineCount; i += 2)
            {
                var uA = Vertices[LineIndices[i]].Point;
                var uB = Vertices[LineIndices[i + 1]].Point;

                distance += Vector2.Distance(uA, uB);
            }

            return distance;
        }

        public void CalculateUcoordinates()
        {
            for (int i = 0; i < VertexCount; i++)
            {
                Vertices[i].U = i / (VertexCount - 1f);
            }
        }
    }
}