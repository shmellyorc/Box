// namespace Box.Pathfind;

// internal sealed class Vertex
// {
//     public int Id { get; set; }
//     public Vect2 Position { get; set; }

//     public List<Vertex> Edges;
//     public int Count => Edges.Count;

//     internal Vertex(int id, Vect2 position)
//     {
//         Edges = new List<Vertex>();
//         Id = id;
//         Position = position;
//     }

//     public bool IsConnected(int id)
//     {
//         foreach (Vertex vertex in Edges)
//         {
//             if (vertex.Id.Equals(id))
//                 return true;
//         }

//         return false;
//     }
// }
