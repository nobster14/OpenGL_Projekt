using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK_Tutorial.DataStructures
{
    public class ProgramOptions
    {
        #region Fields
        public ShadersOptions ShadersOptions { get; set; } = ShadersOptions.phong;
        public CameraOptions CameraOptions { get; set; } = CameraOptions.movingObjectCamera;
        public bool VibrationsEnabled = false;
        public float fogDensity = 0.0f;
        private bool increment = true;
        public int whichSpotLightToModify = 0;
        #endregion

        #region Public methods
        public void UpdateFogDensity()
        {
            if (increment)
                fogDensity += 0.0001f;
            else
                fogDensity -= 0.0001f;

            if (fogDensity > 0.4f)
            {
                fogDensity = 0.4f;
                increment = false;
            }
            if (fogDensity < 0.0f)
            {
                fogDensity = 0.0f;
                increment = true;
            }
        }
        #endregion
    }
}
