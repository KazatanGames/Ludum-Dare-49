namespace KazatanGames.Framework
{
    using System;

    public struct GridPosDir : IEquatable<GridPosDir>
    {
        public GridPos pos;
        public GameDirection dir;

        public GridPosDir(GridPos pos, GameDirection dir)
        {
            this.pos = pos;
            this.dir = dir;
        }

        public static bool operator ==(GridPosDir gp1, GridPosDir gp2)
        {
            return gp1.Equals(gp2);
        }
        public static bool operator !=(GridPosDir gp1, GridPosDir gp2)
        {
            return !gp1.Equals(gp2);
        }

        public override bool Equals(Object other)
        {
            if (other is GridPosDir)
            {
                return Equals((GridPosDir)other);
            }
            return false;
        }
        public bool Equals(GridPosDir other)
        {
            return other.pos == pos && other.dir == dir;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{pos}+{dir}";
        }

        public GridPosDir Pair { get { return new GridPosDir(pos.Adjacent(dir), dir.Opposite()); } }
    }
}