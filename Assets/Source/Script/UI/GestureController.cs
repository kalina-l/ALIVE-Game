﻿//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;

using System.Collections.Generic;

namespace DigitalRubyShared
{
    public class GestureController
    {
        private const float xPadding = 0.03f;

        #region Gesture Patterns

        private static readonly Dictionary<ImageGestureImage, string> recognizableImages = new Dictionary<ImageGestureImage, string>
        {
            // check-mark
            { new ImageGestureImage(new ulong[] { 0x000000000000003C, 0x000000000000003E, 0x000000000000003F, 0x000000000000003F, 0x000000000000003F, 0x000000000000003B, 0x000000000000003B, 0x0000000000000078, 0x0000000000000078, 0x0000000000000070, 0x0000000000000070, 0x0000000000000070, 0x0000000000000070, 0x0000000000000070, 0x00000000000000F0, 0x00000000000000F0 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x000000000000003C, 0x000000000000007E, 0x000000000000007F, 0x00000000000000FF, 0x00000000000001F7, 0x00000000000001E3, 0x00000000000003C3, 0x00000000000007C0, 0x0000000000000F80, 0x0000000000000F00, 0x0000000000001E00, 0x0000000000003E00, 0x0000000000003C00, 0x0000000000007800, 0x000000000000F800, 0x000000000000F000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x000000000000007E, 0x00000000000000FF, 0x00000000000001FF, 0x00000000000003E7, 0x00000000000007C7, 0x0000000000000F83, 0x0000000000001F03, 0x0000000000003E00, 0x0000000000007C00, 0x000000000000F800, 0x000000000000F000, 0x000000000000E000, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x00000000000001FF, 0x0000000000000FFF, 0x0000000000003FE7, 0x000000000000FF87, 0x000000000000FC03, 0x000000000000F003, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x00000000000007FC, 0x0000000000000FFE, 0x0000000000000F1E, 0x0000000000001E0F, 0x0000000000001E0F, 0x0000000000003C07, 0x0000000000003C07, 0x0000000000007803, 0x0000000000007803, 0x000000000000F000, 0x000000000000F000, 0x000000000000E000, 0x000000000000E000, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x000000000000007E, 0x000000000000007E, 0x000000000000007F, 0x000000000000007F, 0x00000000000000F7, 0x00000000000000F7, 0x00000000000000E3, 0x00000000000000E3, 0x00000000000001E0, 0x00000000000001E0, 0x00000000000001C0, 0x00000000000003C0, 0x00000000000003C0, 0x0000000000000380, 0x0000000000000380, 0x0000000000000380 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x000000000000000E, 0x000000000000001F, 0x000000000000003F, 0x000000000000003F, 0x000000000000007F, 0x000000000000007B, 0x00000000000000F3, 0x00000000000001F3, 0x00000000000001E0, 0x00000000000003C0, 0x00000000000007C0, 0x0000000000000F80, 0x0000000000000F00, 0x0000000000001E00, 0x0000000000003E00, 0x0000000000003C00 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x00000000000001F8, 0x00000000000001FC, 0x00000000000003FC, 0x00000000000003DE, 0x000000000000079F, 0x000000000000078F, 0x0000000000000F07, 0x0000000000000F07, 0x0000000000001E03, 0x0000000000003E03, 0x0000000000003C00, 0x0000000000007800, 0x0000000000007800, 0x000000000000F000, 0x000000000000F000, 0x000000000000E000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x00000000000001FE, 0x00000000000003FF, 0x00000000000007CF, 0x0000000000000F87, 0x0000000000001F03, 0x0000000000001E00, 0x0000000000001C00, 0x0000000000001C00, 0x0000000000003C00, 0x0000000000003C00, 0x0000000000003800, 0x0000000000007800, 0x0000000000007800, 0x0000000000007000, 0x000000000000F000, 0x000000000000F000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x00000000000000FF, 0x00000000000007FF, 0x0000000000003FFB, 0x000000000000FFC3, 0x000000000000FE03, 0x000000000000F000, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x000000000000007F, 0x00000000000000FF, 0x00000000000003FB, 0x0000000000000FE3, 0x0000000000001FC3, 0x0000000000007F03, 0x000000000000FC03, 0x000000000000F803, 0x000000000000E000, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x000000000000003E, 0x000000000000007F, 0x00000000000000FF, 0x00000000000001F7, 0x00000000000003E3, 0x00000000000007C3, 0x0000000000000780, 0x0000000000000F00, 0x0000000000001F00, 0x0000000000003E00, 0x0000000000007C00, 0x000000000000F800, 0x000000000000F000, 0x000000000000E000, 0x000000000000C000, 0x0000000000000000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x000000000000000F, 0x000000000000001F, 0x000000000000003F, 0x000000000000007F, 0x00000000000000FB, 0x00000000000001F0, 0x00000000000003E0, 0x00000000000007C0, 0x0000000000000F80, 0x0000000000001F00, 0x0000000000003E00, 0x0000000000007C00, 0x000000000000F800, 0x000000000000F000, 0x000000000000E000, 0x000000000000C000 }, 16), "Check Mark" },
            { new ImageGestureImage(new ulong[] { 0x000000000000007F, 0x00000000000000FF, 0x00000000000001F3, 0x00000000000007E3, 0x0000000000000FC3, 0x0000000000001F80, 0x0000000000003E00, 0x000000000000FC00, 0x000000000000F800, 0x000000000000F000, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Check Mark" },

            // lightning bolt
            { new ImageGestureImage(new ulong[] { 0x0000000000000007, 0x000000000000000F, 0x000000000000001F, 0x000000000000003E, 0x000000000000007C, 0x00000000000000F8, 0x00000000000001F0, 0x00000000000007FF, 0x00000000000007FF, 0x00000000000007FF, 0x000000000000071F, 0x000000000000003E, 0x000000000000007C, 0x00000000000000F8, 0x00000000000001F0, 0x00000000000001E0 }, 16), "Lightning Bolt" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000070, 0x0000000000000070, 0x0000000000000070, 0x0000000000000070, 0x0000000000000070, 0x0000000000000070, 0x000000000000007F, 0x000000000000007F, 0x000000000000007F, 0x000000000000007F, 0x000000000000007B, 0x0000000000000073, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003 }, 16), "Lightning Bolt" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000003, 0x0000000000000007, 0x0000000000000007, 0x000000000000000F, 0x000000000000000F, 0x000000000000001F, 0x000000000000001F, 0x000000000000003F, 0x000000000000003F, 0x000000000000003F, 0x000000000000000F, 0x000000000000000F, 0x000000000000001E, 0x000000000000001E, 0x000000000000003C, 0x000000000000003C }, 16), "Lightning Bolt" },

