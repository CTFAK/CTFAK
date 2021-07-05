
package Services;

public class CoreMath
{
    public static final double M_PI = 3.1415926535897932;

    public static native float Q_rsqrt(float x);

    public double degreesToRadians(double degrees)
    {
        return degrees * M_PI/180.0f;
    }

    public double radiansToDegrees(double radians)
    {
        double ret = radians * 180.0f/M_PI;
        if (ret < 0)
            return 360+ret;
        else
            return ret;
    }

    public static class Vec2f
    {
        public double x, y;

        public Vec2f()
        {
        }

        public Vec2f(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2f(Vec2f rhs)
        {
            this.x = rhs.x;
            this.y = rhs.y;
        }

        public static double distanceBetweenPositions(Vec2f a, Vec2f b)
        {
            double dX = b.x - a.x;
            double dY = b.y - a.y;
            return Math.sqrt(dX*dX+dY*dY);
        }

        static Vec2f interpolate(Vec2f a, Vec2f b, double step)
        {
            return new Vec2f(a.x + (b.x-a.x)*step,
                             a.y + (b.y-a.y)*step);
        }

        public double distanceToPosition(Vec2f point)
        {
            double dX = point.x - x;
            double dY = point.y - y;
            return Math.sqrt(dX*dX+dY*dY);
        }

        public void normalize()
        {
            double len = Math.sqrt(x*x + y*y);
            if(len > 0)
            {
                double inv = 1.0f/len;
                x *= inv;
                y *= inv;
            }
            else
            {
                x = 0;
                y = 0;
            }
        }

        public Vec2f normalized()
        {
            double len = Math.sqrt(x*x + y*y);
            if(len > 0)
            {
                Vec2f ret = new Vec2f(x,y);
                double inv = 1.0f/len;
                ret.x *= inv;
                ret.y *= inv;
            }
            return new Vec2f(Vec2fZero);
        }

        public void normaliseFast()
        {
            double len = Q_rsqrt((float) (x*x + y*y));
            if(len > 0)
            {
                double inv = 1.0f/len;
                x *= inv;
                y *= inv;
            }
            else
            {
                x = 0;
                y = 0;
            }
        }

        public Vec2f normalizedFast()
        {
            Vec2f ret = new Vec2f(this);
            ret.normaliseFast();
            return ret;
        }
    }

    public static class Vec2i
    {
        public int x, y;

        public Vec2i()
        {
        }

        public Vec2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2i(Vec2f fVec)
        {
            this.x = (int) fVec.x;
            this.y = (int) fVec.y;
        }

    }

    public static final Vec2f Vec2fZero = new Vec2f(0,0);
    public static final Vec2f Vec2fOne = new Vec2f(1,1);
    public static final Vec2i Vec2iZero = new Vec2i(0,0);
    public static final Vec2i Vec2iOne = new Vec2i(1,1);

    public static class Mat3f
    {
        //double a,b,c,    d,e,f,   g,h,i;
        double a,d,g,    b,e,h,	c,f,i;		//Column major order   (rows are "abc, def, ghi")

        public static Mat3f identity()
        {
            Mat3f ret = new Mat3f();

            ret.a = 1.0f;  ret.d = 0.0f;  ret.g = 0.0f;
            ret.b = 0.0f;  ret.e = 1.0f;  ret.h = 0.0f;
            ret.c = 0.0f;  ret.f = 0.0f;  ret.i = 1.0f;

            return ret;
        }

        public static Mat3f zero()
        {
            Mat3f ret = new Mat3f();

            ret.a = 0.0f;  ret.d = 0.0f;  ret.g = 0.0f;
            ret.b = 0.0f;  ret.e = 0.0f;  ret.h = 0.0f;
            ret.c = 0.0f;  ret.f = 0.0f;  ret.i = 0.0f;

            return ret;
        }

        public static Mat3f identityFlippedY()
        {
            Mat3f ret = new Mat3f();

            ret.a = 1.0f;  ret.d = 0.0f;  ret.g = 0.0f;
            ret.b = 0.0f;  ret.e = -1.0f; ret.h = 0.0f;
            ret.c = 0.0f;  ret.f = 1.0f;  ret.i = 1.0f;

            return ret;
        }

        public static Mat3f translationMatrix(double x, double y)
        {
            Mat3f ret = new Mat3f();

            ret.a = 1.0f;  ret.d = 0.0f;  ret.g = 0.0f;
            ret.b = 0.0f;  ret.e = 1.0f;  ret.h = 0.0f;
            ret.c = x;     ret.f = y;     ret.i = 1.0f;

            return ret;
        }

