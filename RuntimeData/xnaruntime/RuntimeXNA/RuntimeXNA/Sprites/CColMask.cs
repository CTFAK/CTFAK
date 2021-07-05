//----------------------------------------------------------------------------------
//
// CCOLMASK : masque de collision
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuntimeXNA.Sprites
{
    public class CColMask
    {
        public const short CM_TEST_OBSTACLE = 0;
        public const short CM_TEST_PLATFORM = 1;
        public const int CM_OBSTACLE = 0x0001;
        public const int CM_PLATFORM = 0x0002;
        // Collision mask margins
        public const int COLMASK_XMARGIN = 64;
        public const int COLMASK_YMARGIN = 16;
        public const int HEIGHT_PLATFORM = 6;
        public static ushort[] lMask =
        {
            0xFFFF, // 1111111111111111B
            0x7FFF, // 0111111111111111B
            0x3FFF, // 0011111111111111B
            0x1FFF, // 0001111111111111B
            0x0FFF, // 0000111111111111B
            0x07FF, // 0000011111111111B
            0x03FF, // 0000001111111111B
            0x01FF, // 0000000111111111B
            0x00FF, // 0000000011111111B
            0x007F, // 0000000001111111B
            0x003F, // 0000000000111111B
            0x001F, // 0000000000011111B
            0x000F, // 0000000000001111B
            0x0007, // 0000000000000111B
            0x0003, // 0000000000000011B
            0x0001	// 0000000000000001B
        };
        public static ushort[] rMask =
        {
            0x0000, // 0000000000000000B0
            0x8000, // 1000000000000000B0
            0xC000, // 1100000000000000B1
            0xE000, // 1110000000000000B2
            0xF000, // 1111000000000000B3
            0xF800, // 1111100000000000B4
            0xFC00, // 1111110000000000B5
            0xFE00, // 1111111000000000B6
            0xFF00, // 1111111100000000B7
            0xFF80, // 1111111110000000B8
            0xFFC0, // 1111111111000000B9
            0xFFE0, // 1111111111100000B10
            0xFFF0, // 1111111111110000B11
            0xFFF8, // 1111111111111000B12
            0xFFFC, // 1111111111111100B13
            0xFFFE, // 1111111111111110B14
            0xFFFF	// 1111111111111111B15
        };
        public ushort[] obstacle = null;
        public ushort[] platform = null;
        public int lineWidth;
        public int width;
        public int height;
        public int mX1;
        public int mX2;
        public int mY1;
        public int mY2;
        public int mX1Clip;
        public int mX2Clip;
        public int mY1Clip;
        public int mY2Clip;
        public int mDxScroll;
        public int mDyScroll;

        public static CColMask create(int xx1, int yy1, int xx2, int yy2, int flags)
        {
            CColMask m = new CColMask();

            m.mDxScroll = 0;
            m.mDyScroll = 0;
            m.mX1 = m.mX1Clip = xx1;
            m.mY1 = m.mY1Clip = yy1;
            m.mX2 = m.mX2Clip = xx2;
            m.mY2 = m.mY2Clip = yy2;
            m.width = xx2 - xx1;
            m.height = yy2 - yy1;
            m.lineWidth = ((m.width + 15) & ~15) / 16;
            if ((flags & CM_OBSTACLE) != 0)
            {
                m.obstacle = new ushort[m.lineWidth * m.height+1];
            }
            if ((flags & CM_PLATFORM) != 0)
            {
                m.platform = new ushort[m.lineWidth * m.height+1];
            }
            return m;
        }

        public void setOrigin(int dx, int dy)
        {
            mDxScroll = dx;
            mDyScroll = dy;
        }

        public void fill(ushort value)
        {
            int l = lineWidth * height;
            int s;
            if (obstacle != null)
            {
                for (s = 0; s < l; s++)
                {
                    obstacle[s] = value;
                }
            }
            if (platform != null)
            {
                for (s = 0; s < l; s++)
                {
                    platform[s] = value;
                }
            }
        }

        public void fillRectangle(int x1, int y1, int x2, int y2, int val)
        {
            // Ajouter dx scrolling
            // --------------------
            x1 += mDxScroll;
            x2 += mDxScroll;
            y1 += mDyScroll;
            y2 += mDyScroll;

            // Verifier le clipping
            // --------------------
            if (x1 < mX1Clip)
            {
                x1 = mX1Clip;
            }
            if (x2 > mX2Clip)
            {
                x2 = mX2Clip;
            }
            if (x1 >= x2)
            {
                return;
            }

            if (y1 < mY1Clip)
            {
                y1 = mY1Clip;
            }
            if (y2 > mY2Clip)
            {
                y2 = mY2Clip;
            }
            if (y1 >= y2)
            {
                return;
            }

            x1 -= mX1;
            x2 -= mX1;
            y1 -= mY1;
            y2 -= mY1;

            if (obstacle != null)
            {
                fillRect(obstacle, x1, y1, x2, y2, (val & 1));
            }
            if (platform != null)
            {
                fillRect(platform, x1, y1, x2, y2, (val >> 1) & 1);
            }
        }

        void fillRect(ushort[] mask, int x1, int y1, int x2, int y2, int val)
        {
            int offset = y1 * lineWidth + (x1 & ~15) / 16;
            int h = y2 - y1;
            int w = (x2 / 16) - (x1 / 16) + 1;

            int x, y;
            ushort leftMask;
            ushort rightMask;
            int yOffset;
            if (w > 1)
            {
                if (val == 0)
                {
                    leftMask = (ushort) ~lMask[x1 & 15];
                    rightMask = (ushort) ~rMask[x2 & 15];
                    for (y = 0; y < h; y++)
                    {
                        yOffset = offset + y * lineWidth;

                        mask[yOffset] &= leftMask;
                        for (x = 1; x < w - 1; x++)
                        {
                            mask[yOffset + x] = 0;
                        }
                        if (x == w - 1)
                        {
                            mask[yOffset + x] &= rightMask;
                        }
                    }
                }
                else
                {
                    leftMask = lMask[x1 & 15];
                    rightMask = rMask[x2 & 15];
                    for (y = 0; y < h; y++)
                    {
                        yOffset = offset + y * lineWidth;

                        mask[yOffset] |= leftMask;
                        for (x = 1; x < w - 1; x++)
                        {
                            mask[yOffset + x] = 0xFFFF;
                        }
                        if (x == w - 1)
                        {
                            mask[yOffset + x] |= rightMask;
                        }
                    }
                }
            }
            else
            {
                if (val == 0)
                {
                    leftMask = (ushort) ~(lMask[x1 & 15] & rMask[x2 & 15]);
                    for (y = 0; y < h; y++)
                    {
                        yOffset = offset + y * lineWidth;
                        mask[yOffset] &= leftMask;
                    }
                }
                else
                {
                    leftMask = (ushort) (lMask[x1 & 15] & rMask[x2 & 15]);
                    for (y = 0; y < h; y++)
                    {
                        yOffset = offset + y * lineWidth;
                        mask[yOffset] |= leftMask;
                    }
                }
            }
        }

        public void orMask(CMask mask, int xx, int yy, int plans, int val)
        {
            if ((plans & CM_OBSTACLE) != 0)
            {
                if (obstacle != null)
                {
                    orIt(obstacle, mask, xx, yy, (val & 1) != 0);
                }
            }
            if ((plans & CM_PLATFORM) != 0)
            {
                if (platform != null)
                {
                    orIt(platform, mask, xx, yy, ((val >> 1) & 1) != 0);
                }
            }
        }

        public void orIt(ushort[] dMask, CMask sMask, int xx, int yy, bool bOr)
        {
            int x1 = xx;
            int y1 = yy;
//            x1 += mDxScroll;
//            y1 += mDyScroll;
            int x2 = xx + sMask.width;
            int y2 = yy + sMask.height;
            int dx = 0;
            int dy = 0;
            int fx = sMask.width;
            int fy = sMask.height;

            // Verifier le clipping
            // --------------------
            if (x1 < mX1Clip)
            {
                dx = mX1Clip - x1;
                if (dx > sMask.width)
                {
                    return;
                }
                x1 = mX1Clip;
            }
            if (x2 > mX2Clip)
            {
                fx = sMask.width - (x2 - mX2Clip);
                if (fx < 0)
                {
                    return;
                }
                x2 = mX2Clip;
            }

            if (y1 < mY1Clip)
            {
                dy = mY1Clip - y1;
                if (dy > sMask.height)
                {
                    return;
                }
                y1 = mY1Clip;
            }
            if (y2 > mY2Clip)
            {
                fy = sMask.height - (y2 - mY2Clip);
                if (fy < 0)
                {
                    return;
                }
                y2 = mY2Clip;
            }

            x1 -= mX1;
            x2 -= mX1;
            y1 -= mY1;
            y2 -= mY1;

            int h = fy - dy;
            int w = (fx / 16) - (dx / 16) + 1;
            uint x, y;
            ushort s;
            int offset, mOffset, shiftX;
            uint i;
            shiftX = x1 & 15;
            if (shiftX != 0)
            {
                switch (w)
                {
                    case 1:
                        if (bOr)
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));

                                i = (uint)(sMask.mask[(dy + y) * sMask.lineWidth + dx / 16] & lMask[dx & 15] & rMask[fx & 15] & (uint)0xFFFF);
                                dMask[offset] |= (ushort) (i >> shiftX);
                                if (x1 / 16 + 1 < lineWidth)
                                {
                                    dMask[++offset] |= (ushort) (i << (15 - shiftX));
                                }
                            }
                        }
                        else
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));
                                i = (uint)(sMask.mask[(dy + y) * sMask.lineWidth + dx / 16] & lMask[dx & 15] & rMask[fx & 15] & (uint)0xFFFF);
                                dMask[offset] &= (ushort)~(i >> shiftX);
                                offset++;
                                if (x1 / 16 + 1 < lineWidth)
                                {
                                    dMask[++offset] &= (ushort)~(i << (15 - shiftX));
                                }
                            }
                        }
                        break;
                    case 2:
                        if (bOr)
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));
                                mOffset = (int)((dy + y) * sMask.lineWidth + dx / 16);

                                i = (uint)(sMask.mask[mOffset] & lMask[dx & 15] & (uint)0xFFFF);
                                dMask[offset] |= (ushort) (i >> shiftX);
                                offset++;
                                dMask[offset] |= (ushort) (i << (16 - shiftX));

                                i = (uint)((sMask.mask[mOffset + 1] & rMask[fx & 15]) & (uint)0xFFFF);
                                dMask[offset] |= (ushort) (i >> shiftX);
                                if (x1 / 16 + 2 < lineWidth)
                                {
                                    dMask[++offset] |= (ushort) (i << (16 - shiftX));
                                }
                            }
                        }
                        else
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));
                                mOffset = (int)((dy + y) * sMask.lineWidth + dx / 16);

                                i = (uint)(sMask.mask[mOffset] & lMask[dx & 15] & (uint)0xFFFF);
                                dMask[offset] &= (ushort)~(i >> shiftX);
                                offset++;
                                dMask[offset] &= (ushort)~(i << (16 - shiftX));

                                i = (uint)(sMask.mask[mOffset + 1] & rMask[fx & 15] & (uint)0xFFFF);
                                dMask[offset] &= (ushort)~(i >> shiftX);
                                if (x1 / 16 + 2 < lineWidth)
                                {
                                    dMask[++offset] &= (ushort)~(i << (16 - shiftX));
                                }
                            }
                        }
                        break;
                    default:
                        if (bOr)
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));
                                mOffset = (int)((dy + y) * sMask.lineWidth + dx / 16);

                                // Gauche
                                i = (uint)(sMask.mask[mOffset] & lMask[dx & 15] & (uint)0xFFFF);
                                dMask[offset] |= (ushort) (i >> shiftX);
                                offset++;
                                dMask[offset] |= (ushort) (i << (16 - shiftX));

                                // Milieu
                                for (x = 1; x < w - 1; x++)
                                {
                                    i = sMask.mask[mOffset + x] & (uint)0xFFFF;
                                    dMask[offset] |= (ushort) (i >> shiftX);
                                    offset++;
                                    dMask[offset] |= (ushort) (i << (16 - shiftX));
                                }

                                // Droite
                                i = (uint)(sMask.mask[mOffset + x] & rMask[fx & 15] & (uint)0xFFFF);
                                dMask[offset] |= (ushort) (i >> shiftX);
                                if (x1 / 16 + x < lineWidth)
                                {
                                    dMask[++offset] |= (ushort) (i << (16 - shiftX));
                                }
                            }
                        }
                        else
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));
                                mOffset = (int)((dy + y) * sMask.lineWidth + dx / 16);

                                // Gauche
                                i = (uint)(sMask.mask[mOffset] & lMask[dx & 15] & (uint)0xFFFF);
                                dMask[offset] &= (ushort)~(i >> shiftX);
                                offset++;
                                dMask[offset] &= (ushort)~(i << (16 - shiftX));

                                // Milieu
                                for (x = 1; x < w - 1; x++)
                                {
                                    i = sMask.mask[mOffset + x] & (uint)0xFFFF;
                                    dMask[offset] &= (ushort)~(i >> shiftX);
                                    offset++;
                                    dMask[offset] &= (ushort)~(i << (16 - shiftX));
                                }

                                // Droite
                                i = (uint)(sMask.mask[mOffset + x] & rMask[fx & 15] & (uint)0xFFFF);
                                dMask[offset] &= (ushort)~(i >> shiftX);
                                if (x1 / 16 + x < lineWidth)
                                {
                                    dMask[++offset] &= (ushort)~(i << (16 - shiftX));
                                }
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (w)
                {
                    case 1:
                        if (bOr)
                        {
                            for (y = 0; y < h; y++)
                            {
                                s = (ushort) (sMask.mask[(dy + y) * sMask.lineWidth + dx / 16] & lMask[dx & 15] & rMask[fx & 15]);
                                dMask[(y1 + y) * lineWidth + (x1 / 16)] |= s;
                            }
                        }
                        else
                        {
                            for (y = 0; y < h; y++)
                            {
                                s = (ushort) (sMask.mask[(dy + y) * sMask.lineWidth + dx / 16] & lMask[dx & 15] & rMask[fx & 15]);
                                dMask[(y1 + y) * lineWidth + (x1 / 16)] &= (ushort)~s;
                            }
                        }
                        break;
                    case 2:
                        if (bOr)
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));
                                mOffset = (int)((dy + y) * sMask.lineWidth + dx / 16);

                                s = (ushort) (sMask.mask[mOffset] & lMask[dx & 15]);
                                dMask[offset] |= s;
                                s = (ushort) (sMask.mask[mOffset + 1] & rMask[fx & 15]);
                                dMask[offset + 1] |= s;
                            }
                        }
                        else
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));
                                mOffset = (int)((dy + y) * sMask.lineWidth + dx / 16);

                                s = (ushort) (sMask.mask[mOffset] & lMask[dx & 15]);
                                dMask[offset] &= (ushort)~s;
                                s = (ushort) (sMask.mask[mOffset + 1] & rMask[fx & 15]);
                                dMask[offset + 1] &= (ushort)~s;
                            }
                        }
                        break;
                    default:
                        if (bOr)
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));
                                mOffset = (int)((dy + y) * sMask.lineWidth + dx / 16);

                                // Gauche
                                s = (ushort) (sMask.mask[mOffset] & lMask[dx & 15]);
                                dMask[offset] |= s;

                                // Milieu
                                for (x = 1; x < w - 1; x++)
                                {
                                    s = sMask.mask[mOffset + x];
                                    dMask[offset + x] |= s;
                                }
                                if ((fx & 16) > 0)
                                {
                                    // Droite
                                    s = (ushort) (sMask.mask[mOffset + x] & rMask[fx & 15]);
                                    dMask[offset + x] |= s;
                                }
                            }
                        }
                        else
                        {
                            for (y = 0; y < h; y++)
                            {
                                offset = (int)((y1 + y) * lineWidth + (x1 / 16));
                                mOffset = (int)((dy + y) * sMask.lineWidth + dx / 16);

                                // Gauche
                                s = (ushort) (sMask.mask[mOffset] & lMask[dx & 15]);
                                dMask[offset] &= (ushort)~s;

                                // Milieu
                                for (x = 1; x < w - 1; x++)
                                {
                                    s = sMask.mask[mOffset + x];
                                    dMask[offset + x] &= (ushort)~s;
                                }

                                if ((fx & 16) > 0)
                                {
                                    // Droite
                                    s = (ushort) (sMask.mask[mOffset + x] & rMask[fx & 15]);
                                    dMask[offset + x] &= (ushort)~s;
                                }
                            }
                        }
                        break;
                }
            }
        }

        public void orPlatformMask(CMask sMask, int xx, int yy)
        {
            int x1 = xx;
            int y1 = yy;
//            x1 += mDxScroll;
//            y1 += mDyScroll;
            int x2 = xx + sMask.width;
            int y2 = yy + sMask.height;
            int dx = 0;
            int dy = 0;
            int fx = sMask.width;
            int fy = sMask.height;

            // Verifier le clipping
            // --------------------
            if (x1 < mX1Clip)
            {
                dx = mX1Clip - x1;
                if (dx > sMask.width)
                {
                    return;
                }
                x1 = mX1Clip;
            }
            if (x2 > mX2Clip)
            {
                fx = sMask.width - (x2 - mX2Clip);
                if (fx < 0)
                {
                    return;
                }
                x2 = mX2Clip;
            }

            if (y1 < mY1Clip)
            {
                dy = mY1Clip - y1;
                if (dy > sMask.height)
                {
                    return;
                }
                y1 = mY1Clip;
            }
            if (y2 > mY2Clip)
            {
                fy = sMask.height - (y2 - mY2Clip);
                if (fy < 0)
                {
                    return;
                }
                y2 = mY2Clip;
            }

            x1 -= mX1;
            x2 -= mX1;
            y1 -= mY1;
            y2 -= mY1;

            int h = fy - dy;
            int w = fx - dx;
            int x, y, yLimit;
            int xSOffset, xDOffset;
            ushort si, di;
            ushort[] mask = sMask.mask;
            for (x = 0; x < w; x++)
            {
                xSOffset = (dx + x) / 16;
                si = (ushort) (0x8000 >> ((dx + x) & 15));
                for (y = 0; y < h; y++)
                {
                    if ((mask[(dy + y) * sMask.lineWidth + xSOffset] & si) != 0)
                    {
                        break;
                    }
                }
                if (y < h)
                {
                    yLimit = Math.Min(y + HEIGHT_PLATFORM, h);
                    xDOffset = (x1 + x) / 16;
                    di = (ushort) (0x8000 >> ((x1 + x) & 15));
                    for (; y < yLimit; y++)
                    {
                        if ((mask[(dy + y) * sMask.lineWidth + xSOffset] & si) != 0)
                        {
                            platform[(y1 + y) * lineWidth + xDOffset] |= di;
                        }
                    }
                }
            }
        }

        public bool testPoint(int x, int y, int plans)
        {
            if (plans == CM_TEST_OBSTACLE)
            {
                if (obstacle != null)
                {
                    if (testPt(obstacle, x, y))
                    {
                        return true;
                    }
                }
            }
            if (plans == CM_TEST_PLATFORM)
            {
                if (platform != null)
                {
                    if (testPt(platform, x, y))
                    {
                        return true;
                    }
                }
                else if (obstacle != null)
                {
                    if (testPt(obstacle, x, y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool testPt(ushort[] mask, int x, int y)
        {
            x += mDxScroll;
            y += mDyScroll;
            if (x < mX1Clip || x > mX2Clip)
            {
                return false;
            }
            if (y < mY1Clip || y > mY2Clip)
            {
                return false;
            }
            x -= mX1;
            y -= mY1;

            int offset = y * lineWidth + x / 16;
            ushort m = (ushort) (0x8000 >> (x & 15));
            return (mask[offset] & m) != 0;
        }

        public bool testRect(int x, int y, int w, int h, int plans)
        {
            if (plans == CM_TEST_OBSTACLE)
            {
                if (obstacle != null)
                {
                    if (testRc(obstacle, x, y, w, h))
                    {
                        return true;
                    }
                }
            }
            if (plans == CM_TEST_PLATFORM)
            {
                if (platform != null)
                {
                    if (testRc(platform, x, y, w, h))
                    {
                        return true;
                    }
                }
                else if (obstacle != null)
                {
                    if (testRc(obstacle, x, y, w, h))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool testRc(ushort[] mask, int xx, int yy, int sx, int sy)
        {
            int x1 = xx;
            int y1 = yy;
            x1 += mDxScroll;
            y1 += mDyScroll;
            int x2 = x1 + sx;
            int y2 = y1 + sy;

            // Verifier le clipping
            // --------------------
            if (x1 < mX1Clip)
            {
                x1 = mX1Clip;
            }
            if (x2 > mX2Clip)
            {
                x2 = mX2Clip;
            }

            if (y1 < mY1Clip)
            {
                y1 = mY1Clip;
            }
            if (y2 > mY2Clip)
            {
                y2 = mY2Clip;
            }

            if (x2 <= x1 || y2 <= y1)
            {
                return false;
            }

            x1 -= mX1;
            x2 -= mX1;
            y1 -= mY1;
            y2 -= mY1;

            int h = y2 - y1;
            int w = ((x2 - 1) / 16) - (x1 / 16) + 1;
            int x, y;
            ushort s;
            int offset;

            switch (w)
            {
                case 1:
                    s = (ushort) (lMask[x1 & 15] & rMask[((x2-1)&15)+1]);
                    for (y = 0; y < h; y++)
                    {
                        offset = (y1 + y) * lineWidth + x1 / 16;
                        if ((mask[offset] & s) != 0)
                        {
                            return true;
                        }
                    }
                    break;
                case 2:
                    for (y = 0; y < h; y++)
                    {
                        offset = (y1 + y) * lineWidth + x1 / 16;
                        if ((mask[offset] & lMask[x1 & 15]) != 0)
                        {
                            return true;
                        }
                        if ((mask[offset + 1] & rMask[((x2-1)&15)+1]) != 0)
                        {
                            return true;
                        }
                    }
                    break;
                default:
                    for (y = 0; y < h; y++)
                    {
                        offset = (y1 + y) * lineWidth + x1 / 16;
                        if ((mask[offset] & lMask[x1 & 15]) != 0)
                        {
                            return true;
                        }
                        for (x = 1; x < w - 1; x++)
                        {
                            if ((mask[offset + x] != 0))
                            {
                                return true;
                            }
                        }
                        if ((mask[offset + x] & rMask[((x2-1)&15)+1]) != 0)
                        {
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        public bool testMask(CMask mask, int yBase, int xx, int yy, int plans)
        {
            if (plans == CM_TEST_OBSTACLE)
            {
                if (obstacle != null)
                {
                    if (testIt(obstacle, mask, yBase, xx, yy))
                    {
                        return true;
                    }
                }
            }
            if (plans == CM_TEST_PLATFORM)
            {
                if (platform != null)
                {
                    if (testIt(platform, mask, yBase, xx, yy))
                    {
                        return true;
                    }
                }
                else if (obstacle != null)
                {
                    if (testIt(obstacle, mask, yBase, xx, yy))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool testIt(ushort[] dMask, CMask sMask, int yBase, int xx, int yy)
        {
            int x1 = xx;
            int y1 = yy;
            x1 += mDxScroll;
            y1 += mDyScroll;
            int x2 = x1 + sMask.width;
            int y2 = y1 + sMask.height;
            int dx = 0;
            int dy = yBase;
            int fx = sMask.width;
            int fy = sMask.height;

            // Verifier le clipping
            // --------------------
            if (x1 < mX1Clip)
            {
                dx = mX1Clip - x1;
                if (dx > sMask.width)
                {
                    return false;
                }
                x1 = mX1Clip;
            }
            if (x2 > mX2Clip)
            {
                fx = sMask.width - (x2 - mX2Clip);
                if (fx < 0)
                {
                    return false;
                }
                x2 = mX2Clip;
            }

            if (y1 < mY1Clip)
            {
                dy = mY1Clip - y1;
                if (dy > sMask.height)
                {
                    return false;
                }
                y1 = mY1Clip;
            }
            if (y2 > mY2Clip)
            {
                fy = sMask.height - (y2 - mY2Clip);
                if (fy < 0)
                {
                    return false;
                }
                y2 = mY2Clip;
            }
            if (fx <= dx)
            {
                return false;
            }

            x1 -= mX1;
            x2 -= mX1;
            y1 -= mY1;
            y2 -= mY1;

            int h = fy - dy;
            int w = (fx - dx + 15) / 16;
            int x, y;
            ushort s;
            int offset, mOffset, shiftX;
            uint i;
            shiftX = x1 & 15;
            if (shiftX != 0)
            {
                switch (w)
                {
                    case 1:
                        for (y = 0; y < h; y++)
                        {
                            offset = (y1 + y) * lineWidth + (x1 / 16);

                            i = (uint)(sMask.mask[(dy + y) * sMask.lineWidth + dx / 16] & lMask[dx & 15] & rMask[((fx-1)&15)+1] & 0xFFFF);
                            if ((dMask[offset] & (ushort) (i >> shiftX)) != 0)
                            {
                                return true;
                            }
                            if (x1 / 16 + 1 < lineWidth)
                            {
                                if ((dMask[++offset] & (ushort) (i << (15 - shiftX))) != 0)
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                    case 2:
                        for (y = 0; y < h; y++)
                        {
                            offset = (y1 + y) * lineWidth + (x1 / 16);
                            mOffset = (dy + y) * sMask.lineWidth + dx / 16;

                            i = (uint)(sMask.mask[mOffset] & lMask[dx & 15] & (uint)0xFFFF);
                            if ((dMask[offset] & (ushort) (i >> shiftX)) != 0)
                            {
                                return true;
                            }
                            offset++;
                            if ((dMask[offset] & (ushort) (i << (16 - shiftX))) != 0)
                            {
                                return true;
                            }

                            i = (uint)((sMask.mask[mOffset + 1] & rMask[((fx-1)&15)+1]) & 0xFFFF);
                            if ((dMask[offset] & (ushort) (i >> shiftX)) != 0)
                            {
                                return true;
                            }
                            if (x1 / 16 + 2 < lineWidth)
                            {
                                if ((dMask[++offset] & (ushort) (i << (16 - shiftX))) != 0)
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                    default:
                        for (y = 0; y < h; y++)
                        {
                            offset = (y1 + y) * lineWidth + (x1 / 16);
                            mOffset = (dy + y) * sMask.lineWidth + dx / 16;

                            // Gauche
                            i = (uint)(sMask.mask[mOffset] & lMask[dx & 15] & 0xFFFF);
                            if ((dMask[offset] & (ushort) (i >> shiftX)) != 0)
                            {
                                return true;
                            }
                            offset++;
                            if ((dMask[offset] & (ushort) (i << (16 - shiftX))) != 0)
                            {
                                return true;
                            }

                            // Milieu
                            for (x = 1; x < w - 1; x++)
                            {
                                i = (uint)(sMask.mask[mOffset + x] & 0xFFFF);
                                if ((dMask[offset] & (ushort) (i >> shiftX)) != 0)
                                {
                                    return true;
                                }
                                offset++;
                                if ((dMask[offset] & (ushort) (i << (16 - shiftX))) != 0)
                                {
                                    return true;
                                }
                            }

                            // Droite
                            i = (uint)(sMask.mask[mOffset + x] & rMask[((fx-1)&15)+1] & 0xFFFF);
                            if ((dMask[offset] & (ushort) (i >> shiftX)) != 0)
                            {
                                return true;
                            }
                            if (x1 / 16 + x < lineWidth)
                            {
                                if ((dMask[++offset] & (ushort) (i << (16 - shiftX))) != 0)
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (w)
                {
                    case 1:
                        for (y = 0; y < h; y++)
                        {
                            s = (ushort) (sMask.mask[(dy + y) * sMask.lineWidth + dx / 16] & lMask[dx & 15] & rMask[((fx-1)&15)+1]);
                            if ((dMask[(y1 + y) * lineWidth + (x1 / 16)] & s) != 0)
                            {
                                return true;
                            }
                        }
                        break;
                    case 2:
                        for (y = 0; y < h; y++)
                        {
                            offset = (y1 + y) * lineWidth + (x1 / 16);
                            mOffset = (dy + y) * sMask.lineWidth + dx / 16;

                            s = (ushort) (sMask.mask[mOffset] & lMask[dx & 15]);
                            if ((dMask[offset] & s) != 0)
                            {
                                return true;
                            }
                            s = (ushort) (sMask.mask[mOffset + 1] & rMask[((fx-1)&15)+1]);
                            if ((dMask[offset + 1] & s) != 0)
                            {
                                return true;
                            }
                        }
                        break;
                    default:
                        for (y = 0; y < h; y++)
                        {
                            offset = (y1 + y) * lineWidth + (x1 / 16);
                            mOffset = (dy + y) * sMask.lineWidth + dx / 16;

                            // Gauche
                            s = (ushort) (sMask.mask[mOffset] & lMask[dx & 15]);
                            if ((dMask[offset] & s) != 0)
                            {
                                return true;
                            }

                            // Milieu
                            for (x = 1; x < w - 1; x++)
                            {
                                s = sMask.mask[mOffset + x];
                                if ((dMask[offset + x] & s) != 0)
                                {
                                    return true;
                                }
                            }

                            // Droite
                            s = (ushort) (sMask.mask[mOffset + x] & rMask[((fx-1)&15)+1]);
                            if ((dMask[offset + x] & s) != 0)
                            {
                                return true;
                            }
                        }
                        break;
                }
            }
            return false;
        }
    }
}
