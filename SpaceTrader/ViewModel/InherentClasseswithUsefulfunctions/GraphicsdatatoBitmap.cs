
using System;
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
        internal static WriteableBitmap SetBitmap(FastRandom rand, int width, int height, byte[,,] Pixels, byte[] Pixels1d, IEnumerable<IStellarObject> stellarobjects, IEnumerable<IShip> ships, IEnumerable<IShip> stellarobjecttradingships, WriteableBitmap GrdBmp, Int32Rect Rect,  bool drawstarlanes, bool drawships, IStellarObject selectedstellarobject,  IReadOnlyList<IStellarObject> stararray, IShip selectedship, double scalefactor, IStarlane selectedstarlane) 
        {
            ConvertPointArrayto1DPixelArray(rand, width, height, Pixels, Pixels1d, stellarobjects, ships, stellarobjecttradingships, scalefactor, drawships, drawstarlanes,selectedstarlane);
            //write 1d pixel array to bitmap
            GrdBmp.WritePixels(Rect, Pixels1d, 4 * width, 0);

            DrawPathfromSourcetoDestinationstar(GrdBmp, stararray);
            // draw circles around selected objects 
            if (selectedstarlane != null)
            {
                //DrawStarlne(GrdBmp, selectedstellarobject.FinalPosition, Color.FromRgb(200, 100, 100));
            }
            if (selectedstellarobject != null)
            {
                DrawcircleAroundActiveStar(GrdBmp, selectedstellarobject.FinalPosition, Color.FromRgb(200, 100, 100));
            }
            if (selectedship != null && drawships)
            {
                DrawcircleAroundActiveShip(GrdBmp, selectedship.FinalPosition, Color.FromRgb(255, 0, 255));
            }
            Color clr = new Color(); ;
            //additional drawings for stellar objects
            int size = 3;
            foreach (IStellarObject stellarobject in stellarobjects)
            {
                #region draw disaster animation
                if (stellarobject.GlobalDisasterTimer > 0)
                {
                    if (stellarobject.GlobalDisasterTimer > 3)
                    {
                        size = 1;
                        clr = Color.FromRgb(225, 25, 25);
                    }
                    else if (stellarobject.GlobalDisasterTimer > 2)
                    {
                        clr = Color.FromRgb(175, 25, 25);
                        size = 2;
                    }
                    else if (stellarobject.GlobalDisasterTimer > 1)
                    {
                        clr = Color.FromRgb(125, 25, 25);
                        size = 3;
                    }
                    else if (stellarobject.GlobalDisasterTimer > 0)
                    {
                        clr = Color.FromRgb(75, 25, 25);
                        size = 4;
                    }

                    HighlightSelectedStellarObjects(GrdBmp, stellarobject.FinalPosition, clr, size);
                }
                #endregion

                #region highlight stellar object if stellar object type is selected to be highlighted
                if (stellarobject.BHighlightonScreen == true)
                {
                    HighlightSelectedStellarObjects(GrdBmp, stellarobject.FinalPosition, Color.FromRgb(100, 100, 100), 3);
                }
                #endregion
            }
            return GrdBmp;
            // Apparently a WriteableBitmap is automatically converted to an ImageSource.  I am sure this did not work before, but apparently now it does. ? 
            //ImageSource timage = GrdBmp;   
            //return timage;
        }
        /// <summary>
        /// draw initial objects such as stellar objects, ships and starlanes to 1d pixel array
        /// </summary>
        /// <param name="rand">random number</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixels"></param>
        /// <param name="pixels1d"></param>
        /// <param name="stellarobjects"></param>
        /// <param name="ships"></param>
        /// <param name="stellarobjecttradingships"></param>
        /// <param name="scalefactor"></param>
        /// <param name="drawships"></param>
        /// <param name="drawstarlanes"></param>
        /// <param name="selectedstarlane"></param>
        internal static void ConvertPointArrayto1DPixelArray(FastRandom rand, int width, int height, byte[,,] pixels, byte[] pixels1d, IEnumerable<IStellarObject> stellarobjects, IEnumerable<IShip> ships, IEnumerable<IShip> stellarobjecttradingships, double scalefactor, bool drawships, bool drawstarlanes, IStarlane selectedstarlane)
        {
            #region reset bitmap rgb to 0 alpha to 255
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
            #endregion

            #region initialize local variables
            int z, x;
            Color clr;
            double trand;
            double deltax, deltaz;
            int writex, writez;
            double deltaxstep = 0;
            double deltazstep = 0;
            #endregion

            #region drawstellarobjects
            foreach (IStellarObject stellarobject in stellarobjects)
            {
                z = (int)stellarobject.FinalPosition.Z;
                x = (int)stellarobject.FinalPosition.X;
                if (x > 2 && z > 2 && x < (width - 5) && z < (height - 5))
                {
                    if (drawstarlanes)
                    {

                        foreach (IStarlane starlane in stellarobject.StarLanes)
                        {
                            writex = 0;
                            writez = 0;
                            if (starlane.To.FinalPosition.X > 2 && starlane.To.FinalPosition.X < width && starlane.To.FinalPosition.Z > 2 && starlane.To.FinalPosition.Z < height)
                            {
                                deltax = starlane.To.FinalPosition.X - stellarobject.FinalPosition.X;
                                deltaz = starlane.To.FinalPosition.Z - stellarobject.FinalPosition.Z;
                                if (Math.Abs(deltax) > Math.Abs(deltaz))
                                {
                                    deltaxstep = 1;
                                    deltazstep = deltaz / deltax;
                                    for (int i = 1; i < deltax - 1; i++)
                                    {
                                        writex = (int)(stellarobject.FinalPosition.X + (deltaxstep * i));
                                        writez = (int)(stellarobject.FinalPosition.Z + (deltazstep * i));
                                        pixels[writez, writex, 0] = starlane.Color.B;
                                        pixels[writez, writex, 1] = starlane.Color.R;
                                        pixels[writez, writex, 2] = starlane.Color.G;
                                    }
                                }
                                else
                                {
                                    deltaxstep = deltax / deltaz;
                                    deltazstep = 1;
                                    for (int i = 1; i < deltaz - 1; i++)
                                    {
                                        writex = (int)(stellarobject.FinalPosition.X + deltaxstep * i);
                                        writez = (int)(stellarobject.FinalPosition.Z + deltazstep * i);
                                        pixels[writez, writex, 0] = starlane.Color.B;
                                        pixels[writez, writex, 1] = starlane.Color.R;
                                        pixels[writez, writex, 2] = starlane.Color.G;
                                    }
                                }
                            }
                        }
                    }
                    if (scalefactor > 3 || (scalefactor > 1.5 && stellarobject.AbsoluteMagnitude < 5.5) || (scalefactor < 1.5 && stellarobject.AbsoluteMagnitude < 2.8))
                    {
                        clr = stellarobject.StarColor;
                        trand = rand.NextDouble() * 100;
                        if (trand < 2)
                        {
                            clr = stellarobject.StarColorDimmed;
                        }
                        pixels[z, x, 0] = clr.B;
                        pixels[z, x, 1] = clr.R;
                        pixels[z, x, 2] = clr.G;
                    }
                }
            }
            #endregion
            //draw central blackhole still to do

            #region drawselectedstarlane
            if (selectedstarlane != null)
            {
                double loopsize = 0;
                Point3D Startpoint = new Point3D();
                if (selectedstarlane.To.FinalPosition.X > 2 && selectedstarlane.From.FinalPosition.X < width && selectedstarlane.To.FinalPosition.Z > 2 && selectedstarlane.From.FinalPosition.Z < height && selectedstarlane.From.FinalPosition.X > 2 && selectedstarlane.To.FinalPosition.X < width && selectedstarlane.From.FinalPosition.Z > 2 && selectedstarlane.To.FinalPosition.Z < height)
                {
                    deltax = selectedstarlane.To.FinalPosition.X - selectedstarlane.From.FinalPosition.X;
                    deltaz = selectedstarlane.To.FinalPosition.Z - selectedstarlane.From.FinalPosition.Z;
                    if (Math.Abs(deltax) > Math.Abs(deltaz))
                    {
                        if (deltax > 0)
                        {
                            Startpoint = selectedstarlane.From.FinalPosition;
                            loopsize = deltax - 1;
                            deltaxstep = 1;
                            deltazstep = deltaz / deltax;
                        }
                    }
                    else
                    {
                        if (deltaz > 0)
                        {
                            Startpoint = selectedstarlane.From.FinalPosition;
                            loopsize = deltaz - 1;
                            deltaxstep = deltax / deltaz;
                            deltazstep = 1;
                        }
                    }
                    deltax = selectedstarlane.From.FinalPosition.X - selectedstarlane.To.FinalPosition.X;
                    deltaz = selectedstarlane.From.FinalPosition.Z - selectedstarlane.To.FinalPosition.Z;
                    if (Math.Abs(deltax) > Math.Abs(deltaz))
                    {
                        if (deltax > 0)
                        {
                            Startpoint = selectedstarlane.To.FinalPosition;
                            loopsize = deltax - 1;
                            deltaxstep = 1;
                            deltazstep = deltaz / deltax;
                        }
                    }                    
                    else
                    {
                        if (deltaz > 0)
                        {
                            Startpoint = selectedstarlane.To.FinalPosition;
                            loopsize = deltaz - 1;
                            deltaxstep = deltax / deltaz;
                            deltazstep = 1;
                        }
                    }
                    for (int i = 1; i < loopsize; i++)
                    {
                        writex = (int)(Startpoint.X + deltaxstep * i);
                        writez = (int)(Startpoint.Z + deltazstep * i);
                        pixels[writez - 1, writex - 1, 0] = 50;
                        pixels[writez - 1, writex - 1, 1] = 30;
                        pixels[writez - 1, writex - 1, 2] = 30;
                        pixels[writez - 1, writex + 1, 0] = 50;
                        pixels[writez - 1, writex + 1, 1] = 30;
                        pixels[writez - 1, writex + 1, 2] = 30;
                        pixels[writez, writex, 0] = 125;
                        pixels[writez, writex, 1] = 65;
                        pixels[writez, writex, 2] = 65;
                        pixels[writez + 1, writex - 1, 0] = 50;
                        pixels[writez + 1, writex - 1, 1] = 30;
                        pixels[writez + 1, writex - 1, 2] = 30;
                        pixels[writez + 1, writex + 1, 0] = 50;
                        pixels[writez + 1, writex + 1, 1] = 30;
                        pixels[writez + 1, writex + 1, 2] = 30;
                    }
                }
            }
            #endregion

            #region draw ships
            // draw ship pixels
            if (drawships)
            {
                foreach (IShip ship in stellarobjecttradingships)
                {
                    z = (int)ship.FinalPosition.Z;
                    x = (int)ship.FinalPosition.X;

                    if (x > 10 && z > 3 && x < (width - 5) && z < (height - 5))
                    {
                        pixels[z + 1, x + 3, 0] = 150;
                        pixels[z + 1, x + 3, 1] = 150;
                        pixels[z + 1, x + 3, 2] = 150;
                        pixels[z, x + 1, 0] = ship.Color.B;
                        pixels[z, x + 1, 1] = ship.Color.G;
                        pixels[z, x + 1, 2] = ship.Color.R;
                        pixels[z, x + 2, 0] = ship.Color.B;
                        pixels[z, x + 2, 1] = ship.Color.G;
                        pixels[z, x + 2, 2] = ship.Color.R;
                        pixels[z, x + 1, 0] = ship.Color.B;
                        pixels[z, x + 1, 1] = ship.Color.G;
                        pixels[z, x + 1, 2] = ship.Color.R;
                        pixels[z - 1, x + 3, 0] = 150;
                        pixels[z - 1, x + 3, 1] = 150;
                        pixels[z - 1, x  + 3, 2] = 150;
                    }
                }
                foreach (IShip ship in ships)
                {
                    z = (int)ship.FinalPosition.Z;
                    x = (int)ship.FinalPosition.X;

                    if (x > 10 && z > 3 && x < (width - 5) && z < (height - 5))
                    {
                        //pixels[z - 1, x - 3, 0] = 255;
                        //pixels[z - 1, x - 3, 1] = 255;
                        //pixels[z - 1, x - 3, 2] = 255;
                        //pixels[z, x - 3, 0] = 255;
                        //pixels[z, x - 3, 1] = 255;
                        //pixels[z, x - 3, 2] = 255;
                        //pixels[z + 1, x - 3, 0] = 255;
                        //pixels[z + 1, x - 3, 1] = 255;
                        //pixels[z + 1, x - 3, 2] = 255;
                        //pixels[z, x - 4, 0] = 255;
                        //pixels[z, x - 4, 1] = 255;
                        //pixels[z, x - 4, 2] = 255;
                        //pixels[z, x - 5, 0] = 255;
                        //pixels[z, x - 5, 1] = 255;
                        //pixels[z, x - 5, 2] = 255;
                        //pixels[z, x - 7, 0] = 200;
                        //pixels[z, x - 7, 1] = 200;
                        //pixels[z, x - 7, 2] = 200;
                        //pixels[z, x - 8, 0] = 200;
                        //pixels[z, x - 8, 1] = 200;
                        //pixels[z, x - 8, 2] = 200;
                        pixels[z + 1, x + 3, 0] = 150;
                        pixels[z + 1, x + 3, 1] = 150;
                        pixels[z + 1, x + 3, 2] = 150;
                        pixels[z, x + 1, 0] = ship.EconomicEntity.Color.B;
                        pixels[z, x + 1, 1] = ship.EconomicEntity.Color.G;
                        pixels[z, x + 1, 2] = ship.EconomicEntity.Color.R;
                        pixels[z, x + 2, 0] = ship.EconomicEntity.Color.B;
                        pixels[z, x + 2, 1] = ship.EconomicEntity.Color.G;
                        pixels[z, x + 2, 2] = ship.EconomicEntity.Color.R;
                        pixels[z, x + 1, 0] = ship.EconomicEntity.Color.B;
                        pixels[z, x + 1, 1] = ship.EconomicEntity.Color.G;
                        pixels[z, x + 1, 2] = ship.EconomicEntity.Color.R;
                        pixels[z - 1, x + 3, 0] = 150;
                        pixels[z - 1, x + 3, 1] = 150;
                        pixels[z - 1, x + 3, 2] = 150;
                        //pixels[z, x - 6, 0] = ship.EconomicEntity.Color.B;
                        //pixels[z, x - 6, 1] = ship.EconomicEntity.Color.G;
                        //pixels[z, x - 6, 2] = ship.EconomicEntity.Color.R;
                        //pixels[z + 1, x - 6, 0] = ship.EconomicEntity.Color.B;
                        //pixels[z + 1, x - 6, 1] = ship.EconomicEntity.Color.G;
                        //pixels[z + 1, x - 6, 2] = ship.EconomicEntity.Color.R;
                        //pixels[z, x - 1, 0] = 0;
                        //pixels[z, x - 1, 1] = 0;
                        //pixels[z, x - 1, 2] = 200;
                        //pixels[z, x, 0] = 0;
                        //pixels[z, x, 1] = 0;
                        //pixels[z, x, 2] = 200;
                        //pixels[z, x + 1, 0] = 0;
                        //pixels[z, x + 1, 1] = 100;
                        //pixels[z, x + 1, 2] = 200;
                        //pixels[z - 1, x - 1, 0] = 0;
                        //pixels[z - 1, x - 1, 1] = 0;
                        //pixels[z - 1, x - 1, 2] = 200;
                        //pixels[z - 1, x, 0] = 0;
                        //pixels[z - 1, x, 1] = 0;
                        //pixels[z - 1, x, 2] = 200;
                        //pixels[z - 1, x + 1, 0] = 0;
                        //pixels[z - 1, x + 1, 1] = 100;
                        //pixels[z - 1, x + 1, 2] = 200;
                    }
                }
            }
            #endregion

            #region write pixels to 1 dim pixel array
            int index = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int i = 0; i < 4; i++)
                        pixels1d[index++] = pixels[row, col, i];
                }
            }
            #endregion
        }

        #region functions that draw extra structure to bitmap after initial objects have been written to the 1d pixel array and this array has been written to the bitmap

        /// <summary>
        /// the multinode path from players own fleet, when a distant destination is set.
        /// </summary>
        /// <param name="Grdbmp"></param>
        /// <param name="stellarobjectarray"></param>
        public static void DrawPathfromSourcetoDestinationstar(WriteableBitmap Grdbmp, IReadOnlyList<IStellarObject> stellarobjectarray) 
        {
            Color clr = Color.FromRgb(75, 75, 75);
            Point tpnt1, tpnt2;
            for (int i = 0; i < stellarobjectarray.Count - 1; i++)
            {
                tpnt1 = new Point(stellarobjectarray[i].FinalPosition.X, stellarobjectarray[i].FinalPosition.Z);
                tpnt2 = new Point(stellarobjectarray[i + 1].FinalPosition.X, stellarobjectarray[i + 1].FinalPosition.Z);
                Grdbmp.DrawLine((int)tpnt1.X, (int)tpnt1.Y, (int)tpnt2.X, (int)tpnt2.Y, clr);
            }
        }
        /// <summary>
        /// highlight all the instances of a selected stellar object type
        /// </summary>
        /// <param name="Grdbmp"></param>
        /// <param name="pnt"></param>
        /// <param name="clr"></param>
        /// <param name="size"></param>
        public static void HighlightSelectedStellarObjects(WriteableBitmap Grdbmp, Point3D pnt, Color clr, int size)
        {
            Grdbmp.DrawEllipse((int)pnt.X - size, (int)pnt.Z - size, (int)pnt.X + size, (int)pnt.Z + size, clr);
        }
        /// <summary>
        /// Draw selected star on screen
        /// </summary>
        /// <param name="Grdbmp"></param>
        /// <param name="pnt"></param>
        /// <param name="clr"></param>
        public static void DrawcircleAroundActiveStar(WriteableBitmap Grdbmp, Point3D pnt, Color clr)
        {

            Grdbmp.DrawEllipse((int)pnt.X - 3, (int)pnt.Z - 3, (int)pnt.X + 3, (int)pnt.Z + 3, clr);
        }
        /// <summary>
        /// draw selected ship on screen
        /// </summary>
        /// <param name="Grdbmp"></param>
        /// <param name="pnt"></param>
        /// <param name="clr"></param>
        public static void DrawcircleAroundActiveShip(WriteableBitmap Grdbmp, Point3D pnt, Color clr)
        {
            Grdbmp.DrawEllipse((int)pnt.X - 3, (int)pnt.Z - 3, (int)pnt.X + 3, (int)pnt.Z + 3, clr);
        }
        #endregion
    }
}