        public static Mat3f translationMatrix(double x, double y, double z)
        {
            Mat3f ret = new Mat3f();

            ret.a = 1.0f;  ret.d = 0.0f;  ret.g = 0.0f;
            ret.b = 0.0f;  ret.e = 1.0f;  ret.h = 0.0f;
            ret.c = x;     ret.f = y;     ret.i = z;

            return ret;
        }

        public static Mat3f scaleMatrix(double x, double y, double z)
        {
            Mat3f ret = new Mat3f();

            ret.a = x;     ret.d = 0.0f;  ret.g = 0.0f;
            ret.b = 0.0f;  ret.e = y;     ret.h = 0.0f;
            ret.c = 0.0f;  ret.f = 0.0f;  ret.i = z;

            return ret;
        }

        public static Mat3f scaleMatrix(double x, double y)
        {
            Mat3f ret = new Mat3f();

            ret.a = x;     ret.d = 0.0f;  ret.g = 0.0f;
            ret.b = 0.0f;  ret.e = y;     ret.h = 0.0f;
            ret.c = 0.0f;  ret.f = 0.0f;  ret.i = 1.0f;

            return ret;
        }

        public static Mat3f multiply(Mat3f a, Mat3f b)
        {
            Mat3f ret = new Mat3f();
            ret.a = b.a * a.a + b.d * a.b + b.g * a.c;
            ret.b = b.b * a.a + b.e * a.b + b.h * a.c;
            ret.c = b.c * a.a + b.f * a.b + b.i * a.c;
            ret.d = b.a * a.d + b.d * a.e + b.g * a.f;
            ret.e = b.b * a.d + b.e * a.e + b.h * a.f;
            ret.f = b.c * a.d + b.f * a.e + b.i * a.f;
            ret.g = b.a * a.g + b.d * a.h + b.g * a.i;
            ret.h = b.b * a.g + b.e * a.h + b.h * a.i;
            ret.i = b.c * a.g + b.f * a.h + b.i * a.i;
            return ret;
        }

        public static Mat3f multiply(Mat3f a, Mat3f b, Mat3f c, Mat3f d)
        {
            Mat3f first = multiply(a, b);
            Mat3f second = multiply(c, d);
            return multiply(first, second);
        }

        public static Mat3f objectMatrix(Vec2f position, Vec2f size, Vec2f center)
        {
            Mat3f ret = new Mat3f();

            ret.a = size.x;     ret.d = 0.0f;     ret.g = 0.0f;
            ret.b = 0.0f;       ret.e = size.y;   ret.h = 0.0f;

            ret.c = position.x - center.x;
            ret.f = position.y - center.y;
            ret.i = 1.0f;

            return ret;
        }

        public static Mat3f objectRotationMatrix(Vec2f position, Vec2f size, Vec2f scale, Vec2f center, double angle)
        {
            //Concatenation of several transformations: scale to object size, adjust hotspot, scale, rotation and translate
            double radians = -angle * 0.0174532925f;
            double sino = Math.sin(radians);
            double coso = Math.cos(radians);
            double sxCoso = scale.x * coso;
            double syCoso = scale.y * coso;
            double sxSino = scale.x * sino;
            double sySino = scale.y * sino;

            Mat3f ret = new Mat3f();

            ret.a = size.x * sxCoso;
            ret.d = size.x * sxSino;
            ret.g = 0.0f;

            ret.b = -size.y * sySino;
            ret.e = size.y * syCoso;
            ret.h = 0.0f;

            ret.c = position.x - center.x * sxCoso + center.y * sySino;
            ret.f = position.y - center.y * syCoso - center.x * sxSino;
            ret.i = 1.0f;

            return ret;
        }

        public static Mat3f orthogonalProjectionMatrix(int x, int y, int w, int h)
        {
            double left = x;
            double right = x + w;
            double top = y;
            double bottom = y+h;
            double tx = - (right+left)/(right-left);
            double ty = - (top+bottom)/(top-bottom);

            Mat3f ret = new Mat3f();

            ret.a = 2.0f/(right - left);
            ret.d = 0.0f;
            ret.g = 0.0f;

            ret.b = 0.0f;
            ret.e = 2.0f/(top - bottom);
            ret.h = 0.0f;

            ret.c = tx;
            ret.f = ty;
            ret.i = 0.0f;

            return ret;
        }

        public static Mat3f textureMatrix(double x, double y, double w, double h, double textureWidth, double textureHeight)
        {
            double invW = 1.0f/textureWidth;
            double invH = 1.0f/textureHeight;

            x *= invW;
            y *= invH;
            w *= invW;
            h *= invH;

            Mat3f ret = new Mat3f();

            ret.a = w;     ret.d = 0.0f;  ret.g = 0.0f;
            ret.b = 0.0f;  ret.e = h;     ret.h = 0.0f;
            ret.c = x;     ret.f = y;     ret.i = 1.0f;

            return ret;
        }

