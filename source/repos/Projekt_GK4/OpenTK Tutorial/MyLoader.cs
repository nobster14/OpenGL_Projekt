
using ObjLoader.Loader.Data;
using ObjLoader.Loader.Data.VertexData;
using ObjLoader.Loader.Loaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK_Tutorial.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenTK_Tutorial
{
    public static class MyLoader
    {
        public static List<MyObject> Load(string path)
        {
            var ret = new List<MyObject>();



            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var fileStream = new FileStream(path, FileMode.Open);
            var result = objLoader.Load(fileStream);
            fileStream.Close();


            foreach (var shape in result.Groups)
            {
                var newObj = new MyObject(shape.Name, shape.Material);
                if (newObj.material == null)
                    newObj.material = result.Materials.First(it => it.Name == "default");

                foreach (var triangle in shape.Faces)
                {
                    newObj.verticiesAndNormals.Add(result.Vertices[triangle[0].VertexIndex - 1].Map());
                    newObj.verticiesAndNormals.Add(result.Normals[triangle[0].NormalIndex - 1].Map());
                    newObj.verticiesAndNormals.Add(result.Vertices[triangle[1].VertexIndex - 1].Map());
                    newObj.verticiesAndNormals.Add(result.Normals[triangle[1].NormalIndex - 1].Map());
                    newObj.verticiesAndNormals.Add(result.Vertices[triangle[2].VertexIndex - 1].Map());
                    newObj.verticiesAndNormals.Add(result.Normals[triangle[2].NormalIndex - 1].Map());
                }

                ret.Add(newObj);
            }

            ret.Add(new MovingObject("moving cube", result.Materials.First(it => it.Name == "default")));

            return ret;
        }

        private static Vector3 Map(this Vertex vert)
        {
            return new Vector3(vert.X, vert.Y, vert.Z);
        }
        private static Vector3 Map(this Normal normal)
        {
            return new Vector3(normal.X, normal.Y, normal.Z);
        }

        public static Vector3 Map(this Vec3 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }
            
    }
}
