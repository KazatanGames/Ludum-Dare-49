namespace KazatanGames.Framework
{
    using System;
    // no UnityEngine using directive as it conflicts with System.Object

    public struct GridPos : IEquatable<GridPos>
    {
        public static GridPos Invalid { get { return new GridPos(-1, -1); } }

        public static implicit operator UnityEngine.Vector2(GridPos pos) { return pos.ToVector2(); }
        public static implicit operator UnityEngine.Vector3(GridPos pos) { return pos.ToVector3(); }

        public int x;
        public int z;

        public GridPos(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
        public GridPos(UnityEngine.Vector2Int input)
        {
            x = input.x;
            z = input.y;
        }
        public GridPos(UnityEngine.Vector3Int input)
        {
            x = input.x;
            z = input.z;
        }
        public GridPos(UnityEngine.Vector2 input)
        {
            x = UnityEngine.Mathf.FloorToInt(input.x);
            z = UnityEngine.Mathf.FloorToInt(input.y);
        }
        public GridPos(UnityEngine.Vector3 input)
        {
            x = UnityEngine.Mathf.FloorToInt(input.x);
            z = UnityEngine.Mathf.FloorToInt(input.z);
        }

        public static bool operator ==(GridPos gp1, GridPos gp2)
        {
            return gp1.Equals(gp2);
        }
        public static bool operator !=(GridPos gp1, GridPos gp2)
        {
            return !gp1.Equals(gp2);
        }
        public static GridPos operator -(GridPos gp1, GridPos gp2)
        {
            return new GridPos(gp1.x - gp2.x, gp1.z - gp2.z);
        }
        public static GridPos operator +(GridPos gp1, GridPos gp2)
        {
            return new GridPos(gp1.x + gp2.x, gp1.z + gp2.z);
        }

        public override bool Equals(Object other)
        {
            if (other is GridPos)
            {
                return Equals((GridPos)other);
            }
            return false;
        }
        public bool Equals(GridPos other)
        {
            return other.x == x && other.z == z;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"({x},{z})";
        }

        public UnityEngine.Vector2 ToVector2()
        {
            return new UnityEngine.Vector2(x, z);
        }

        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(x, 0, z);
        }

        public GameDirection DirectionTo(GridPos adj)
        {
            GameDirection result = GameDirectionHelper.Nil;
            if (adj.z > z) result |= GameDirection.North;
            if (adj.x > x) result |= GameDirection.East;
            if (adj.z < z) result |= GameDirection.South;
            if (adj.x < x) result |= GameDirection.West;
            return result;
        }

        public GridPosDir[] AllDirections
        {
            get
            {
                return new GridPosDir[]
                {
                new GridPosDir(this, GameDirection.North),
                new GridPosDir(this, GameDirection.East),
                new GridPosDir(this, GameDirection.South),
                new GridPosDir(this, GameDirection.West)
                };
            }
        }

        public GridPos[] Neighbours
        {
            get
            {
                return new GridPos[]
                {
                new GridPos(x - 1, z),
                new GridPos(x, z - 1),
                new GridPos(x + 1, z),
                new GridPos(x, z + 1)
                };
            }
        }
    }

    public static class GridPosExtensions
    {
        public static GridPos Adjacent(this GridPos me, GameDirection dir)
        {
            if (dir == GameDirection.North)
            {
                return new GridPos(me.x, me.z + 1);
            }
            if (dir == GameDirection.East)
            {
                return new GridPos(me.x + 1, me.z);
            }
            if (dir == GameDirection.South)
            {
                return new GridPos(me.x, me.z - 1);
            }
            if (dir == GameDirection.West)
            {
                return new GridPos(me.x - 1, me.z);
            }

            return GridPos.Invalid;
        }
    }
}