using ObjLoader.Loader.Data;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTK_Tutorial.DataStructures
{
    public class MovingObject : MyObject
    {
        #region Fields
        private Random random = new Random();
        #endregion

        #region Constructors
        public MovingObject(string name, Material material) : base(name, material)
        {
            model = OpenTK.Mathematics.Matrix4.Identity;
            Matrix4.CreateTranslation(0, 2, 0, out model);


            verticiesAndNormals = new List<OpenTK.Mathematics.Vector3>
            {
                 new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f,  0.0f, -1.0f), // Front face
                 new Vector3(0.5f, -0.5f, -0.5f),  new Vector3(0.0f,  0.0f, -1.0f),
                 new Vector3(0.5f,  0.5f, -0.5f),  new Vector3(0.0f,  0.0f, -1.0f),
                 new Vector3(0.5f,  0.5f, -0.5f),  new Vector3(0.0f,  0.0f, -1.0f),
                 new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0.0f,  0.0f, -1.0f),
                 new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f,  0.0f, -1.0f),

                 new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(0.0f,  0.0f,  1.0f), // Back face
                 new Vector3(0.5f, -0.5f,  0.5f),  new Vector3(0.0f,  0.0f,  1.0f),
                 new Vector3(0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  0.0f,  1.0f),
                 new Vector3(0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  0.0f,  1.0f),
                 new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(0.0f,  0.0f,  1.0f),
                 new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(0.0f,  0.0f,  1.0f),

                 new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(-1.0f,  0.0f,  0.0f), // Left face
                 new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(-1.0f,  0.0f,  0.0f),
                 new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1.0f,  0.0f,  0.0f),
                 new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1.0f,  0.0f,  0.0f),
                 new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(-1.0f,  0.0f,  0.0f),
                 new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(-1.0f,  0.0f,  0.0f),

                 new Vector3(0.5f,  0.5f,  0.5f),  new Vector3(1.0f,  0.0f,  0.0f), // Right face
                 new Vector3(0.5f,  0.5f, -0.5f),  new Vector3(1.0f,  0.0f,  0.0f),
                 new Vector3(0.5f, -0.5f, -0.5f),  new Vector3(1.0f,  0.0f,  0.0f),
                 new Vector3(0.5f, -0.5f, -0.5f),  new Vector3(1.0f,  0.0f,  0.0f),
                 new Vector3(0.5f, -0.5f,  0.5f),  new Vector3(1.0f,  0.0f,  0.0f),
                 new Vector3(0.5f,  0.5f,  0.5f),  new Vector3(1.0f,  0.0f,  0.0f),

                 new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f,  0.0f), // Bottom face
                 new Vector3(0.5f, -0.5f, -0.5f),  new Vector3(0.0f, -1.0f,  0.0f),
                 new Vector3(0.5f, -0.5f,  0.5f),  new Vector3(0.0f, -1.0f,  0.0f),
                 new Vector3(0.5f, -0.5f,  0.5f),  new Vector3(0.0f, -1.0f,  0.0f),
                 new Vector3(-0.5f, -0.5f,  0.5f), new Vector3(0.0f, -1.0f,  0.0f),
                 new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.0f, -1.0f,  0.0f),

                 new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0.0f,  1.0f,  0.0f), // Top face
                 new Vector3(0.5f,  0.5f, -0.5f),  new Vector3(0.0f,  1.0f,  0.0f),
                 new Vector3(0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  1.0f,  0.0f),
                 new Vector3(0.5f,  0.5f,  0.5f),  new Vector3(0.0f,  1.0f,  0.0f),
                 new Vector3(-0.5f,  0.5f,  0.5f), new Vector3(0.0f,  1.0f,  0.0f),
                 new Vector3(-0.5f,  0.5f, -0.5f), new Vector3(0.0f,  1.0f,  0.0f)
            };
        }
        #endregion

        public void MoveObject(Vector3 move)
        {
            model.Row3.X += move.X;
            model.Row3.Y += move.Y;
            model.Row3.Z += move.Z;
        }
        public void RotateObject(float angle)
        {
            Matrix4.CreateRotationX(angle, out var matrix);
            model.Row1 = matrix.Row1;
            model.Row2 = matrix.Row2;
        }
        public void VibrateObject()
        {
            var vibrX = random.Next(-100, 100);
            var vibrY = random.Next(-100, 100);
            var vibrZ = random.Next(-100, 100);

            model.Row3.X += vibrX / (float)50000;
            model.Row3.Y += vibrY / (float)50000;
            model.Row3.Z += vibrZ / (float)50000;
        }
    }
}