            // one horizontal line
            { new ImageGestureImage(new ulong[] { 0x000000000000FFFF, 0x000000000000FFFF, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Horizontal Line" },

            // one vertical line
            { new ImageGestureImage(new ulong[] { 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000003 }, 16), "Vertical Line" },

            // diagonal line (bottom left to top right)
            { new ImageGestureImage(new ulong[] { 0x0000000000000003, 0x0000000000000007, 0x000000000000000F, 0x000000000000001F, 0x000000000000003E, 0x000000000000007C, 0x00000000000000F8, 0x00000000000001F0, 0x00000000000003E0, 0x00000000000007C0, 0x0000000000000F80, 0x0000000000001F00, 0x0000000000003E00, 0x0000000000007C00, 0x000000000000F800, 0x000000000000F000 }, 16), "Diagonal Line /" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000003, 0x0000000000000003, 0x0000000000000003, 0x0000000000000007, 0x0000000000000007, 0x0000000000000007, 0x0000000000000007, 0x000000000000000F, 0x000000000000000F, 0x000000000000000E, 0x000000000000000E, 0x000000000000001E, 0x000000000000001E, 0x000000000000001C, 0x000000000000003C, 0x000000000000003C }, 16), "Diagonal Line /" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000003, 0x0000000000000007, 0x0000000000000007, 0x000000000000000F, 0x000000000000000F, 0x000000000000001E, 0x000000000000001E, 0x000000000000003C, 0x000000000000003C, 0x0000000000000078, 0x0000000000000078, 0x00000000000000F0, 0x00000000000000F0, 0x00000000000001E0, 0x00000000000003E0, 0x00000000000003C0 }, 16), "Diagonal Line /" },
            { new ImageGestureImage(new ulong[] { 0x000000000000001F, 0x000000000000003F, 0x00000000000000FE, 0x00000000000003F8, 0x0000000000000FF0, 0x0000000000001FC0, 0x0000000000007F00, 0x000000000000FC00, 0x000000000000F800, 0x000000000000E000, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Diagonal Line /" },
            { new ImageGestureImage(new ulong[] { 0x000000000000007F, 0x00000000000007FF, 0x0000000000003FFC, 0x000000000000FFE0, 0x000000000000FE00, 0x000000000000F000, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Diagonal Line /" },
            { new ImageGestureImage(new ulong[] { 0x00000000000007FF, 0x000000000000FFFF, 0x000000000000FFF0, 0x000000000000FE00, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Diagonal Line /" },
            { new ImageGestureImage(new ulong[] { 0x000000000000000F, 0x000000000000001F, 0x000000000000003E, 0x000000000000007C, 0x00000000000000F8, 0x00000000000003F0, 0x00000000000007E0, 0x0000000000000FC0, 0x0000000000001F00, 0x0000000000003E00, 0x0000000000007C00, 0x000000000000F800, 0x000000000000F000, 0x000000000000E000, 0x000000000000C000, 0x0000000000000000 }, 16), "Diagonal Line /" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000003, 0x0000000000000007, 0x000000000000000F, 0x000000000000000F, 0x000000000000001E, 0x000000000000003E, 0x000000000000007C, 0x0000000000000078, 0x00000000000000F0, 0x00000000000001F0, 0x00000000000003E0, 0x00000000000003C0, 0x0000000000000780, 0x0000000000000F80, 0x0000000000001F00, 0x0000000000001E00 }, 16), "Diagonal Line /" },
            { new ImageGestureImage(new ulong[] { 0x000000000000000F, 0x000000000000001F, 0x000000000000003E, 0x000000000000007C, 0x00000000000000F8, 0x00000000000001F0, 0x00000000000003E0, 0x00000000000007C0, 0x0000000000000F80, 0x0000000000001F00, 0x0000000000003E00, 0x0000000000007C00, 0x000000000000F800, 0x000000000000F000, 0x000000000000E000, 0x000000000000C000 }, 16), "Diagonal Line /" },
            { new ImageGestureImage(new ulong[] { 0x000000000000003F, 0x00000000000000FF, 0x00000000000003FC, 0x0000000000000FF0, 0x0000000000003FC0, 0x000000000000FF00, 0x000000000000FC00, 0x000000000000F000, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Diagonal Line /" },

            // diagonal line (top left to bottom right)
            { new ImageGestureImage(new ulong[] { 0x0000000000001E00, 0x0000000000001F00, 0x0000000000000F80, 0x0000000000000780, 0x00000000000003C0, 0x00000000000003E0, 0x00000000000001F0, 0x00000000000000F0, 0x0000000000000078, 0x000000000000007C, 0x000000000000003E, 0x000000000000001E, 0x000000000000000F, 0x000000000000000F, 0x0000000000000007, 0x0000000000000003 }, 16), "Diagonal Line \\" },
            { new ImageGestureImage(new ulong[] { 0x00000000000003C0, 0x00000000000003E0, 0x00000000000001E0, 0x00000000000000F0, 0x00000000000000F0, 0x0000000000000078, 0x0000000000000078, 0x000000000000003C, 0x000000000000003E, 0x000000000000001E, 0x000000000000000F, 0x000000000000000F, 0x0000000000000007, 0x0000000000000007, 0x0000000000000003, 0x0000000000000003 }, 16), "Diagonal Line \\" },
            { new ImageGestureImage(new ulong[] { 0x000000000000E000, 0x000000000000F000, 0x000000000000F800, 0x0000000000007C00, 0x0000000000003E00, 0x0000000000001F00, 0x0000000000000F80, 0x00000000000007C0, 0x00000000000003E0, 0x00000000000001F0, 0x00000000000000F8, 0x000000000000007C, 0x000000000000003E, 0x000000000000001F, 0x000000000000000F, 0x0000000000000007 }, 16), "Diagonal Line \\" },
            { new ImageGestureImage(new ulong[] { 0x000000000000FE00, 0x000000000000FF80, 0x0000000000001FC0, 0x00000000000007F0, 0x00000000000001FC, 0x00000000000000FF, 0x000000000000003F, 0x000000000000000F, 0x0000000000000003, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Diagonal Line \\" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000078, 0x0000000000000078, 0x000000000000003C, 0x000000000000003C, 0x000000000000001C, 0x000000000000001E, 0x000000000000001E, 0x000000000000000E, 0x000000000000000E, 0x000000000000000F, 0x000000000000000F, 0x0000000000000007, 0x0000000000000007, 0x0000000000000007, 0x0000000000000003, 0x0000000000000003 }, 16), "Diagonal Line \\" },
            { new ImageGestureImage(new ulong[] { 0x000000000000F000, 0x000000000000FC00, 0x0000000000007E00, 0x0000000000003F80, 0x0000000000000FC0, 0x00000000000007F0, 0x00000000000001FC, 0x00000000000000FE, 0x000000000000003F, 0x000000000000000F, 0x0000000000000007, 0x0000000000000003, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "Diagonal Line \\" },

            // circle
            { new ImageGestureImage(new ulong[] { 0x0000000000003FFC, 0x0000000000007FFE, 0x000000000000781F, 0x000000000000F00F, 0x000000000000F007, 0x000000000000E003, 0x000000000000C003, 0x000000000000C003, 0x000000000000C003, 0x000000000000E003, 0x000000000000F007, 0x000000000000F00F, 0x000000000000781F, 0x0000000000007F3E, 0x0000000000003FFE, 0x0000000000003FFC }, 16, 0.05f), "Circle" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000FFE, 0x0000000000000FFF, 0x0000000000001E0F, 0x0000000000001E07, 0x0000000000003C03, 0x0000000000003C03, 0x0000000000003803, 0x0000000000003803, 0x0000000000003803, 0x0000000000003803, 0x0000000000003803, 0x0000000000003C07, 0x0000000000003E0F, 0x0000000000003F9F, 0x0000000000000FFE, 0x00000000000007FE }, 16, 0.05f), "Circle" },
            { new ImageGestureImage(new ulong[] { 0x000000000000FFFF, 0x000000000000FFFF, 0x000000000000E00F, 0x000000000000E003, 0x000000000000C003, 0x000000000000C003, 0x000000000000E007, 0x000000000000E00F, 0x000000000000F83F, 0x000000000000FFFF, 0x0000000000007FFC, 0x0000000000001FF0, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16, 0.05f), "Circle" },
            { new ImageGestureImage(new ulong[] { 0x0000000000001FFF, 0x0000000000003FFF, 0x0000000000007C07, 0x000000000000F803, 0x000000000000F003, 0x000000000000E003, 0x000000000000E007, 0x000000000000E007, 0x000000000000E00F, 0x000000000000C00F, 0x000000000000E01E, 0x000000000000FFFE, 0x000000000000FFFC, 0x0000000000007FF8, 0x0000000000001C70, 0x0000000000000000 }, 16, 0.05f), "Circle" },

            // U
            { new ImageGestureImage(new ulong[] { 0x0000000000001FFE, 0x0000000000001FFF, 0x0000000000003C0F, 0x0000000000003C07, 0x0000000000007807, 0x0000000000007803, 0x0000000000007803, 0x0000000000007003, 0x0000000000007003, 0x0000000000007803, 0x0000000000007803, 0x0000000000003803, 0x0000000000003803, 0x0000000000003803, 0x0000000000003803, 0x0000000000003803 }, 16, 0.05f), "U" },
            { new ImageGestureImage(new ulong[] { 0x0000000000003FFE, 0x0000000000007FFF, 0x000000000000F81F, 0x000000000000F007, 0x000000000000E007, 0x000000000000E007, 0x000000000000E003, 0x000000000000E003, 0x000000000000C003, 0x000000000000C003, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16, 0.05f), "U" },

            // V
            { new ImageGestureImage(new ulong[] { 0x00000000000003E0, 0x00000000000007F0, 0x00000000000007F0, 0x0000000000000F78, 0x0000000000001F78, 0x0000000000003E3C, 0x0000000000007C3C, 0x000000000000781E, 0x000000000000701E, 0x000000000000F00F, 0x000000000000F00F, 0x000000000000F007, 0x000000000000E007, 0x000000000000C003, 0x000000000000C003, 0x0000000000000000 }, 16), "V" },
            { new ImageGestureImage(new ulong[] { 0x00000000000001E0, 0x00000000000003E0, 0x00000000000003F0, 0x00000000000007F0, 0x00000000000007F8, 0x0000000000000F78, 0x0000000000000F3C, 0x0000000000001E3C, 0x0000000000001E1E, 0x0000000000003C1E, 0x0000000000003C0F, 0x000000000000780F, 0x000000000000F807, 0x000000000000F007, 0x000000000000E003, 0x000000000000E003 }, 16), "V" },
            { new ImageGestureImage(new ulong[] { 0x00000000000000F0, 0x00000000000000F0, 0x00000000000001F8, 0x00000000000001F8, 0x00000000000003FC, 0x00000000000003FC, 0x000000000000039C, 0x000000000000079E, 0x000000000000079E, 0x0000000000000F0F, 0x0000000000000F0F, 0x0000000000001F07, 0x0000000000001E07, 0x0000000000003C03, 0x0000000000003C03, 0x0000000000003803 }, 16), "V" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000FE0, 0x0000000000001FF0, 0x0000000000001EF8, 0x0000000000001C7C, 0x0000000000003C3E, 0x0000000000003C1E, 0x000000000000780F, 0x000000000000780F, 0x000000000000F007, 0x000000000000F003, 0x000000000000E003, 0x000000000000E000, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16), "V" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000F80, 0x0000000000000FC0, 0x0000000000001FC0, 0x0000000000001FE0, 0x0000000000003DF0, 0x0000000000003CF0, 0x0000000000007878, 0x0000000000007878, 0x000000000000F03C, 0x000000000000F03C, 0x000000000000E01E, 0x000000000000E01F, 0x000000000000C00F, 0x0000000000000007, 0x0000000000000007, 0x0000000000000003 }, 16), "V" },

            // X
            { new ImageGestureImage(new ulong[] { 0x0000000000000F07, 0x0000000000000F87, 0x00000000000007CF, 0x00000000000003CF, 0x00000000000001FE, 0x00000000000001FE, 0x00000000000000FC, 0x00000000000000F8, 0x00000000000000F8, 0x00000000000001FC, 0x00000000000003FC, 0x00000000000007DE, 0x000000000000079F, 0x0000000000000F0F, 0x0000000000001F07, 0x0000000000001E03 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000F803, 0x000000000000FC07, 0x0000000000003E07, 0x0000000000001E0F, 0x0000000000000F1F, 0x0000000000000FBE, 0x00000000000007FC, 0x00000000000003F8, 0x00000000000003F0, 0x00000000000007F0, 0x00000000000007F8, 0x0000000000000F7C, 0x0000000000001F3E, 0x0000000000003E1F, 0x0000000000007C0F, 0x0000000000007807 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000FC07, 0x000000000000FF1F, 0x0000000000003FFF, 0x0000000000000FFE, 0x00000000000007F8, 0x0000000000001FF8, 0x0000000000003FFE, 0x000000000000FE7F, 0x000000000000F81F, 0x000000000000F007, 0x000000000000C003, 0x0000000000000003, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x0000000000007C1C, 0x0000000000007E3C, 0x0000000000001F7C, 0x0000000000000FF8, 0x00000000000007F0, 0x00000000000003F0, 0x00000000000001F0, 0x00000000000003F8, 0x00000000000007FC, 0x0000000000000FBE, 0x0000000000001F1F, 0x0000000000003F0F, 0x0000000000007C07, 0x000000000000F803, 0x000000000000F000, 0x000000000000E000 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000F01F, 0x000000000000F81F, 0x0000000000007C3C, 0x0000000000003E7C, 0x0000000000001FF8, 0x0000000000000FF0, 0x00000000000007E0, 0x00000000000007E0, 0x0000000000000FF0, 0x0000000000001FF8, 0x0000000000003E7C, 0x0000000000007C3E, 0x000000000000F81F, 0x000000000000F00F, 0x000000000000E007, 0x000000000000E003 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000F000, 0x000000000000F800, 0x0000000000007C07, 0x0000000000003E0F, 0x0000000000001E3F, 0x0000000000000FFE, 0x0000000000000FFC, 0x00000000000007F0, 0x0000000000000FE0, 0x0000000000001FE0, 0x0000000000007FF0, 0x000000000000FCF8, 0x000000000000F87C, 0x000000000000E03E, 0x000000000000C01E, 0x000000000000000E }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000E00F, 0x000000000000F01F, 0x000000000000F87E, 0x0000000000007CFC, 0x0000000000003FF8, 0x0000000000001FE0, 0x0000000000000FC0, 0x0000000000001F80, 0x0000000000003FC0, 0x000000000000FFE0, 0x000000000000F9F0, 0x000000000000F0F8, 0x000000000000E07C, 0x000000000000003C, 0x000000000000001C, 0x000000000000001C }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000E000, 0x000000000000F000, 0x000000000000F800, 0x0000000000007C03, 0x0000000000003E07, 0x0000000000001F07, 0x0000000000000F8F, 0x00000000000007DF, 0x00000000000003FE, 0x00000000000001FC, 0x00000000000000F8, 0x00000000000000FC, 0x00000000000001FE, 0x00000000000003FF, 0x00000000000007CF, 0x0000000000000787 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x0000000000001E03, 0x0000000000001F0F, 0x0000000000000F9F, 0x00000000000007FF, 0x00000000000003FC, 0x00000000000001F8, 0x00000000000007F8, 0x0000000000000FFC, 0x0000000000003FBE, 0x0000000000007E1F, 0x000000000000FC0F, 0x000000000000F007, 0x000000000000E003, 0x000000000000C003, 0x000000000000C000, 0x0000000000000000 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000F800, 0x000000000000FC03, 0x0000000000003E03, 0x0000000000001F07, 0x0000000000000FBF, 0x00000000000007FF, 0x00000000000003FE, 0x0000000000000FF0, 0x0000000000003FF8, 0x000000000000FF7C, 0x000000000000FC3E, 0x000000000000F01F, 0x000000000000000F, 0x0000000000000007, 0x0000000000000003, 0x0000000000000000 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000E01F, 0x000000000000F07F, 0x000000000000F8FC, 0x0000000000007FF8, 0x0000000000003FE0, 0x0000000000001FC0, 0x0000000000003F00, 0x0000000000007F80, 0x000000000000FF80, 0x000000000000F380, 0x000000000000E380, 0x000000000000C000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000, 0x0000000000000000 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000E000, 0x000000000000F000, 0x000000000000F800, 0x0000000000007C3C, 0x0000000000003E7C, 0x0000000000001FFC, 0x0000000000000FF0, 0x00000000000007E0, 0x00000000000007E0, 0x0000000000000FF0, 0x0000000000001FF8, 0x0000000000001E7C, 0x0000000000001C3E, 0x000000000000001F, 0x000000000000000F, 0x0000000000000007 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000FC0F, 0x000000000000FE1F, 0x0000000000001F7F, 0x0000000000000FFC, 0x00000000000007F8, 0x00000000000007E0, 0x0000000000001FE0, 0x0000000000003FE0, 0x0000000000007EF0, 0x00000000000078F8, 0x000000000000707C, 0x000000000000003C, 0x000000000000001E, 0x000000000000001F, 0x000000000000000F, 0x0000000000000007 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x000000000000F803, 0x000000000000FE03, 0x0000000000003F07, 0x0000000000001F87, 0x00000000000007EF, 0x00000000000003EF, 0x00000000000001FE, 0x00000000000000FE, 0x000000000000007C, 0x000000000000007C, 0x00000000000000FE, 0x00000000000001FF, 0x00000000000003EF, 0x00000000000007C7, 0x0000000000000783, 0x0000000000000703 }, 16, xPadding), "X" },
            { new ImageGestureImage(new ulong[] { 0x0000000000000007, 0x000000000000C00F, 0x000000000000E01F, 0x000000000000F03E, 0x000000000000F87C, 0x0000000000007CF8, 0x0000000000003FF0, 0x0000000000001FE0, 0x0000000000000FC0, 0x0000000000000FC0, 0x0000000000001FE0, 0x0000000000003FF0, 0x0000000000007CF8, 0x000000000000F87C, 0x000000000000F03C, 0x000000000000E01C }, 16, xPadding), "X" }
        };

        #endregion Gesture Patterns

        private List<List<Vector2>> lineSet = new List<List<Vector2>>();
        private List<Vector2> currentPointList;
        private ImageGestureImage lastImage;

        private FingersScript FingersScript;
        public ImageGestureRecognizer imageGesture = new ImageGestureRecognizer();

        private bool waitForGesture;

        private FeedbackViewController _feedback;

        public GestureController(Transform parent, FeedbackViewController feedback)
        {
            FingersScript = parent.gameObject.AddComponent<FingersScript>();
            _feedback = feedback;

            Start();
        }

        private void UpdateImage()
        {
            lastImage = imageGesture.Image.Clone();

            if (waitForGesture)
            {
                if (imageGesture.MatchedGestureImage == null)
                {
                    Debug.Log("No match");
                }
                else
                {
                    string gestureName = recognizableImages[imageGesture.MatchedGestureImage];

                    Debug.Log("Match: " + recognizableImages[imageGesture.MatchedGestureImage]);

                    switch (gestureName)
                    {
                        case "Horizontal Line":
                        case "X":
                            _feedback.SendFeedBack(-1);
                            break;
                        case "Vertical Line":
                        case "Circle":
                        case "Check Mark":
                            _feedback.SendFeedBack(1);
                            break;
                    }
                    
                }
            }
        }

        public void AskForGesture()
        {
            waitForGesture = true;
            ResetLines();
        }

        public void StopAsking()
        {
            waitForGesture = false;
            ResetLines();
            imageGesture.Reset();
        }

        private void AddTouches(ICollection<GestureTouch> touches)
        {
            GestureTouch? t = null;
            foreach (GestureTouch tt in touches)
            {
                t = tt;
                break;
            }
            if (t != null)
            {
                Vector3 v = new Vector3(t.Value.X, t.Value.Y, 0.0f);
                v = Camera.main.ScreenToWorldPoint(v);

                // Debug.LogFormat("STW: {0},{1} = {2},{3}", t.Value.X, t.Value.Y, v.x, v.y);

                currentPointList.Add(v);
            }
        }

        private void ImageGestureUpdated(GestureRecognizer imageGesture, ICollection<GestureTouch> touches)
        {
            if (imageGesture.State == GestureRecognizerState.Ended)
            {
                AddTouches(touches);
                UpdateImage();

                // note - if you have received an image you care about, you should reset the image gesture, i.e. imageGesture.Reset()
                // the ImageGestureRecognizer doesn't automaticaly Reset like other gestures when it ends because some images need multiple paths
                // which requires lifting the mouse or finger and drawing again
            }
            else if (imageGesture.State == GestureRecognizerState.Began)
            {
                // began
                currentPointList = new List<Vector2>();
                lineSet.Add(currentPointList);
                AddTouches(touches);
            }
            else if (imageGesture.State == GestureRecognizerState.Executing)
            {
                // moving
                AddTouches(touches);
            }
        }

        private void ResetLines()
        {
            currentPointList = null;
            lineSet.Clear();
            UpdateImage();
            
        }

        private void MaximumPathCountExceeded(object sender, System.EventArgs e)
        {
            ResetLines();
        }

        private void Start()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Updated += Tap_Updated;
            FingersScript.AddGesture(tap);

            imageGesture.MaximumPathCount = 2;
            imageGesture.Updated += ImageGestureUpdated;
            imageGesture.MaximumPathCountExceeded += MaximumPathCountExceeded;
            imageGesture.GestureImages = new List<ImageGestureImage>(recognizableImages.Keys);
            FingersScript.AddGesture(imageGesture);

            // imageGesture.Simulate(752, 382, 760, 365, 768, 348, 780, 335, 789, 329, 802, 327, 814, 336, 828, 354, 837, 371, 841, 381, 841, 386);
        }

        private void Tap_Updated(GestureRecognizer gesture, ICollection<GestureTouch> touches)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                UnityEngine.Debug.Log("Tap Gesture Ended");
            }
        }
    }
}
