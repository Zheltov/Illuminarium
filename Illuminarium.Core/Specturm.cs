using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public class Spectrum
    {
        private float _r;
        private float _g;
        private float _b;

        public float R { get { return this._r; } }
        public float G { get { return this._g; } }
        public float B { get { return this._b; } }

        public float ValueMax
        {
            get
            {
                if ( this._r >= this._g && this._r >= this._b )
                    return this._r;
                else if ( this._g >= this._r && this._g >= this._b )
                    return this._g;
                else
                    return this._b;
            }
        }

        public Spectrum()
        {
        }

        public Spectrum(Spectrum spectrum) : this ( spectrum._r, spectrum._g, spectrum._b )
        {
        }

        public Spectrum( float rgb ) : this ( rgb, rgb, rgb )
        {
        }

        public Spectrum( float r, float g, float b )
        {
            this._r = r;
            this._g = g;
            this._b = b;
        }

        public float Sum()
        {
            return this._r + this._g + this._b;
        }

        public Spectrum Add( Spectrum spectrum )
        {
            this._r += spectrum._r;
            this._g += spectrum._g;
            this._b += spectrum._b;

            return this;
        }

        public Spectrum Multiplication( float f )
        {
            this._r *= f;
            this._g *= f;
            this._b *= f;

            return this;
        }

        public Spectrum Multiplication( Spectrum spectrum )
        {
            this._r *= spectrum._r;
            this._g *= spectrum._g;
            this._b *= spectrum._b;

            return this;
        }

        public Spectrum Division( float f )
        {
            this._r /= f;
            this._g /= f;
            this._b /= f;

            return this;
        }

        public Color ToColor( float cx )
        {
            return Color.FromArgb(
                (int)( this._r * cx * 255f ),
                (int)( this._g * cx * 255f ),
                (int)( this._b * cx * 255f ) );
        }

        public static Spectrum operator +( Spectrum s1, Spectrum s2 )
        {
            return new Spectrum( s1._r + s2._r, s1._g + s2._g, s1._b + s2._b );
        }

        public static Spectrum operator +( Spectrum s1, float f )
        {
            return new Spectrum( s1._r + f, s1._g + f, s1._b + f );
        }

        public static Spectrum operator -( Spectrum s1, Spectrum s2 )
        {
            return new Spectrum( s1._r - s2._r, s1._g - s2._g, s1._b - s2._b );
        }

        public static Spectrum operator /( Spectrum s1, float f )
        {
            return new Spectrum( s1._r / f, s1._g / f, s1._b / f );
        }

        public static Spectrum operator *( Spectrum s1, float f )
        {
            return new Spectrum( s1._r * f, s1._g * f, s1._b * f );
        }

        public static Spectrum operator *( Spectrum s1, Spectrum s2 )
        {
            return new Spectrum( s1._r * s2._r, s1._g * s2._g, s1._b * s2._b );
        }

        public override string ToString()
        {
            return string.Format( "({0}, {1}, {2})", this._r, this._g, this._b );
        }
    }
}
