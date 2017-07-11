using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace _3DCube  //actually, this app is used for a rectangular
{
    class Math3D
    {
        const double PIOVER180 = Math.PI / 180.0;
        public enum Side { Front, Back, Left, Right, Top, Bottom }  //6 sides of a rectangular

        public class Vector3D
        {
            public float x;
            public float y;
            public float z;

            public Vector3D(int _x, int _y, int _z) { x = _x;  y = _y;  z = _z; }
            public Vector3D(float _x, float _y, float _z) { x = _x; y = _y; z = _z;}
            public Vector3D(double _x, double _y, double _z) { x = (float)_x;  y = (float)_y;  z = (float)_z; }
            public Vector3D() { } //Constructor
            public override string ToString()
            {
                return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
            }            
        }
        //====================================================================        
        public class Cube
        {
            //Cube face, has four points, 3D and 2D
            internal class Face : IComparable<Face> //interface for comparison method
            {
                //public enum Side { Front, Back, Left, Right, Top, Bottom}
                public PointF[] Corners2D; //get from System.Drawing
                public Vector3D[] Corners3D;
                public Vector3D Center;
                public Side CubeSide; //Front, Back, ...

                public Face() { }
                public int CompareTo(Face otherFace)
                {
                    return (int)(this.Center.z - otherFace.Center.z); //In order of which is closest to the screen
                }
            }

            public int width = 0;
            public int height = 0;
            public int depth = 0;
            float xRotation = 0.0f;  //the current xRotation of this 3DCube in compare with the original position
            float yRotation = 0.0f;
            float zRotation = 0.0f;

            bool drawWires = true;
            bool fillFront;
            bool fillBack = true;
            bool fillLeft;
            bool fillRight = true;
            bool fillTop;
            bool fillBottom = true;

            Vector3D cubeOrigin;
            Face[] faces;

            //========= GET, SET the value of attributes ========================
            public float RotateX  //xRotation get, set accessor or modifier
            {
                get { return xRotation; }
                set
                {   //Perform two tasks: 1. Rotate the Cube     2. set new value to xRotation
                    //rotate an angle around X-axis: deltaX = value - xRotation
                    RotateCubeX(value - xRotation);
                    //update current xRotation
                    xRotation = value;
                }
            }

            public float RotateY
            {
                get { return yRotation; }
                set
                {
                    //rotate an angle around Y-axis: deltaY = value - yRotation
                    RotateCubeY(value - yRotation);
                    //update current yRotation
                    yRotation = value;
                }
            }

            public float RotateZ
            {
                get { return zRotation; }
                set
                {
                    //rotate an angle around Z-axis: deltaZ = value - zRotation
                    RotateCubeZ(value - zRotation);
                    //update current zRotation
                    zRotation = value;
                }
            }

            public bool DrawWires
            {
                get { return drawWires; }
                set { drawWires = value; }
            }

            public bool FillFront
            {
                get { return fillFront; }
                set { fillFront = value; }
            }

            public bool FillBack
            {
                get { return fillBack; }
                set { fillBack = value; }
            }

            public bool FillLeft
            {
                get { return fillLeft; }
                set { fillLeft = value; }
            }

            public bool FillRight
            {
                get { return fillRight; }
                set { fillRight = value; }
            }

            public bool FillTop
            {
                get { return fillTop; }
                set { fillTop = value; }
            }

            public bool FillBottom
            {
                get { return fillBottom; }
                set { fillBottom = value; }
            }

            #region Initializers         

            public Cube(int _width, int _height, int _depth) 
            {
                width = _width;
                height = _height;
                depth = _depth;
                cubeOrigin = new Math3D.Vector3D(width / 2, height / 2, depth / 2);

                InitializeCube();
            }

            #endregion

            private void InitializeCube() //Given width, height, depth | cubeCenter = (width/2, height/2, depth/2)
            {
                //Fill in the cube
                faces = new Face[6]; //cube has 6 cubes

                //Front Face ----------------------------------------
                faces[0] = new Face();
                faces[0].CubeSide = Side.Front;
                faces[0].Corners3D = new Vector3D[4];
                faces[0].Corners3D[0] = new Vector3D(0, 0, 0);
                faces[0].Corners3D[1] = new Vector3D(0, height, 0);
                faces[0].Corners3D[2] = new Vector3D(width, height, 0);
                faces[0].Corners3D[3] = new Vector3D(width, 0, 0);
                faces[0].Center = new Vector3D(width / 2, height / 2, 0);

                //Back Face --------------------------------------------
                faces[1] = new Face();
                faces[1].CubeSide = Side.Back;
                faces[1].Corners3D = new Vector3D[4];
                faces[1].Corners3D[0] = new Vector3D(0, 0, depth);
                faces[1].Corners3D[1] = new Vector3D(0, height, depth);
                faces[1].Corners3D[2] = new Vector3D(width, height, depth);
                faces[1].Corners3D[3] = new Vector3D(width, 0, depth);
                faces[1].Center = new Vector3D(width / 2, height / 2, depth);

                //Left Face --------------------------------------------
                faces[2] = new Face();
                faces[2].CubeSide = Side.Left;
                faces[2].Corners3D = new Vector3D[4];
                faces[2].Corners3D[0] = new Vector3D(0, 0, 0);
                faces[2].Corners3D[1] = new Vector3D(0, 0, depth);
                faces[2].Corners3D[2] = new Vector3D(0, height, depth);
                faces[2].Corners3D[3] = new Vector3D(0, height, 0);
                faces[2].Center = new Vector3D(0, height / 2, depth / 2);

                //Right Face --------------------------------------------
                faces[3] = new Face();
                faces[3].CubeSide = Side.Right;
                faces[3].Corners3D = new Vector3D[4];
                faces[3].Corners3D[0] = new Vector3D(width, 0, 0);
                faces[3].Corners3D[1] = new Vector3D(width, 0, depth);
                faces[3].Corners3D[2] = new Vector3D(width, height, depth);
                faces[3].Corners3D[3] = new Vector3D(width, height, 0);
                faces[3].Center = new Vector3D(width, height / 2, depth / 2);

                //Top Face --------------------------------------------
                faces[4] = new Face();
                faces[4].CubeSide = Side.Top;
                faces[4].Corners3D = new Vector3D[4];
                faces[4].Corners3D[0] = new Vector3D(0, 0, 0);
                faces[4].Corners3D[1] = new Vector3D(0, 0, depth);
                faces[4].Corners3D[2] = new Vector3D(width, 0, depth);
                faces[4].Corners3D[3] = new Vector3D(width, 0, 0);
                faces[4].Center = new Vector3D(width / 2, 0, depth / 2);

                //Bottom Face --------------------------------------------
                faces[5] = new Face();
                faces[5].CubeSide = Side.Bottom;
                faces[5].Corners3D = new Vector3D[4];
                faces[5].Corners3D[0] = new Vector3D(0, height, 0);
                faces[5].Corners3D[1] = new Vector3D(0, height, depth);
                faces[5].Corners3D[2] = new Vector3D(width, height, depth);
                faces[5].Corners3D[3] = new Vector3D(width, height, 0);
                faces[5].Center = new Vector3D(width / 2, height, depth / 2);

            }


            //========== CALCULATE THE 2D POINTS OF EACH FACES =============                  
            //We need to identify the coordination of each point in the screen (2D)
            private void Update2DPoints(Point drawOrigin)
            {
                //Update the 2D points of all the faces
                for(int i = 0; i < faces.Length; i++)
                {
                    Update2DPoints(drawOrigin, i);
                }
            }

            private void Update2DPoints(Point drawOrigin, int faceIndex)
            {
                //Calculates the projected coordinates of the 3D points in a cube face
                PointF[] point2D = new PointF[4];                

                //Convert 3D Points to 2D
                Math3D.Vector3D vec;
                for(int i = 0; i < point2D.Length; i++)
                {
                    vec = faces[faceIndex].Corners3D[i];
                    point2D[i] = Get2D(vec, drawOrigin);
                }

                //update face
                faces[faceIndex].Corners2D = point2D;
            }

            //project 3D points to 2D points on screen            
            //Represents an ordered pair of floating-point x- and y-coordinates that defines a point in a two-dimensional plane
            private PointF Get2D(Vector3D vec)
            {
                PointF returnPoint = new PointF();

                float zoom = (float)Screen.PrimaryScreen.Bounds.Width / 1.5f;
                returnPoint.X = (cubeOrigin.x - vec.x) / (-vec.z - zoom) * zoom;
                returnPoint.Y = (cubeOrigin.y - vec.y) / (-vec.z - zoom) * zoom;
                //(distance to cubeOrigin) / (element too see different points for same x same y)

                return returnPoint;                
            }
            //drawOrigin might be the center point on the screen??? I don't know
            private PointF Get2D(Vector3D vec, Point drawOrigin)
            {
                PointF point2D = Get2D(vec);
                return new PointF(point2D.X + drawOrigin.X, point2D.Y + drawOrigin.Y);
            }


            //========= DRAW CUBE =========================================
            //bitmap consists of the pixel data for a graphics image and its attributes
            public Bitmap DrawCube(Point drawOrigin)
            {
                //Get the corresponding 2D of current 3D object
                Update2DPoints(drawOrigin);

                //Get the bounds of the final bitmap
                Rectangle bounds = getDrawingBounds();
                bounds.Width += drawOrigin.X;
                bounds.Height += drawOrigin.Y;

                Bitmap finalBmp = new Bitmap(bounds.Width, bounds.Height);
                Graphics g = Graphics.FromImage(finalBmp);

                g.SmoothingMode = SmoothingMode.AntiAlias;

                Array.Sort(faces); //sort faces from closest to farthest

                for(int i = faces.Length - 1; i >= 0; i--) //fill color faces from back to front
                {
                    switch (faces[i].CubeSide)
                    {
                        case Side.Front:                            
                            if (fillFront) g.FillPolygon(Brushes.White, faces[i].Corners2D);
                            break;
                        case Side.Back:
                            if (fillBack) g.FillPolygon(Brushes.Yellow, faces[i].Corners2D);                            
                            break;
                        case Side.Left:
                            if (fillLeft) g.FillPolygon(Brushes.Green, faces[i].Corners2D);
                            break;
                        case Side.Right:
                            if (fillRight) g.FillPolygon(Brushes.Blue, faces[i].Corners2D);
                            break;
                        case Side.Top:
                            if (fillTop) g.FillPolygon(Brushes.Red, faces[i].Corners2D);
                            break;
                        case Side.Bottom:
                            if (fillBottom) g.FillPolygon(Brushes.Orange, faces[i].Corners2D);
                            break;
                        default:
                            break;
                    }
                    if (drawWires)
                    {
                        g.DrawLine(Pens.Black, faces[i].Corners2D[0], faces[i].Corners2D[1]);
                        g.DrawLine(Pens.Black, faces[i].Corners2D[1], faces[i].Corners2D[2]);
                        g.DrawLine(Pens.Black, faces[i].Corners2D[2], faces[i].Corners2D[3]);
                        g.DrawLine(Pens.Black, faces[i].Corners2D[3], faces[i].Corners2D[0]);
                    }
                }
                g.Dispose(); //show on the screen
                return finalBmp;
            }

            private Rectangle getDrawingBounds()
            {
                //Find the farthest most points to calculate the size of the returning bitmap
                float left = float.MaxValue;
                float right = float.MinValue;
                float bottom = float.MaxValue;
                float top = float.MinValue;

                for(int i = 0; i < faces.Length; i++)
                {
                    for(int j = 0; j < faces[i].Corners2D.Length; j++)
                    {
                        left = Math.Min(left, faces[i].Corners2D[j].X);
                        right = Math.Max(right, faces[i].Corners2D[j].X);
                        bottom = Math.Min(bottom, faces[i].Corners2D[j].Y);     //question about top, bottom
                        top = Math.Max(top, faces[i].Corners2D[j].Y);
                    }
                }

                return new Rectangle(0, 0, (int)Math.Round(right - left), (int)Math.Round(top - bottom));
            }
            //========== ROTATION ==========================================
            //rotate a cube around its cubeOrigin on a plane perpendicular to X-axis by deltaX degrees 
            private void RotateCubeX(float deltaX)
            {

                for (int i = 0; i < faces.Length; i++)
                {
                    //rotate corner points
                    Vector3D point0 = new Vector3D(0, 0, 0); //point0 is the origin coordinate
                    faces[i].Corners3D = Math3D.Translate(faces[i].Corners3D, cubeOrigin, point0); //move corner to origin
                    faces[i].Corners3D = Math3D.RotateX(faces[i].Corners3D, deltaX);
                    faces[i].Corners3D = Math3D.Translate(faces[i].Corners3D, point0, cubeOrigin); //move back


                    //-------Rotate center
                    faces[i].Center = Math3D.Translate(faces[i].Center, cubeOrigin, point0);
                    faces[i].Center = Math3D.RotateX(faces[i].Center, deltaX);
                    faces[i].Center = Math3D.Translate(faces[i].Center, point0, cubeOrigin);
                }
                //the cubeOrigin does not change its position             
            }

            //rotate a cube around its cubeOrigin on a plane perpendicular to Y-axis by deltaY degrees 
            private void RotateCubeY(float deltaY)
            {
                for (int i = 0; i < faces.Length; i++)
                {
                    //Apply rotation
                    //------Rotate points
                    Vector3D point0 = new Vector3D(0, 0, 0);
                    faces[i].Corners3D = Math3D.Translate(faces[i].Corners3D, cubeOrigin, point0); //Move corner to origin
                    faces[i].Corners3D = Math3D.RotateY(faces[i].Corners3D, deltaY);
                    faces[i].Corners3D = Math3D.Translate(faces[i].Corners3D, point0, cubeOrigin); //Move back

                    //-------Rotate center
                    faces[i].Center = Math3D.Translate(faces[i].Center, cubeOrigin, point0);
                    faces[i].Center = Math3D.RotateY(faces[i].Center, deltaY);
                    faces[i].Center = Math3D.Translate(faces[i].Center, point0, cubeOrigin);
                }
            }

            //rotate a cube around its cubeOrigin on a plane perpendicular to Z-axis by deltaZ degrees 
            private void RotateCubeZ(float deltaZ)
            {
                for (int i = 0; i < faces.Length; i++)
                {
                    //Apply rotation
                    //------Rotate points
                    Vector3D point0 = new Vector3D(0, 0, 0);
                    faces[i].Corners3D = Math3D.Translate(faces[i].Corners3D, cubeOrigin, point0); //Move corner to origin
                    faces[i].Corners3D = Math3D.RotateZ(faces[i].Corners3D, deltaZ);
                    faces[i].Corners3D = Math3D.Translate(faces[i].Corners3D, point0, cubeOrigin); //Move back

                    //-------Rotate center
                    faces[i].Center = Math3D.Translate(faces[i].Center, cubeOrigin, point0);
                    faces[i].Center = Math3D.RotateZ(faces[i].Center, deltaZ);
                    faces[i].Center = Math3D.Translate(faces[i].Center, point0, cubeOrigin);
                }
            }
        }
        //====== ROTATION ======================================
        //rotate a point around X-axis
        public static Vector3D RotateX(Vector3D point3D, float degrees)
        {
            //rotation matrix
            //[ 1    0        0   ]
            //[ 0   cos(x)  sin(x)]
            //[ 0   -sin(x) cos(x)]
            double cDegrees = degrees * PIOVER180;
            double cosDegrees = Math.Cos(cDegrees);
            double sinDegrees = Math.Sin(cDegrees);

            //keeping value of x, changing values of y, z
            double y = (point3D.y * cosDegrees) + (point3D.z * sinDegrees);
            double z = (point3D.y * -sinDegrees) + (point3D.z * cosDegrees);

            return new Vector3D(point3D.x, y, z);
        }

        //rotate a point around Y-axis, the formula is the same as X-axis
        public static Vector3D RotateY(Vector3D point3D, float degrees)
        {
            //rotation matrix
            //[ cos(x)   0    sin(x)]
            //[   0      1      0   ]
            //[-sin(x)   0    cos(x)]
            double cDegrees = degrees * PIOVER180;
            double cosDegrees = Math.Cos(cDegrees);
            double sinDegrees = Math.Sin(cDegrees);

            double x = (point3D.x * cosDegrees) + (point3D.z * sinDegrees);
            double z = (point3D.x * -sinDegrees) + (point3D.z * cosDegrees);

            return new Vector3D(x, point3D.y, z);
        }

        //rotate a point around Z-axis, the formula is the same as X-axis
        public static Vector3D RotateZ(Vector3D point3D, float degrees)
        {
            //rotation matrix
            //[ cos(x)  sin(x) 0]
            //[ -sin(x) cos(x) 0]
            //[    0     0     1]
            double cDegrees = degrees * PIOVER180;
            double cosDegrees = Math.Cos(cDegrees);
            double sinDegrees = Math.Sin(cDegrees);

            double x = (point3D.x * cosDegrees) + (point3D.y * sinDegrees);
            double y = (point3D.x * -sinDegrees) + (point3D.y * cosDegrees);

            return new Vector3D(x, y, point3D.z);
        }

        //rotate an array of points around X-axis
        public static Vector3D[] RotateX(Vector3D[] points3D, float degrees)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = RotateX((Vector3D)(points3D[i]), degrees);
            }
            return points3D;
        }

        //rotate an array of points around Y-axis
        public static Vector3D[] RotateY(Vector3D[] points3D, float degrees)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = RotateY((Vector3D)(points3D[i]), degrees);
            }
            return points3D;
        }

        //rotate an array of points around Z-axis
        public static Vector3D[] RotateZ(Vector3D[] points3D, float degrees)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = RotateZ((Vector3D)(points3D[i]), degrees);
            }
            return points3D;
        }


        //====== TRANSLATE =====================================================
        public static Vector3D Translate(Vector3D points3D, Vector3D oldOrigin, Vector3D newOrigin)
        {
            Vector3D difference = new Vector3D(newOrigin.x - oldOrigin.x, newOrigin.y - oldOrigin.y, newOrigin.z - oldOrigin.z);
            points3D.x += difference.x;
            points3D.y += difference.y;
            points3D.z += difference.z;
            return points3D;
        }
        public static Vector3D[] Translate(Vector3D[] points3D, Vector3D oldOrigin, Vector3D newOrigin)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = Translate(points3D[i], oldOrigin, newOrigin);

            }
            return points3D;
        }
    }
}