        public static Mat3f textureMatrixFlipped(double x, double y, double w, double h, double o, double textureWidth, double textureHeight)
        {
            double invW = 1.0f/textureWidth;
            double invH = 1.0f/textureHeight;

            x *= invW;
            y *= invH;
            w *= invW;
            h *= invH;
            o *= invH;

            Mat3f ret = new Mat3f();

            ret.a = w;     ret.d = 0.0f;  ret.g = 0.0f;
            ret.b = 0.0f;  ret.e = -h;    ret.h = 0.0f;
            ret.c = x;     ret.f = o-y;   ret.i = 1.0f;

            return ret;
        }

        public static Mat3f maskspaceToWorldspace(Vec2f position, Vec2f hotspot, Vec2f scale, double angle)
        {
            double radians = -angle * 0.0174532925f;
            double cosa = Math.cos(radians);
            double sina = Math.sin(radians);
            double sxa = scale.x;
            double sya = scale.y;
            double pxa = position.x;
            double pya = position.y;
            double hxa = hotspot.x;
            double hya = hotspot.y;

            Mat3f ret = new Mat3f();

            ret.a = cosa*sxa;
            ret.d = -sina*sya;
            ret.g = -cosa*hxa*sxa + hya*sina*sya + pxa;

            ret.b = sina*sxa;
            ret.e = cosa*sya;
            ret.h = -cosa*hya*sya - hxa*sina*sxa + pya;

            ret.c = 0.0f;
            ret.f = 0.0f;
            ret.i = 1.0f;

            return ret;
        }

        public static Mat3f worldspaceToMaskspace(Vec2f position, Vec2f hotspot, Vec2f scale, double angle)
        {
            double radians = angle * 0.0174532925f;
            double cosb = Math.cos(radians);
            double sinb = Math.sin(radians);
            double sxb = 1.0f/scale.x;
            double syb = 1.0f/scale.y;
            double pxb = position.x;
            double pyb = position.y;
            double hxb = hotspot.x;
            double hyb = hotspot.y;

            Mat3f ret = new Mat3f();

            ret.a = cosb*sxb;
            ret.d = -sinb*sxb;
            ret.g = -cosb*pxb*sxb + hxb + pyb*sinb*sxb;

            ret.b = sinb*syb;
            ret.e = cosb*syb;
            ret.h = -cosb*pyb*syb + hyb - pxb*sinb*syb;

            ret.c = 0.0f;
            ret.f = 0.0f;
            ret.i = 1.0f;

            return ret;
        }

        public static Mat3f maskspaceToMaskspace(Vec2f positionA, Vec2f hotspotA, Vec2f scaleA, double angleA, Vec2f positionB, Vec2f hotspotB, Vec2f scaleB, double angleB)
        {
            double radiansA = -angleA * 0.0174532925f;
            double cosa = Math.cos(radiansA);
            double sina = Math.sin(radiansA);
            double sxa = scaleA.x;
            double sya = scaleA.y;
            double pxa = positionA.x;
            double pya = positionA.y;
            double hxa = hotspotA.x;
            double hya = hotspotA.y;

            double radiansB = angleB * 0.0174532925f;
            double cosb = Math.cos(radiansB);
            double sinb = Math.sin(radiansB);
            double sxb = 1.0f/scaleB.x;
            double syb = 1.0f/scaleB.y;
            double pxb = positionB.x;
            double pyb = positionB.y;
            double hxb = hotspotB.x;
            double hyb = hotspotB.y;

            Mat3f ret = new Mat3f();

            ret.a = cosa*cosb*sxa*sxb - sina*sinb*sxa*sxb;
            ret.d = -cosa*sinb*sxb*sya - cosb*sina*sxb*sya;
            ret.g = cosa*(hya*sinb*sxb*sya - cosb*hxa*sxa*sxb) + cosb*(hya*sina*sya + pxa - pxb)*sxb + hxa*sina*sinb*sxa*sxb + hxb - (pya - pyb)*sinb*sxb;

            ret.b = cosa*sinb*sxa*syb + cosb*sina*sxa*syb;
            ret.e = cosa*cosb*sya*syb - sina*sinb*sya*syb;
            ret.h = cosa*(-cosb*hya*sya*syb - hxa*sinb*sxa*syb) - cosb*(hxa*sina*sxa - pya + pyb)*syb + hya*sina*sinb*sya*syb + hyb + pxa*sinb*syb - pxb*sinb*syb;

            ret.c = 0.0f;
            ret.f = 0.0f;
            ret.i = 1.0f;

            return ret;
        }

