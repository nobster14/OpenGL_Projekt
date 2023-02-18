using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK_Tutorial.DataStructures
{
    public class DirectLight
    {
        #region Fields
        public Vector3 direction = new Vector3(-3.0f, -3.0f, -0.0f);
        public Vector3 ambient = new Vector3(0.05f, 0.05f, 0.05f);
        public Vector3 diffuse =  new Vector3(0.4f, 0.4f, 0.4f);
        public Vector3 specular = new Vector3(0.5f, 0.5f, 0.5f);
        private int counter = 0;
        #endregion

        #region Public methods
        public void ChangeDirection()
        {
            if (counter == 3600)
                counter = 0;

            direction.Y = (float)(-1 * Math.Sin(Math.PI * counter / 1800));
            direction.Z = (float)(-1 * Math.Cos(Math.PI * counter / 1800));

            counter++;
        }
        #endregion
    }
}
