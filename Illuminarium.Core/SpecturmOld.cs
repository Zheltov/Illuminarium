using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public class SpectrumOld
    {
        public const int Size = 3;

        public static SpectrumOld Zero = new SpectrumOld( 0 );

        public float[] Values { get; set; }

        public float ValueMax
        {
            get
            {
                if ( this.Values[0] >= this.Values[1] && this.Values[0] >= this.Values[2] )
                    return this.Values[0];
                else if ( this.Values[1] >= this.Values[0] && this.Values[1] >= this.Values[2] )
                    return this.Values[1];
                else
                    return this.Values[2];
            }
        }

        public SpectrumOld()
        {
            this.Values = new float[SpectrumOld.Size];
        }

        public SpectrumOld( float[] values )
        {
            this.Values = values;
        }

        public SpectrumOld( float illuminance )
        {
            this.Values = new float[SpectrumOld.Size];
            for ( int i = 0; i < SpectrumOld.Size; i++ )
                this.Values[i] = illuminance;
        }

        public float Sum()
        {
            float result = 0;
            for ( int i = 0; i < SpectrumOld.Size; i++ )
                result += this.Values[i];

            return result;
        }

        public float Avg()
        {
            return this.Sum() / SpectrumOld.Size;
        }

        public Color ToColor( float cx )
        {
            return Color.FromArgb(
                (int)( this.Values[0] * cx * 255f ),
                (int)( this.Values[1] * cx * 255f ),
                (int)( this.Values[2] * cx * 255f )
                );
        }

        public static SpectrumOld operator +( SpectrumOld s1, SpectrumOld s2 )
        {
            var result = new SpectrumOld();
            for ( int i = 0; i < SpectrumOld.Size; i++ )
                result.Values[i] = s1.Values[i] + s2.Values[i];

            return result;
        }

        public static SpectrumOld operator +( SpectrumOld s1, float f )
        {
            var result = new SpectrumOld();
            for ( int i = 0; i < SpectrumOld.Size; i++ )
                result.Values[i] = s1.Values[i] + f;

            return result;
        }

        public static SpectrumOld operator /( SpectrumOld s1, float f )
        {
            var result = new SpectrumOld();
            for ( int i = 0; i < SpectrumOld.Size; i++ )
                result.Values[i] = s1.Values[i] / f;

            return result;
        }

        public static SpectrumOld operator *( SpectrumOld s1, float f )
        {
            var result = new SpectrumOld();
            for ( int i = 0; i < SpectrumOld.Size; i++ )
                result.Values[i] = s1.Values[i] * f;

            return result;
        }

        public static SpectrumOld operator *( SpectrumOld s1, SpectrumOld s2 )
        {
            var result = new SpectrumOld();
            for ( int i = 0; i < SpectrumOld.Size; i++ )
                result.Values[i] = s1.Values[i] * s2.Values[i];

            return result;
        }

        public override string ToString()
        {
            var result = string.Empty;

            for ( int i = 0; i < SpectrumOld.Size; i++ )
                result += string.Format( "[{0}] = {1}", i, this.Values[i] );

            return result.Trim();
        }
    }
}
