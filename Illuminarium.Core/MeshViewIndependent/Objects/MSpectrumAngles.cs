using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.MeshViewIndependent
{
    public class MSpectrumAngles
    {
        public IList<float> Theta { get; private set; }
        public IList<float> Mu { get; private set; }
        public IList<float> Phi { get; private set; }
        public IList<double> GaussWeight { get; private set; }

        public MSpectrumAngles( IList<float> theta, IList<float> phi, IList<double> gaussWeight )
        {
            Theta = theta;
            Phi = phi;
            GaussWeight = gaussWeight;
            Mu = theta.Select( x => (float)Math.Cos( x ) ).ToList();
        }
    }
}
