using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK_Tutorial.DataStructures
{
    public class SpotLight
    {
        #region Fields
        public Vector3 position;
        public Vector3 direction = new Vector3(0, 0, -1f);
        #endregion

        #region Constructors
        public SpotLight(Vector3 position)
        {
            this.position = position;
        }
        #endregion

        #region Public methods
        public void MoveLight(Vector3 move)
        {
            position += move;
        }
        public void ChangeDirection(Vector3 dir)
        {
            direction += dir;
        }
        #endregion
    }
}
