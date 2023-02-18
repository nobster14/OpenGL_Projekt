using ObjLoader.Loader.Data;
using ObjLoader.Loader.Data.Elements;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace OpenTK_Tutorial.DataStructures
{
    public class MyObject
    {
        #region Fields
        public Material material;
        private string name;
        public List<Vector3> verticiesAndNormals = new List<Vector3>();
        public Matrix4 model = Matrix4.Identity;
        #endregion

        #region Constructors
        public MyObject(string name, Material material)
        {
            this.name = name;
            this.material = material;
        }
        #endregion
        #region Public methods
        public float[] ConvertVerticiesToFlatArray()
        { 
            return verticiesAndNormals
                .Select(it => new List<float> { it.X, it.Y, it.Z })
                .SelectMany(it => it)
                .ToArray();

        }
        #endregion
    }
}
