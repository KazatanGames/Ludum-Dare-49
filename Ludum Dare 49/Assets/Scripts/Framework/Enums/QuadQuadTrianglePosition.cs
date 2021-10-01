namespace KazatanGames.Framework
{
    public enum QuadQuadTrianglePosition
    {
        /**
         * Triangles are ordered as such:
         * +-----+-----+
         * | 0  /|\  2 |   
         * |  /  |  \  |
         * |/  1 | 3  \|
         * +-----+-----+
         * |\  7 | 5  /|
         * |  \  |  /  |
         * | 6  \|/  4 |
         * +-----+-----+
         */
        OuterTL = 0,
        InnerTL = 1,
        OuterTR = 2,
        InnerTR = 3,
        OuterBR = 4,
        InnerBR = 5,
        OuterBL = 6,
        InnerBL = 7
    }
}