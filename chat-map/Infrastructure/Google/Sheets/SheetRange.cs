using System;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public struct SheetRange : IEquatable<SheetRange>
    {
        public SheetRange(int startX, int startY, int endX, int endY)
        {
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
        }

        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }

        public bool Equals(SheetRange other)
        {
            return StartX == other.StartX && StartY == other.StartY && EndX == other.EndX && EndY == other.EndY;
        }

        public static bool operator ==(SheetRange one, SheetRange two) => one.Equals(two);

        public static bool operator !=(SheetRange one, SheetRange two) => !one.Equals(two);

        public static readonly SheetRange None = new SheetRange(-1, -1, -1, -1);

        public int LengthX
        {
            get
            {
                if(StartX == -1)
                    throw new InvalidOperationException($"Length is not supported for {typeof(SheetRange).FullName}.{nameof(None)}");

                return EndX - StartX + 1;
            }
        }

        public int LengthY
        {
            get
            {
                if (StartY == -1)
                    throw new InvalidOperationException($"Length is not supported for {typeof(SheetRange).FullName}.{nameof(None)}");

                return EndY - StartY + 1;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(SheetRange))
                return false;

            return Equals((SheetRange)obj);
        }

        public override int GetHashCode()
        {
            return StartX ^ 31 * EndX ^ 961 * StartY ^ 29791 * EndY;
        }
    }
}
