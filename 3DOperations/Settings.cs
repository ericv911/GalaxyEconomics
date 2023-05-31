using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DOperations
{


    public class  _3DSettings
    {
        /// <summary>
        /// Class used to store the variables concerning the  3D settings Scaling, Rotating and Translating
        /// Additionally this will be used to store the cameraposition and the camerangles
        /// </summary>
        public _3DSettings()
        {
            RotationAngles = new RotationAngles();
            Translations = new Translation();
            ScaleFactor = 1;
        }
        public Translation Translations
        {
            get; set;
        }
        public double ScaleFactor
        {
            get; set;
        }
        public RotationAngles RotationAngles
        {
            get; set;

        }
    }
}
