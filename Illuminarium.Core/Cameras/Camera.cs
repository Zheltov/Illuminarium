using System;

namespace Illuminarium.Core
{
    /// <summary>
    /// ����� ������
    /// </summary>
    public class Camera : ICamera
    {
        float theta;
        float phi;
        float r;
        Point3D pos = new Point3D( 0, 0, 0 );
        Point3D target = new Point3D( 1, 0, 0 );
        Point3D upVector = new Point3D( 0, 0, 1 );

        public float Theta
        {
            get { return theta; }
            set { theta = value; ReCalcPos(); }
        }
        public float Phi
        {
            get { return phi; }
            set { phi = value; ReCalcPos(); }
        }
        public float R
        {
            get { return r; }
            set { r = value; ReCalcPos(); }
        }
        public Point3D Position
        {
            get { return pos; }
            set { pos = value; ReCalcAngle(); }
        }
        public Point3D Target
        {
            get { return target; }
            set
            {
                target = value;//.Normalize(); 
                ReCalcAngle();
            }
        }
        public Point3D UpVector
        {
            get { return upVector; }
            set { upVector = value; ReCalcAngle(); }
        }

        public float Fov { get; set; }

        public Camera()
        {
            this.Fov = (float)Math.PI / 3;
            this.ReCalcAngle();
        }

        /// <summary>
        /// ��������������� ������
        /// </summary>
        /// <param name="dTheta">�������� �� ���� ����</param>
        /// <param name="dPhi">�������� �� ���� ��</param>
        public void Targeting( float dTheta, float dPhi )
        {
            // ������� ���� ����� ����� � ����� �����������, � ����� � ������
            // � ������ ���� �� ������
            float x = pos.X - target.X;
            float y = pos.Y - target.Y;
            float z = pos.Z - target.Z;

            float mtheta = (float)Math.Atan2( y, z ) + dTheta;
            float mphi = (float)Math.Atan2( z, Math.Sqrt( x * x + y * y ) ) + dPhi;

            // ������������ ������� ����� �����������
            target = new Point3D(
                (float)( -r * Math.Cos( mphi ) * Math.Cos( mtheta ) + pos.X ),
                (float)( -r * Math.Cos( mphi ) * Math.Sin( mtheta ) + pos.Y ),
                (float)( -r * Math.Sin( mphi ) + pos.Z ) );

            // ������������ ����, ��� ��� ������� ����� �����������, 
            // ������������� � ��� ����������, ��� ��� ���� �����������
            ReCalcAngle();
        }
        /// <summary>
        /// ����������� ������ ��� �� ������� ����� ������ 
        /// �� ������ ������ �� ����� �����������
        /// </summary>
        /// <param name="dR">�������� �����������</param>
        public void Dolly( float dR )
        {
            Point3D v = Point3D.Normalize( target - pos );
            v = v * dR;
            target += v;
            pos += v;
        }

        public Ray GetTracingRay( int pixel_x_coord, int pixel_y_coord, int clientWidth, int clientHeight )
        {
            double width = 2 * ( Math.Sin( this.Fov / 2 ) );
            double height = ( (double)clientHeight / (double)clientWidth ) * width;

            var direction = Point3D.Normalize( this.Target - this.Position );

            Point3D right = direction * this.UpVector;

            pixel_y_coord = clientHeight - pixel_y_coord;

            float x = ( pixel_x_coord - clientWidth / 2f );
            float y = ( pixel_y_coord - clientHeight / 2f );

            Point3D target = right * (float)( width * x / ( clientWidth - 1 ) )
                        + this.UpVector * (float)( height * y / ( clientHeight - 1 ) );

            target += direction;

            return new Ray( this.Position, this.Position + target );
        }

        /// <summary>
        /// �������� ��������� Pos. ���� ������ ����, 
        /// �� �������������� ������������� ����������
        /// </summary>
        private void ReCalcPos()
        {
            this.pos = new Point3D(
                (float)( -r * Math.Cos( phi ) * Math.Cos( theta ) + target.X ),
                (float)( -r * Math.Cos( phi ) * Math.Sin( theta ) + target.Y ),
                (float)( -r * Math.Sin( phi ) + target.Z ) );
        }
        /// <summary>
        /// �������� ����� � ������ ������� ��� ���������
        /// ��������� ����� ����������� ��� �������
        /// </summary>
        private void ReCalcAngle()
        {
            float x = target.X - pos.X;
            float y = target.Y - pos.Y;
            float z = target.Z - pos.Z;

            theta = (float)Math.Atan2( y, z );
            phi = (float)Math.Atan2( z, Math.Sqrt( x * x + y * y ) );
            r = (float)Math.Sqrt( x * x + y * y + z * z );

            //if (theta < 0)
            //    theta = (float)(2 * Math.PI + theta);
            /*
            if (Math.Abs(Math.Abs(phi) - Math.PI / 2) < Eps)
            {
                if (phi < 0)
                    phi = (float)-Math.PI / 2 + Eps;
                if (phi >0)
                    phi = (float)Math.PI / 2 - Eps;


            }
             */
        }
    }
}