        public Mat3f transpose()
        {
            Mat3f ret = new Mat3f();

            ret.a = a;
            ret.d = d;
            ret.g = g;

            ret.b = b;
            ret.e = e;
            ret.h = h;

            ret.c = c;
            ret.f = f;
            ret.i = i;

            return ret;
        }

        public double determinant()
        {
            return a*e*i + b*f*g + c*d*h - c*e*g - b*d*i - a*f*h;
        }

        public Mat3f inverted()
        {
            double invD = 1.0f/determinant();
            Mat3f tr = new Mat3f();
            tr.a = invD * (e*i - f*h);
            tr.b = invD * (c*h - b*i);
            tr.c = invD * (b*f - c*e);
            tr.d = invD * (f*g - d*i);
            tr.e = invD * (a*i - c*g);
            tr.f = invD * (c*d - a*f);
            tr.g = invD * (d*h - e*g);
            tr.h = invD * (g*b - a*h);
            tr.i = invD * (a*e - b*d);
            return tr;
        }

        public Mat3f flippedTexCoord(boolean flipX, boolean flipY)
        {
            double iW = a;
            double iH = e;

            double iX = c;
            double iY = f;

            double fX = flipX ? -1 : 1;
            double fY = flipY ? -1 : 1;

            double ftX = flipX ? 1 : 0;
            double ftY = flipY ? 1 : 0;

            Mat3f ret = new Mat3f();

            ret.a = fX * iW;  ret.d = 0.0f;         ret.g = 0.0f;
            ret.b = 0;        ret.e = fY * iH;      ret.h = 0.0f;

            ret.c = ftX*iW + iX;
            ret.f = ftY*iH + iY;
            ret.i = 0.0f;

            return ret;
        }

        public Vec2f transformPoint(Vec2f p)
        {
            return new Vec2f(a*p.x + b*p.y + c, d*p.x + e*p.y + f);
        }
    }

    public static class ColorRGBA
    {
        double r, g, b, a;

        public ColorRGBA()
        {
        }

        public ColorRGBA(ColorRGBA ret)
        {
            r = ret.r;
            g = ret.g;
            b = ret.b;
            a = ret.a;
        }

        public ColorRGBA(int color)
        {
            r = (color & 0x000000FF)/255.0f;
            g = ((color & 0x0000FF00) >> 8)/255.0f;
            b = ((color & 0x00FF0000) >> 16)/255.0f;
            a = 1.0f;
        }

        public ColorRGBA(double red, double green, double blue, double alpha)
        {
            r = red;
            g = green;
            b = blue;
            a = alpha;
        }

    }

    public static class GradientColor
    {
        public ColorRGBA a, b, c, d;

        public GradientColor()
        {
            a = new ColorRGBA();
            b = new ColorRGBA();
            c = new ColorRGBA();
            d = new ColorRGBA();
        }

        public GradientColor(int color)
        {
            a = new ColorRGBA(color);
            b = new ColorRGBA(color);
            c = new ColorRGBA(color);
            d = new ColorRGBA(color);
        }

        public GradientColor(ColorRGBA color)
        {
            a = new ColorRGBA(color);
            b = new ColorRGBA(color);
            c = new ColorRGBA(color);
            d = new ColorRGBA(color);
        }

        public GradientColor(ColorRGBA a, ColorRGBA b, boolean horizontal)
        {
            if(horizontal)
            {
                this.a = new ColorRGBA(a);
                this.c = new ColorRGBA(a);

                this.b = new ColorRGBA(b);
                this.d = new ColorRGBA(b);
            }
            else
            {
                this.a = new ColorRGBA(a);
                this.b = new ColorRGBA(a);

                this.c = new ColorRGBA(b);
                this.d = new ColorRGBA(b);
            }
        }

        public GradientColor(ColorRGBA a, ColorRGBA b, ColorRGBA c, ColorRGBA d)
        {
            this.a = new ColorRGBA(a);
            this.b = new ColorRGBA(b);
            this.c = new ColorRGBA(c);
            this.d = new ColorRGBA(d);
        }

        public GradientColor(int a, int b, int c, int d)
        {
            this.a = new ColorRGBA(a);
            this.b = new ColorRGBA(b);
            this.c = new ColorRGBA(c);
            this.d = new ColorRGBA(d);
        }

        public GradientColor(int a, int b, boolean horizontal)
        {
            if(horizontal)
            {
                this.a = new ColorRGBA(a);
                this.c = new ColorRGBA(a);

                this.b = new ColorRGBA(b);
                this.d = new ColorRGBA(b);
            }
            else
            {
                this.a = new ColorRGBA(a);
                this.b = new ColorRGBA(a);

                this.c = new ColorRGBA(b);
                this.d = new ColorRGBA(b);
            }
        }
    }
}

