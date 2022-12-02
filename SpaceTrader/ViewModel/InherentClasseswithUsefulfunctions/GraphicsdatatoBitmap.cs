
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace SpaceTrader
{
    //class for writing data to bitmap. Only Interfaces used. No data can be written to anything other than the bitmap.
    public class DisplayFunctions
    {
        internal static ImageSource SetImageFromStarArray(int width, int height, byte[,,] Pixels, byte[] Pixels1d, IEnumerable<IStellarObject> stellarobjects, IEnumerable<IShip> ships, WriteableBitmap GrdBmp, Int32Rect Rect,  bool bdrawlines, IStellarObject selectedstellarobject,  IReadOnlyList<IStellarObject> stararray, IShip selectedship, double scalefactor) 
        {
            ConvertPointArrayto1DPixelArray(width, height, Pixels, Pixels1d, stellarobjects, ships, scalefactor);
            GrdBmp.WritePixels(Rect, Pixels1d, 4 * width, 0);
            if (bdrawlines == true)
            {
                DrawStarlanestoBitmap(GrdBmp, stellarobjects, width, height);
            }
            DrawPathfromSourcetoDestinationstar(GrdBmp, stararray);
            // draw circles around selected objects 
            if (selectedstellarobject != null)
            {
                DrawcircleAroundActiveStar(GrdBmp, selectedstellarobject.FinalPosition, Color.FromRgb(200, 100, 100));
            }
            if (selectedship != null)
            {
                DrawcircleAroundActiveShip(GrdBmp, selectedship.FinalPosition, Color.FromRgb(255, 0, 255));
            }
            //draw circles around selected stellar types
            foreach (IStellarObject stellarobject in stellarobjects)
            {
                if (stellarobject.BHighlightonScreen == true)
                {
                    HighlightSelectedStellarObjects(GrdBmp, stellarobject.FinalPosition, Color.FromRgb(100, 100, 100));
                }
            }
            ImageSource timage = GrdBmp;
            return timage;
        }
        internal static void ConvertPointArrayto1DPixelArray(int width, int height, byte[,,] pixels, byte[] pixels1d, IEnumerable<IStellarObject> stellarobjects, IEnumerable<IShip> ships, double scalefactor)
        {
            FastRandom rand = new FastRandom();
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        pixels[row, col, i] = 0;
                    }
                    pixels[row, col, 3] = 255;
                }
            }
            int z, x;
            Color clr;
            int trand;
            foreach (IStellarObject stellarobject in stellarobjects)
            {
                if (scalefactor > 3 || (scalefactor > 1.5 && stellarobject.AbsoluteMagnitude < 5.5) || (scalefactor < 1.5 && stellarobject.AbsoluteMagnitude < 2.8))
                {
                    z = (int)stellarobject.FinalPosition.Z;
                    x = (int)stellarobject.FinalPosition.X;
                    if (x > 2 && z > 2 && x < (width - 5)  && z < (height - 5))
                    {
                        clr = stellarobject.StarColor;
                        trand = rand.Next(1, 100);
                        if (trand < 8)
                        {
                            clr = stellarobject.StarColorDimmed;
                        }
                        pixels[z, x, 0] = clr.B;
                        pixels[z, x, 1] = clr.R;
                        pixels[z, x, 2] = clr.G;
                    }
                }
            }
            //drawship. first make ship class
            foreach(IShip ship in ships)
            {
                z = (int)ship.FinalPosition.Z;
                x = (int)ship.FinalPosition.X;

                if (x > 10 && z > 3 && x < (width - 5) && z < (height -5))
                {
                    pixels[z - 1, x - 3, 0] = 255;
                    pixels[z - 1, x - 3, 1] = 255;
                    pixels[z - 1, x - 3, 2] = 255;
                    pixels[z, x - 3, 0] = 255;
                    pixels[z, x - 3, 1] = 255;
                    pixels[z, x - 3, 2] = 255;
                    pixels[z + 1, x - 3, 0] = 255;
                    pixels[z + 1, x - 3, 1] = 255;
                    pixels[z + 1, x - 3, 2] = 255;
                    pixels[z, x - 4, 0] = 255;
                    pixels[z, x - 4, 1] = 255;
                    pixels[z, x - 4, 2] = 255;
                    pixels[z, x - 5, 0] = 255;
                    pixels[z, x - 5, 1] = 255;
                    pixels[z, x - 5, 2] = 255;
                    pixels[z, x - 7, 0] = 200;
                    pixels[z, x - 7, 1] = 200;
                    pixels[z, x - 7, 2] = 200;
                    pixels[z, x - 8, 0] = 200;
                    pixels[z, x - 8, 1] = 200;
                    pixels[z, x - 8, 2] = 200;
                    pixels[z + 2, x - 5, 0] = ship.EconomicEntity.Color.B;
                    pixels[z + 2, x - 5, 1] = ship.EconomicEntity.Color.G;
                    pixels[z + 2, x - 5, 2] = ship.EconomicEntity.Color.R;
                    pixels[z + 1, x - 5, 0] = ship.EconomicEntity.Color.B;
                    pixels[z + 1, x - 5, 1] = ship.EconomicEntity.Color.G;
                    pixels[z + 1, x - 5, 2] = ship.EconomicEntity.Color.R;
                    pixels[z - 1, x - 5, 0] = ship.EconomicEntity.Color.B;
                    pixels[z - 1, x - 5, 1] = ship.EconomicEntity.Color.G;
                    pixels[z - 1, x - 5, 2] = ship.EconomicEntity.Color.R;
                    pixels[z - 2, x - 5, 0] = ship.EconomicEntity.Color.B;
                    pixels[z - 2, x - 5, 1] = ship.EconomicEntity.Color.G;
                    pixels[z - 2, x - 5, 2] = ship.EconomicEntity.Color.R;
                    pixels[z - 1, x - 6, 0] = ship.EconomicEntity.Color.B;
                    pixels[z - 1, x - 6, 1] = ship.EconomicEntity.Color.G;
                    pixels[z - 1, x - 6, 2] = ship.EconomicEntity.Color.R;
                    pixels[z, x - 6, 0] = ship.EconomicEntity.Color.B;
                    pixels[z, x - 6, 1] = ship.EconomicEntity.Color.G;
                    pixels[z, x - 6, 2] = ship.EconomicEntity.Color.R;
                    pixels[z + 1, x - 6, 0] = ship.EconomicEntity.Color.B;
                    pixels[z + 1, x - 6, 1] = ship.EconomicEntity.Color.G;
                    pixels[z + 1, x - 6, 2] = ship.EconomicEntity.Color.R;
                    //pixels[z + 2, x - 5, 0] = ship.Color.B;
                    //pixels[z + 2, x - 5, 1] = ship.Color.G;
                    //pixels[z + 2, x - 5, 2] = ship.Color.R;
                    //pixels[z + 1, x - 5, 0] = ship.Color.B;
                    //pixels[z + 1, x - 5, 1] = ship.Color.G;
                    //pixels[z + 1, x - 5, 2] = ship.Color.R;
                    //pixels[z - 1, x - 5, 0] = ship.Color.B;
                    //pixels[z - 1, x - 5, 1] = ship.Color.G;
                    //pixels[z - 1, x - 5, 2] = ship.Color.R;
                    //pixels[z - 2, x - 5, 0] = ship.Color.B;
                    //pixels[z - 2, x - 5, 1] = ship.Color.G;
                    //pixels[z - 2, x - 5, 2] = ship.Color.R;
                    //pixels[z - 1, x - 6, 0] = ship.Color.B;
                    //pixels[z - 1, x - 6, 1] = ship.Color.G;
                    //pixels[z - 1, x - 6, 2] = ship.Color.R;
                    //pixels[z, x - 6, 0] = ship.Color.B;
                    //pixels[z, x - 6, 1] = ship.Color.G;
                    //pixels[z, x - 6, 2] = ship.Color.R;
                    //pixels[z + 1, x - 6, 0] = ship.Color.B;
                    //pixels[z + 1, x - 6, 1] = ship.Color.G;
                    //pixels[z + 1, x - 6, 2] = ship.Color.R;

                }
            }
            int index = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int i = 0; i < 4; i++)
                        pixels1d[index++] = pixels[row, col, i];
                }
            }
        }
        #region drawtobitmap
        public static void DrawStarlanestoBitmap(WriteableBitmap Grdbmp, IEnumerable<IStellarObject> stellarobjects, int width, int height)
        {
            Point tpnt, tpnt2;
            foreach (IStellarObject stellarobject in stellarobjects)
            {
                if (stellarobject.FinalPosition.X > 2 && stellarobject.FinalPosition.X < width && stellarobject.FinalPosition.Z > 2 && stellarobject.FinalPosition.Z < height)
                {
                    tpnt = new Point(stellarobject.FinalPosition.X, stellarobject.FinalPosition.Z);
                    foreach (IStarlane starlane in stellarobject.StarLanes)
                    {
                        if (starlane.To.FinalPosition.X > 2 && starlane.To.FinalPosition.X < width && starlane.To.FinalPosition.Z > 2 && starlane.To.FinalPosition.Z < height)
                        {
                            tpnt2 = new Point(starlane.To.FinalPosition.X, starlane.To.FinalPosition.Z);
                           // Grdbmp.DrawLineDotted((int)tpnt.X, (int)tpnt.Y, (int)tpnt2.X, (int)tpnt2.Y, 10, 3, starlane.Color);
                            Grdbmp.DrawLineWu((int)tpnt.X, (int)tpnt.Y, (int)tpnt2.X, (int)tpnt2.Y, 2, starlane.Color.R, starlane.Color.G, starlane.Color.B);
                        }
                    }
                }            }
        }
        public static void DrawPathfromSourcetoDestinationstar(WriteableBitmap Grdbmp, IReadOnlyList<IStellarObject> stellarobjectarray)
        {
            Color clr = Color.FromRgb(200, 100, 200);
            Point tpnt1, tpnt2;
            for (int i = 0; i < stellarobjectarray.Count - 1; i++)
            {
                tpnt1 = new Point(stellarobjectarray[i].FinalPosition.X, stellarobjectarray[i].FinalPosition.Z);
                tpnt2 = new Point(stellarobjectarray[i + 1].FinalPosition.X, stellarobjectarray[i + 1].FinalPosition.Z);
                Grdbmp.DrawLine((int)tpnt1.X, (int)tpnt1.Y, (int)tpnt2.X, (int)tpnt2.Y, clr);
            }
        }

        public static void HighlightSelectedStellarObjects(WriteableBitmap Grdbmp, Point3D pnt, Color clr)
        {
            Grdbmp.DrawEllipse((int)pnt.X - 3, (int)pnt.Z - 3, (int)pnt.X + 3, (int)pnt.Z + 3, clr);
        }
        public static void DrawcircleAroundActiveStar(WriteableBitmap Grdbmp, Point3D pnt, Color clr)
        {

            Grdbmp.DrawEllipse((int)pnt.X - 3, (int)pnt.Z - 3, (int)pnt.X + 3, (int)pnt.Z + 3, clr);
        }
        public static void DrawcircleAroundActiveShip(WriteableBitmap Grdbmp, Point3D pnt, Color clr)
        {

            Grdbmp.DrawEllipse((int)pnt.X - 3, (int)pnt.Z - 3, (int)pnt.X + 3, (int)pnt.Z + 3, clr);
        }
        #endregion
    }
}
